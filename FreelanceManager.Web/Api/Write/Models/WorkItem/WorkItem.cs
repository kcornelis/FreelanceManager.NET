namespace FreelanceManager.Web.Api.Write.Models.WorkItem
{
    public class WorkItem
    {
        public WorkItem() { }
        public WorkItem(Domain.WorkItem workItem)
        {
            Populate(workItem);
        }

        public string Description { get; set; }
        public Date DueDate { get; set; }
        public Time DueTime { get; set; }

        public void Populate(Domain.WorkItem workItem)
        {
            Description = workItem.Description;
            DueDate = workItem.DueDate;
            DueTime = workItem.DueTime;
        }
    }
}