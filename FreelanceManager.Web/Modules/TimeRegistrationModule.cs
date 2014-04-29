using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Web.Tools;
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
                                      IRootPathProvider rootPathProvider,
                                      IExcelService excelService)
            : base("/timeregistration")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => View["Index"];

            Get["/report"] = _ => View["Report"];

            Get["/import"] = _ => View["Import"];

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

            Post["/import"] = _ =>
            {
                var viewModel = this.Bind<ImportViewModel>();

                var result = excelService.Import(viewModel.ServerFile, viewModel.ProjectIdColumn, viewModel.TaskColumn,
                                                 viewModel.DateColumn, viewModel.FromColumn, viewModel.ToColumn, 
                                                 viewModel.RateColumn, viewModel.DescriptionColumn);

                return View["ImportResult", result];
            };

            Get["/export"] = parameters =>
            {
                var items = timeRegistrationRepository.GetAll();
                return Response.AsExcel(items);
            };
        }
    }
}