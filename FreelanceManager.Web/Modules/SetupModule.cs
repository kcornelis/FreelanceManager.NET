using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Web.Models.Setup;
using Nancy;
using Nancy.ModelBinding;

namespace FreelanceManager.Web.Modules
{
    public class SetupModule : NancyModule
    {
        public SetupModule(IAggregateRootRepository repository,
                           IAccountRepository accountRepository,
                           IIdGenerator idGenerator)
            :base("/setup")
        {
            Get["/"] = _ =>
            {
                if (accountRepository.Count() > 0)
                    return HttpStatusCode.NotFound;

                return View["Index"];
            };

            Post["/"] = _ =>
            {
                var model = this.Bind<CreateModel>();

                if (accountRepository.Count() > 0)
                    return HttpStatusCode.NotFound;

                var account = new Domain.Account(idGenerator.NextGuid(),
                    model.Name, model.FirstName, model.LastName, model.Email);

                account.ChangePassword(model.Password);
                account.MakeAdmin();

                repository.Save(account);

                return Response.AsRedirect("/");
            };
        }
    }
}