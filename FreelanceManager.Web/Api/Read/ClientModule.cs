using System;
using System.Linq;
using FreelanceManager.ReadModel.Repositories;
using Nancy;
using Nancy.Security;

namespace FreelanceManager.Web.Api.Read
{
    public class ClientModule : ApiModule
    {
        public ClientModule(IClientRepository clientRepository)
            :base("/read/clients")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => Json(clientRepository.ToList());

            Get["/{id:guid}"] = parameters => Json(clientRepository.GetById((Guid)parameters.id));
        }
    }
}