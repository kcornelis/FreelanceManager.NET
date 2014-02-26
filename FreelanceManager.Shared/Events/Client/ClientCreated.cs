using System;
namespace FreelanceManager.Events.Client
{
    public class ClientCreated : Event
    {
        public ClientCreated(Guid id, string name, DateTime createdOn)
        {
            Id = id;
            Name = name;
            CreatedOn = createdOn;
        }

        public string Name { get; private set; }
        public DateTime CreatedOn { get; private set; }
    }
}
