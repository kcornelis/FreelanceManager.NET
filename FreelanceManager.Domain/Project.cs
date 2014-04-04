using System;
using System.Collections.Generic;
using System.Linq;
using FreelanceManager.Domain.ValueObjects;
using FreelanceManager.Events.Project;

namespace FreelanceManager.Domain
{
    public class Project : AggregateRoot
    {
        private List<Task> _tasks = new List<Task>();

        public Project() { }
        public Project(Guid id, string name, string description, Client client)
        {
            ApplyChange(new ProjectCreated(id, name, description, client.Id, DateTime.UtcNow));
            ApplyChange(new ProjectTasksChanged(id, new[]{
                new Dtos.Task{ Name = "Development", Rate = Money.Zero },
                new Dtos.Task{ Name = "Analyse", Rate = Money.Zero },
                new Dtos.Task{ Name = "Meeting", Rate = Money.Zero }
            }));
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Guid ClientId { get; private set; }
        public bool Hidden { get; private set; }
        public IEnumerable<Task> Tasks { get { return _tasks; } private set { _tasks = value.ToList(); } }

        public Task FindTask(string taskName)
        {
            return Tasks.FirstOrDefault(t => t.Name.Equals(taskName, StringComparison.OrdinalIgnoreCase));
        }

        public void ChangeDetails(string name, string description)
        {
            if (Name != name || Description != description)
            {
                ApplyChange(new ProjectDetailsChanged(Id, name, description));
            }
        }

        public void Hide()
        {
            if(!Hidden)
            {
                ApplyChange(new ProjectHidden(Id));
            }
        }

        public void Unhide()
        {
            if (Hidden)
            {
                ApplyChange(new ProjectUnhidden(Id));
            }
        }

        public void ChangeTasks(Task[] tasks)
        {
            bool changed = false;
            if (tasks.Length != Tasks.Count())
            {
                changed = true;
            }
            else
            {
                foreach (var pair in Tasks.Zip(tasks, (t1, t2) => Tuple.Create(t1, t2)))
                {
                    if (pair.Item1.Name != pair.Item2.Name ||
                        pair.Item1.Rate != pair.Item2.Rate)
                    {
                        changed = true;
                        break;
                    }
                }
            }

            if (changed)
            {
                ApplyChange(new ProjectTasksChanged(Id, tasks.Select(t => new Dtos.Task { Name = t.Name, Rate = t.Rate }).ToArray()));
            }
        }

        public void Apply(ProjectCreated e)
        {
            Id = e.Id;
            Name = e.Name;
            Description = e.Description;
            ClientId = e.ClientId;
            CreatedOn = e.CreatedOn;
        }

        public void Apply(ProjectDetailsChanged e)
        {
            Name = e.Name;
            Description = e.Description;
        }

        public void Apply(ProjectTasksChanged e)
        {
            var list = e.Tasks ?? Enumerable.Empty<Dtos.Task>();
            Tasks = list.Select(t => new Task { Name = t.Name, Rate = t.Rate });
        }

        public void Apply(ProjectHidden e)
        {
            Hidden = true;
        }

        public void Apply(ProjectUnhidden e)
        {
            Hidden = false;
        }
    }
}