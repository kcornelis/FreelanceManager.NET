using System;

namespace FreelanceManager.Events.WorkItem
{
    public class WorkItemCreated : Event
    {
        public WorkItemCreated(Guid id, string description, Date dueDate, Time dueTime, DateTime createdOn)
        {
            Id = id;
            Description = description;
            DueDate = dueDate;
            DueTime = dueTime;
            CreatedOn = createdOn;
        }

        public string Description { get; private set; }
        public Date DueDate  { get; private set; }
        public Time DueTime { get; private set; }
        public DateTime CreatedOn { get; private set; }
    }
}
