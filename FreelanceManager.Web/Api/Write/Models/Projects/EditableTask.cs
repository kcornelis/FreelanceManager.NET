namespace FreelanceManager.Web.Api.Write.Models.Projects
{
    public class EditableTask
    {
        public EditableTask() { }
        public EditableTask(Domain.ValueObjects.Task task)
        {
            Populate(task);
        }

        public string Name { get; set; }
        public decimal Rate { get; set; }

        public void Populate(Domain.ValueObjects.Task task)
        {
            Name = task.Name;
            Rate = task.Rate;
        }
    }
}