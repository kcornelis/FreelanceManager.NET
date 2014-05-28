using System;
using FreelanceManager.Events.WorkItem;

namespace FreelanceManager.Domain
{
    public class WorkItem : AggregateRoot
    {        
        public WorkItem() { }

        public WorkItem(Guid id, string description, Date dueDate, Time dueTime)
        {
            ApplyChange(new WorkItemCreated(id, description, dueDate, dueTime, DateTime.UtcNow));
        }

        public string Description { get; private set; }
        public Date DueDate { get; private set; }
        public Time DueTime { get; private set; }
        public bool Deleted { get; private set; }
        public DateTime? DeletedOn { get; private set; }

        public void ChangeDueDate(Date dueDate, Time dueTime)
        {
            if (Deleted)
                throw new AggregateDeletedException();

            if (DueDate != dueDate || DueTime != dueTime)
            {
                ApplyChange(new WorkItemDueDateChanged(Id, dueDate, dueTime));
            }
        }

        public void ChangeDetails(string description)
        {
            if (Deleted)
                throw new AggregateDeletedException();

            if (Description != description)
            {
                ApplyChange(new WorkItemDetailsChanged(Id, description));
            }
        }

        public void Delete()
        {
            if (Deleted)
                throw new AggregateDeletedException();

            ApplyChange(new WorkItemDeleted(Id, DateTime.UtcNow));
        }

        public void Apply(WorkItemCreated e)
        {
            Id = e.Id;
            Description = e.Description;
            DueDate = e.DueDate;
            DueTime = e.DueTime;
            CreatedOn = e.CreatedOn;
        }

        public void Apply(WorkItemDetailsChanged e)
        {
            Description = e.Description;
        }

        public void Apply(WorkItemDueDateChanged e)
        {
            DueDate = e.DueDate;
            DueTime = e.DueTime;
        }

        public void Apply(WorkItemDeleted e)
        {
            Deleted = true;
            DeletedOn = e.DeletedOn;
        }
    }
}
