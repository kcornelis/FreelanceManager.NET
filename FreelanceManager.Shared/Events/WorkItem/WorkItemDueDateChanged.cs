using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelanceManager.Events.WorkItem
{
    public class WorkItemDueDateChanged: Event
    {
        public WorkItemDueDateChanged(Guid id, Date dueDate, Time dueTime)
        {
            Id = id;
            DueDate = dueDate;
            DueTime = dueTime;
        }

        public Date DueDate { get; private set; }
        public Time DueTime { get; private set; }
    }
}
