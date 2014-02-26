using System;
using FreelanceManager.Events.Client;

namespace FreelanceManager.Domain
{
    public class Client : AggregateRoot
    {
        public Client() { }
        public Client(Guid id, string name)
        {
            ApplyChange(new ClientCreated(id, name, DateTime.Now));
        }

        public string Name { get; private set; }

        public void ChangeDetails(string name)
        {
            if (Name != name)
            {
                ApplyChange(new ClientDetailsChanged(Id, name));
            }
        }

        public void Apply(ClientCreated e)
        {
            Id = e.Id;
            Name = e.Name;
            CreatedOn = e.CreatedOn;
        }

        public void Apply(ClientDetailsChanged e)
        {
            Name = e.Name;
        }
    }
}