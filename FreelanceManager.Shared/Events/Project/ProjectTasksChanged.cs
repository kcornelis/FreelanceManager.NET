using System;
using FreelanceManager.Dtos;

namespace FreelanceManager.Events.Project
{
    public class ProjectTasksChanged : Event
    {
        public ProjectTasksChanged(Guid id, Task[] tasks)
        {
            Id = id;
            Tasks = tasks;
        }

        public Task[] Tasks
        {
            get;
            private set;
        }
    }
}
