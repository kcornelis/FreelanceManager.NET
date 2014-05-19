using FreelanceManager.Events.Client;
using FreelanceManager.ReadModel.Repositories;

namespace FreelanceManager.ReadModel.EventHandlers
{
    public class ClientHandlers : IHandleEvent<ClientCreated>,
                                  IHandleEvent<ClientDetailsChanged>
    {
        private readonly IClientRepository _clientRepository;

        public ClientHandlers(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public void Handle(ClientCreated @event)
        {
            var client = new Client
            {
                Id = @event.Id,
                CreatedOn = @event.CreatedOn,
                Name = @event.Name
            };

            _clientRepository.Add(client, @event.Version);
        }

        public void Handle(ClientDetailsChanged @event)
        {
            var client = _clientRepository.GetById(@event.Id);

            if (client != null)
            {
                client.Name = @event.Name;

                _clientRepository.Update(client, @event.Version);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }
    }
}
