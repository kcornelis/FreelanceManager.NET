using System;
using System.Linq;
using FreelanceManager.Web.Api.Write.Models.Projects;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace FreelanceManager.Web.Api.Write
{
    public class ProjectModule: ApiModule
    {
        public ProjectModule(IAggregateRootRepository repository,
                             IIdGenerator idGenerator)
            :base("/write/project")
        {
            this.RequiresAuthentication();

            Post["/create"] = parameters =>
            {
                var model = this.Bind<EditableProject>();

                var client = repository.GetById<Domain.Client>(model.ClientId);
                var project = new Domain.Project(idGenerator.NextGuid(), model.Name, model.Description, client);

                repository.Save(project);

                return Json(new
                {
                    Project = new Project(client, project)
                });
            };

            Post["/update/{id:guid}"] = parameters =>
            {
                var model = this.Bind<EditableProject>();

                var project = repository.GetById<Domain.Project>((Guid)parameters.id);

                if (project != null)
                {
                    project.ChangeDetails(model.Name, model.Description);

                    if (model.Hidden)
                        project.Hide();
                    else project.Unhide();

                    repository.Save(project);

                    var client = repository.GetById<Domain.Client>(project.ClientId);

                    return Json(new
                    {
                        Project = new Project(client, project)
                    });
                }

                return null;
            };

            Post["/updatetasks/{id:guid}"] = parameters =>
            {
                var model = this.Bind<EditableProjectTasks>();

                var project = repository.GetById<Domain.Project>((Guid)parameters.id);

                if (project != null)
                {
                    project.ChangeTasks(model.Tasks.Select(t => new Domain.ValueObjects.Task { Name = t.Name, Rate = t.Rate }).ToArray());

                    repository.Save(project);

                    var client = repository.GetById<Domain.Client>(project.ClientId);

                    return Json(new
                    {
                        Project = new Project(client, project)
                    });
                }

                return null;
            };
        }
    }
}