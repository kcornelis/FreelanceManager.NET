using System;
using System.Collections.Generic;
using System.Linq;

namespace FreelanceManager.Web.Api.Write.Models.Projects
{
    public class Project
    {        
        public Project() { }

        public Project(Domain.Client client, Domain.Project project)
        {
            Populate(client, project);
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public bool Hidden { get; set; }
        public DateTime CreatedOn { get; set; }
        public IEnumerable<Task> Tasks { get; private set; }

        public void Populate(Domain.Client client, Domain.Project project)
        {
            Id = project.Id;
            Name = project.Name;
            ClientId = client.Id;
            ClientName = client.Name;
            Description = project.Description;
            Hidden = project.Hidden;
            CreatedOn = project.CreatedOn;
            Tasks = project.Tasks.Select(p => new Task(p)).ToArray();
        }
    }
}