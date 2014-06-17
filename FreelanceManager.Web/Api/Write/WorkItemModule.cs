using System;
using FreelanceManager.Web.Api.Write.Models.WorkItem;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace FreelanceManager.Web.Api.Write
{
    public class WorkItemModule : ApiModule
    {
        public WorkItemModule(IAggregateRootRepository repository,
                              IIdGenerator idGenerator)
            : base("/write/workitem")
        {
            this.RequiresAuthentication();

            Post["/create"] = parameters =>
            {
                var model = this.Bind<EditableWorkItem>();

                var dueDate = !string.IsNullOrWhiteSpace(model.DueDate) ? Date.Parse(model.DueDate) : null;
                var dueTime = !string.IsNullOrWhiteSpace(model.DueTime) ? Time.Parse(model.DueTime) : null;

                var workItem = new Domain.WorkItem(idGenerator.NextGuid(), model.Description, dueDate, dueTime);

                repository.Save(workItem);

                return Json(new
                {
                    WorkItem = new WorkItem(workItem)
                });
            };

            Post["/update/{id:guid}"] = parameters =>
            {
                var model = this.Bind<EditableWorkItem>();

                var workItem = repository.GetById<Domain.WorkItem>((Guid)parameters.id);

                if (workItem != null)
                {
                    var dueDate = !string.IsNullOrWhiteSpace(model.DueDate) ? Date.Parse(model.DueDate) : null;
                    var dueTime = !string.IsNullOrWhiteSpace(model.DueTime) ? Time.Parse(model.DueTime) : null;

                    workItem.ChangeDetails(model.Description);
                    workItem.ChangeDueDate(dueDate, dueTime);

                    repository.Save(workItem);

                    return Json(new
                    {
                        WorkItem = new WorkItem(workItem)
                    });
                }

                return null;
            };

            Post["/delete/{id:guid}"] = parameters =>
            {
                var workItem = repository.GetById<Domain.WorkItem>((Guid)parameters.id);

                if (workItem != null)
                {
                    workItem.Delete();

                    repository.Save(workItem);
                }

                return null;
            };
        }
    }
}