namespace FreelanceManager.Web.Api.Write.Models.Projects
{
    public class Task
    {
        public Task() { }
        public Task(Domain.ValueObjects.Task task)
        {
            Populate(task);
        }

        public string Name { get; set; }
        public Money Rate { get; set; }

        public void Populate(Domain.ValueObjects.Task task)
        {
            Name = task.Name;
            Rate = task.Rate;
        }
    }
}