using System;

namespace FreelanceManager.Events.Project
{
    public class ProjectDetailsChanged : Event
    {
        public ProjectDetailsChanged(Guid id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}
