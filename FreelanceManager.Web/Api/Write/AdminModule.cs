using System;
using FreelanceManager.Web.Api.Write.Models.Admin;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace FreelanceManager.Web.Api.Write
{
    public class AdminModule : ApiModule
    {
        public AdminModule(IAggregateRootRepository repository,
                           IIdGenerator idGenerator)
            : base("/write/admin")
        {
            this.RequiresAuthentication();
            this.RequiresClaims(new[] { "Admin" });

            Post["/account/create"] = parameters =>
            {
                var model = this.Bind<EditableAccount>();
                var account = new Domain.Account(idGenerator.NextGuid(),
                    model.Name, model.FirstName, model.LastName, model.Email);

                var password = account.GeneratePassword();

                repository.Save(account);

                return Json(new
                {
                    Account = new Account(account),
                    Password = password
                });
            };

            Post["/account/update/{id:guid}"] = parameters =>
            {
                var model = this.Bind<EditableAccount>();
                var account = repository.GetById<Domain.Account>((Guid)parameters.id);

                if (account != null)
                {
                    account.ChangeDetails(model.Name, model.FirstName, model.LastName, model.Email);

                    repository.Save(account);

                    return Json(new
                    {
                        Account = new Account(account)
                    });
                }

                return null;
            };

            Post["/account/{id:guid}/newpassword"] = parameters =>
            {
                var model = this.Bind<AccountNewPassword>();
                var account = repository.GetById<Domain.Account>((Guid)parameters.id);

                if (account != null)
                {
                    account.ChangePassword(model.Password);
                    repository.Save(account);
                }

                return null;
            };
        }
    }
}