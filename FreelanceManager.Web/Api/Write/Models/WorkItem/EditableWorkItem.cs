namespace FreelanceManager.Web.Api.Write.Models.WorkItem
{
    public class EditableWorkItem
    {
        public EditableWorkItem() { }
        public EditableWorkItem(Domain.WorkItem workItem)
        {
            Populate(workItem);
        }

        public string Description { get; set; }
        public string DueDate { get; set; }
        public string DueTime { get; set; }

        public void Populate(Domain.WorkItem workItem)
        {
            Description = workItem.Description;
            DueDate = (workItem.DueDate != null) ? workItem.DueDate.ToString() : "";
            DueTime = (workItem.DueTime != null) ? workItem.DueTime.ToString() : "";
        }
    }
}