using System;
using System.Linq;
using FreelanceManager.ReadModel.Repositories;
using Nancy;
using Nancy.Security;

namespace FreelanceManager.Web.Areas.Read
{
    public class ProjectModule : NancyModule
    {
        public ProjectModule(IProjectRepository projectRepository)
            :base("/read/projects")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => Response.AsJson(projectRepository.ToList());

            Get["/{id:guid}"] = parameters => Response.AsJson(projectRepository.GetById((Guid)parameters.id));

            Get["/getactive"] = parameters => Response.AsJson(projectRepository.GetActive());
        }
    }
}