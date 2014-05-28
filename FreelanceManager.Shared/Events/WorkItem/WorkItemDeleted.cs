using System;

namespace FreelanceManager.Events.WorkItem
{
    public class WorkItemDeleted : Event
    {
        public WorkItemDeleted(Guid id, DateTime deletedOn)
        {
            Id = id;
            DeletedOn = deletedOn;
        }

        public DateTime DeletedOn { get; private set; }
    }
}
