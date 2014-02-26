using System;
namespace FreelanceManager.Events.Client
{
    public class ClientDetailsChanged : Event
    {
        public ClientDetailsChanged(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Name { get; private set; }
    }
}
