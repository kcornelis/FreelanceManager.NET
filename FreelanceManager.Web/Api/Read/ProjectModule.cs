using System;
using System.Linq;
using FreelanceManager.ReadModel.Repositories;
using Nancy;
using Nancy.Security;

namespace FreelanceManager.Web.Api.Read
{
    public class ProjectModule : ApiModule
    {
        public ProjectModule(IProjectRepository projectRepository)
            :base("/read/projects")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => Json(projectRepository.ToList());

            Get["/{id:guid}"] = parameters => Json(projectRepository.GetById((Guid)parameters.id));

            Get["/getactive"] = parameters => Json(projectRepository.GetActive());
        }
    }
}