using System;

namespace FreelanceManager.Events.Project
{
    public class ProjectCreated : Event
    {
        public ProjectCreated(Guid id, string name, string description, Guid clientId, DateTime createdOn)
        {
            Id = id;
            Name = name;
            Description = description;
            ClientId = clientId;
            CreatedOn = createdOn;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Guid ClientId { get; private set; }
        public DateTime CreatedOn { get; private set; }
    }
}
