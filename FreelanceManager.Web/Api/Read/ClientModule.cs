using System;
using System.Linq;
using FreelanceManager.ReadModel.Repositories;
using Nancy;
using Nancy.Security;

namespace FreelanceManager.Web.Areas.Read
{
    public class ClientModule : NancyModule
    {
        public ClientModule(IClientRepository clientRepository)
            :base("/read/clients")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => Response.AsJson(clientRepository.ToList());

            Get["/{id:guid}"] = parameters => Response.AsJson(clientRepository.GetById((Guid)parameters.id));
        }
    }
}