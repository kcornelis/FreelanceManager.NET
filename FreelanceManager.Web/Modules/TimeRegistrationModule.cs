using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Web.Shared;
using FreelanceManager.Web.ViewModels.TimeRegistration;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using OfficeOpenXml;

namespace FreelanceManager.Web.Modules
{
    public class TimeRegistrationModule : NancyModule
    {
        public TimeRegistrationModule(IProjectRepository projectRepository, 
                                      IClientRepository clientRepository, 
                                      ITimeRegistrationRepository timeRegistrationRepository, 
                                      IIdGenerator idGenerator, 
                                      IAggregateRootRepository aggregateRootRepository,
                                      IRootPathProvider rootPathProvider)
            : base("/timeregistration")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => View["Index"];

            Get["/report"] = _ => View["Report"];

            Get["/import"] = _ => View["Import"];

            Get["/importmap/{url*}"] = parameters =>
            {
                var viewModel = new ImportViewModel();
                
                viewModel.ServerFile = Path.Combine(rootPathProvider.GetRootPath(), "App_Data", "Uploads", parameters.url);
                var excelPackage = new ExcelPackage(new FileInfo(viewModel.ServerFile));
                var worksheet = excelPackage.Workbook.Worksheets.First();

                viewModel.ColumnNames = new List<ImportViewModel.ColumnInfo>();
                foreach (var firstRowCell in worksheet.Cells[worksheet.Dimension.Start.Row, worksheet.Dimension.Start.Column, 1, worksheet.Dimension.End.Column])
                    viewModel.ColumnNames.Add(new ImportViewModel.ColumnInfo { Column = firstRowCell.Start.Column, Name = firstRowCell.Text });

                return View["ImportMap", viewModel];
            };

            Post["/importupload"] = _ =>
            {
                var fileName = Guid.NewGuid().ToString("N").Substring(0, 4) + "_" + Path.GetFileName((string)Request.Files.First().Name);
                var path = Path.Combine(rootPathProvider.GetRootPath(), "App_Data", "Uploads", fileName);
               
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    Request.Files.First().Value.CopyTo(fileStream);
                }

                return Response.AsRedirect("~/timeregistration/importmap/" + HttpUtility.UrlEncode(fileName));
            };

            // TODO return/show import results
            Post["/import"] = _ =>
            {
                var viewModel = this.Bind<ImportViewModel>();

                var excelPackage = new ExcelPackage(new FileInfo(viewModel.ServerFile));
                var worksheet = excelPackage.Workbook.Worksheets.First();
                var clients = new Dictionary<Guid, Domain.Client>();
                var projects = new Dictionary<Guid, Domain.Project>();

                for (int r = 2; r <= worksheet.Dimension.End.Row; r++)
                {
                    Guid clientId, projectId;

                    Guid.TryParse(worksheet.Cells[r, viewModel.ClientIdColumn].Text, out clientId);
                    Guid.TryParse(worksheet.Cells[r, viewModel.ProjectIdColumn].Text, out projectId);

                    var client = clients.GetOrAdd(clientId, key => aggregateRootRepository.GetById<Domain.Client>(key));
                    var project = projects.GetOrAdd(projectId, key => aggregateRootRepository.GetById<Domain.Project>(key));

                    var from = DateTime.Parse(worksheet.Cells[r, viewModel.FromColumn].Text);
                    var to = DateTime.Parse(worksheet.Cells[r, viewModel.ToColumn].Text);
                    var rate = decimal.Parse(worksheet.Cells[r, viewModel.RateColumn].Text);

                    var timeRegistration = new Domain.TimeRegistration(idGenerator.NextGuid(),
                        client, project, new Domain.ValueObjects.Task
                        {
                            Name = worksheet.Cells[r, viewModel.TaskColumn].Text,
                            Rate = rate
                        },
                        worksheet.Cells[r, viewModel.DescriptionColumn].Text,
                        new Date(from.Year, from.Month, from.Day),
                        new Time(from.Hour, from.Minute),
                        new Time(to.Hour, to.Minute));

                    aggregateRootRepository.Save(timeRegistration);
                }

                return Response.AsRedirect("~/timeregistration");
            };

            Get["/export"] = _ => View["Export"];

            Post["/export"] = parameters =>
            {
                var items = timeRegistrationRepository.GetForMonth((int)Request.Form.year, (int)Request.Form.month);
                return Response.AsExcel((int)Request.Form.year, (int)Request.Form.month, items);
            };
        }
    }
}