using System;
using System.Linq;
using FreelanceManager.ReadModel.Repositories;
using Nancy;
using Nancy.Security;

namespace FreelanceManager.Web.Areas.Read
{
    public class AdminModule : NancyModule
    {
        public AdminModule(IAccountRepository accountRepository)
            : base("/read/admin")
        {
            this.RequiresAuthentication();
            this.RequiresClaims(new[] { "Admin" });

            Get["/accounts"] = _ => Response.AsJson(accountRepository.ToList());

            Get["/accounts/{id:guid}"] = parameters => Response.AsJson(accountRepository.GetById((Guid)parameters.id));
        }
    }
}