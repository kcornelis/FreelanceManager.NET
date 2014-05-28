using System;

namespace FreelanceManager.ReadModel
{
    public class WorkItem : Model
    {
        public string Description { get; set; }
        public Date DueDate { get; set; }
        public Time DueTime { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
