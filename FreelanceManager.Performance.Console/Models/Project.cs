using System;
using Fluency;
using Fluency.DataGeneration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FreelanceManager.Performance.Console.Models
{
    public class Project
    {
        public Project(Guid clientId)
        {
            ClientId = clientId;
            Name = ARandom.String(20);
            Description = ARandom.String(100);
        }

        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public static class ProjectExt
    {
        public static Project CreateProject(this CustomWebClient browser, Guid clientId)
        {
            var project = new Project(clientId);

            var r = browser.Post("/write/project/create", JsonConvert.SerializeObject(project));

            var response = (dynamic)JObject.Parse(r);

            project.Id = response.Project.Id;

            return project;
        }
    }
}
