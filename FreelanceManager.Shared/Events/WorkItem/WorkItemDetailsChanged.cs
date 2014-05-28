using System;

namespace FreelanceManager.Events.WorkItem
{
    public class WorkItemDetailsChanged : Event
    {
        public WorkItemDetailsChanged(Guid id, string description)
        {
            Id = id;
            Description = description;
        }

        public string Description { get; private set; }
    }
}
