using System;
using System.Linq;
using FreelanceManager.ReadModel.Repositories;
using Nancy;
using Nancy.Security;

namespace FreelanceManager.Web.Api.Read
{
    public class AdminModule : ApiModule
    {
        public AdminModule(IAccountRepository accountRepository)
            : base("/read/admin")
        {
            this.RequiresAuthentication();
            this.RequiresClaims(new[] { "Admin" });

            Get["/accounts"] = _ => Json(accountRepository.ToList());

            Get["/accounts/{id:guid}"] = parameters => Json(accountRepository.GetById((Guid)parameters.id));
        }
    }
}