using System;
using FreelanceManager.Web.Api.Write.Models.Clients;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace FreelanceManager.Web.Api.Write
{
    public class ClientModule : ApiModule
    {
        public ClientModule(IAggregateRootRepository repository,
                            IIdGenerator idGenerator)
            :base("/write/client")
        {
            this.RequiresAuthentication();

            Post["/create"] = parameters =>
            {
                var model = this.Bind<EditableClient>();

                var client = new Domain.Client(idGenerator.NextGuid(), model.Name);

                repository.Save(client);

                return Json(new
                {
                    Client = new Client(client)
                });
            };

            Post["/update/{id:guid}"] = parameters =>
            {
                var model = this.Bind<EditableClient>();

                var client = repository.GetById<Domain.Client>((Guid)parameters.id);

                if (client != null)
                {
                    client.ChangeDetails(model.Name);

                    repository.Save(client);

                    return Json(new
                    {
                        Client = new Client(client)
                    });
                }

                return null;
            };
        }
    }
}