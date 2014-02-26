using System;
using FreelanceManager.Web.Api.Write.Models.TimeRegistrations;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace FreelanceManager.Web.Api.Write
{
    public class TimeRegistrationModule : NancyModule
    {
        public TimeRegistrationModule(IAggregateRootRepository repository,
                                      IIdGenerator idGenerator)
            : base("/write/timeregistration")
        {
            this.RequiresAuthentication();

            Post["/create"] = parameters =>
            {
                var model = this.Bind<EditableTimeRegistration>();

                var client = repository.GetById<Domain.Client>(model.ClientId);
                var project = repository.GetById<Domain.Project>(model.ProjectId);
                var task = project.FindTask(model.Task);

                var date = Date.Parse(model.Date);
                var from = Time.Parse(model.From);
                var to = Time.Parse(model.To);

                var timeRegistration = new Domain.TimeRegistration(idGenerator.NextGuid(),
                    client, project, task,
                    model.Description, date, from, to);

                if (model.CorrectedIncome.HasValue)
                    timeRegistration.CorrectIncome(model.CorrectedIncome.Value, model.CorrectedIncomeMessage);

                repository.Save(timeRegistration);

                return Response.AsJson(new
                {
                    TimeRegistration = new TimeRegistration(timeRegistration, client, project)
                });
            };

            Post["/update/{id:guid}"] = parameters =>
            {
                var model = this.Bind<EditableTimeRegistration>();

                var timeRegistration = repository.GetById<Domain.TimeRegistration>((Guid)parameters.id);

                if (timeRegistration != null)
                {
                    var client = repository.GetById<Domain.Client>(model.ClientId);
                    var project = repository.GetById<Domain.Project>(model.ProjectId);
                    var task = project.FindTask(model.Task);

                    var date = Date.Parse(model.Date);
                    var from = Time.Parse(model.From);
                    var to = Time.Parse(model.To);

                    timeRegistration.ChangeDetails(client, project, task,
                        model.Description, date, from, to);

                    if (model.CorrectedIncome.HasValue)
                        timeRegistration.CorrectIncome(model.CorrectedIncome.Value, model.CorrectedIncomeMessage);
                    else
                        timeRegistration.ClearCorrectedIncome();

                    if (model.RefreshRate)
                        timeRegistration.RefreshRate(task);

                    repository.Save(timeRegistration);

                    return Response.AsJson(new
                    {
                        TimeRegistration = new TimeRegistration(timeRegistration, client, project)
                    });
                }

                return null;
            };

            Post["/delete/{id:guid}"] = parameters =>
            {
                var timeRegistration = repository.GetById<Domain.TimeRegistration>((Guid)parameters.id);

                if (timeRegistration != null)
                {
                    timeRegistration.Delete();

                    repository.Save(timeRegistration);
                }

                return null;
            };
        }
    }
}