using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;

namespace FreelanceManager.Web.Tools
{
    public interface IExcelService
    {
        ExcelImportResult Import(string file, int projectColumn, int taskColumn,
                                 int dateColumn, int fromColumn, int toColumn, int rateColumn,
                                 int descriptionColumn);
    }

    public class ExcelService : IExcelService
    {
        private IAggregateRootRepository _aggregateRootRepository;
        private IIdGenerator _idGenerator;

        public ExcelService(IAggregateRootRepository aggregateRootRepository,
                            IIdGenerator idGenerator)
        {
            _aggregateRootRepository = aggregateRootRepository;
            _idGenerator = idGenerator;
        }

        public ExcelImportResult Import(string file, int projectColumn, int taskColumn,
                                        int dateColumn, int fromColumn, int toColumn, int rateColumn,
                                        int descriptionColumn)
        {
            var excelPackage = new ExcelPackage(new FileInfo(file));
            var worksheet = excelPackage.Workbook.Worksheets.First();
            var clients = new Dictionary<Guid, Domain.Client>();
            var projects = new Dictionary<Guid, Domain.Project>();
            var result = new ExcelImportResult();

            for (int r = 2; r <= worksheet.Dimension.End.Row; r++)
            {
                try
                {
                    Guid projectId;

                    Guid.TryParse(worksheet.Cells[r, projectColumn].Text, out projectId);

                    var project = projects.GetOrAdd(projectId, key => _aggregateRootRepository.GetById<Domain.Project>(key));
                    var client = clients.GetOrAdd(project.ClientId, key => _aggregateRootRepository.GetById<Domain.Client>(key));

                    if (project == null)
                    {
                        result.Errors.Add(r, "Project not found.");
                        continue;
                    }

                    DateTime date;
                    DateTime from;
                    DateTime to;
                    decimal rate;

                    if (!DateTime.TryParseExact(worksheet.Cells[r, dateColumn].Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        result.Errors.Add(r, "Date does not contain a valid date.");
                        continue;
                    }

                    if (!DateTime.TryParseExact(worksheet.Cells[r, fromColumn].Text, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out from))
                    {
                        result.Errors.Add(r, "From does not contain a valid time.");
                        continue;
                    }

                    if (!DateTime.TryParseExact(worksheet.Cells[r, toColumn].Text, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out to))
                    {
                        result.Errors.Add(r, "To does not contain a valid time.");
                        continue;
                    }

                    if (!decimal.TryParse(worksheet.Cells[r, rateColumn].Text, out rate))
                    {
                        result.Errors.Add(r, "Rate does not contain a valid number.");
                        continue;
                    }

                    var timeRegistration = new Domain.TimeRegistration(_idGenerator.NextGuid(),
                        client, project, new Domain.ValueObjects.Task
                        {
                            Name = worksheet.Cells[r, taskColumn].Text,
                            Rate = rate
                        },
                        worksheet.Cells[r, descriptionColumn].Text,
                        new Date(date.Year, date.Month, date.Day),
                        new Time(from.Hour, from.Minute),
                        new Time(to.Hour, to.Minute));

                    _aggregateRootRepository.Save(timeRegistration);

                    result.Success++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(r, ex.Message);
                }
            }

            return result;
        }
    }

    public class ExcelImportResult
    {
        public ExcelImportResult()
        {
            Errors = new Dictionary<int, string>();
        }

        public int Success { get; set; }
        public Dictionary<int, string> Errors { get; set; }
    }
}