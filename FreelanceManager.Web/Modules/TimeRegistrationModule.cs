using FreelanceManager.ReadModel.Repositories;
using Nancy;
using Nancy.Security;
using FreelanceManager.Web.Shared;
using Nancy.ModelBinding;

namespace FreelanceManager.Web.Modules
{
    public class TimeRegistrationModule : NancyModule
    {
        public TimeRegistrationModule(IProjectRepository projectRepository, 
                                      IClientRepository clientRepository, 
                                      ITimeRegistrationRepository timeRegistrationRepository, 
                                      IIdGenerator idGenerator, 
                                      IAggregateRootRepository aggregateRootRepository)
            : base("/timeregistration")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => View["Index"];

            Get["/report"] = _ => View["Report"];

            Get["/export"] = _ => View["Export"];

            Post["/export"] = parameters =>
            {
                var items = timeRegistrationRepository.GetForMonth((int)Request.Form.year, (int)Request.Form.month);
                return Response.AsExcel((int)Request.Form.year, (int)Request.Form.month, items);
            };
        }

        //[HttpPost]
        //public ActionResult Export(int year, int month)
        //{
        //    var items = _timeRegistrationRepository.GetForMonth(year, month);
        //    return new TimeRegistrationsToExcelActionResult { Items = items, Year = year, Month = month };
        //}

        //public ActionResult Import(string id)
        //{
        //    var viewModel = new ImportViewModel();

        //    if (id != null)
        //    {
        //        viewModel.ServerFile = Path.Combine(Server.MapPath("~/App_Data/Uploads"), id);
        //        var excelPackage = new ExcelPackage(new FileInfo(viewModel.ServerFile));
        //        var worksheet = excelPackage.Workbook.Worksheets.First();

        //        viewModel.ColumnNames = new List<ImportViewModel.ColumnInfo>();
        //        foreach (var firstRowCell in worksheet.Cells[worksheet.Dimension.Start.Row, worksheet.Dimension.Start.Column, 1, worksheet.Dimension.End.Column])
        //            viewModel.ColumnNames.Add(new ImportViewModel.ColumnInfo { Column = firstRowCell.Start.Column, Name = firstRowCell.Text });
        //    }

        //    return View(viewModel);
        //}

        //[HttpPost]
        //public ActionResult ImportUpload(ImportViewModel viewModel)
        //{
        //    var fileName = Guid.NewGuid().ToString("N").Substring(0, 4) + "_" + Path.GetFileName(viewModel.ExcelFile.FileName);
        //    var path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);

        //    viewModel.ExcelFile.SaveAs(path);

        //    return RedirectToAction("Import", new { id = fileName });
        //}

        //[HttpPost]
        //public ActionResult Import(ImportViewModel viewModel)
        //{
        //    var excelPackage = new ExcelPackage(new FileInfo(viewModel.ServerFile));
        //    var worksheet = excelPackage.Workbook.Worksheets.First();
        //    var clients = new Dictionary<Guid, Domain.Client>();
        //    var projects = new Dictionary<Guid, Domain.Project>();

        //    for (int r = 2; r <= worksheet.Dimension.End.Row; r++)
        //    {
        //        Guid clientId, projectId;

        //        Guid.TryParse(worksheet.Cells[r, viewModel.ClientIdColumn].Text, out clientId);
        //        Guid.TryParse(worksheet.Cells[r, viewModel.ProjectIdColumn].Text, out projectId);

        //        var client = clients.GetOrAdd(clientId, key => _aggregateRootRepository.GetById<Domain.Client>(key));
        //        var project = projects.GetOrAdd(projectId, key => _aggregateRootRepository.GetById<Domain.Project>(key));

        //        var from = DateTime.Parse(worksheet.Cells[r, viewModel.FromColumn].Text);
        //        var to = DateTime.Parse(worksheet.Cells[r, viewModel.ToColumn].Text);
        //        var rate = decimal.Parse(worksheet.Cells[r, viewModel.RateColumn].Text);

        //        var timeRegistration = new Domain.TimeRegistration(_idGenerator.NextGuid(),
        //            client, project, new Domain.ValueObjects.Task
        //            {
        //                Name = worksheet.Cells[r, viewModel.TaskColumn].Text,
        //                Rate = rate
        //            },
        //            worksheet.Cells[r, viewModel.DescriptionColumn].Text,
        //            new Date(from.Year, from.Month, from.Day),
        //            new Time(from.Hour, from.Minute),
        //            new Time(to.Hour, to.Minute));

        //        _aggregateRootRepository.Save(timeRegistration);
        //    }

        //    return RedirectToAction("Index");
        //}
    }
}