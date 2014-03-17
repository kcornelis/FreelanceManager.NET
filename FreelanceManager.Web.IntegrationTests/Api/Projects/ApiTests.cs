using System;
using System.Collections.Generic;
using System.Linq;
using FreelanceManager.Web.Tools;
using Xunit;

namespace FreelanceManager.Web.Api.Projects
{
    public class ApiTests
    {
        [Fact]
        public void Create()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var client = (string)browser.CreateClient().Client.Id;
            var response = browser.CreateProject(client);

            Assert.NotNull((string)response.Project.Id);
            Assert.Equal("windows 8 app", (string)response.Project.Name);
            Assert.Equal("a windows 8 app", (string)response.Project.Description);
            Assert.Equal(client, (string)response.Project.ClientId);
            Assert.Equal("johny bvba", (string)response.Project.ClientName);
            Assert.Equal(false, (bool)response.Project.Hidden);
            Assert.Equal(DateTime.Today, ((DateTime)response.Project.CreatedOn).Date);

            Assert.Equal("Development", (string)response.Project.Tasks[0].Name);
            Assert.Equal(0, (decimal)response.Project.Tasks[0].Rate.Value);

            Assert.Equal("Analyse", (string)response.Project.Tasks[1].Name);
            Assert.Equal(0, (decimal)response.Project.Tasks[1].Rate.Value);

            Assert.Equal("Meeting", (string)response.Project.Tasks[2].Name);
            Assert.Equal(0, (decimal)response.Project.Tasks[2].Rate.Value);
        }

        [Fact]
        public void Read()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var clientId = (string)browser.CreateClient().Client.Id;
            var projectId = (string)browser.CreateProject(clientId).Project.Id;
            var project = browser.ReadProject(projectId);

            Assert.NotNull((string)project.Id);
            Assert.Equal("windows 8 app", (string)project.Name);
            Assert.Equal("a windows 8 app", (string)project.Description);
            Assert.Equal(clientId, (string)project.ClientId);
            Assert.Equal("johny bvba", (string)project.ClientName);
            Assert.Equal(false, (bool)project.Hidden);
            Assert.Equal(DateTime.Today, ((DateTime)project.CreatedOn).Date);

            Assert.Equal("Development", (string)project.Tasks[0].Name);
            Assert.Equal(0, (decimal)project.Tasks[0].Rate.Value);

            Assert.Equal("Analyse", (string)project.Tasks[1].Name);
            Assert.Equal(0, (decimal)project.Tasks[1].Rate.Value);

            Assert.Equal("Meeting", (string)project.Tasks[2].Name);
            Assert.Equal(0, (decimal)project.Tasks[2].Rate.Value);
        }

        [Fact]
        public void Read_All()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var clientId = (string)browser.CreateClient().Client.Id;
            var projectId = (string)browser.CreateProject(clientId).Project.Id;
            IEnumerable<dynamic> projects = Enumerable.Cast<dynamic>(browser.ReadProjects());

            var project = projects.Where(a => a.Id == projectId).FirstOrDefault();

            Assert.Equal("windows 8 app", (string)project.Name);
        }

        [Fact]
        public void Update()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var clientId = (string)browser.CreateClient().Client.Id;
            var projectId = (string)browser.CreateProject(clientId).Project.Id;
            var response = browser.UpdateProject(projectId);

            Assert.Equal(projectId, (string)response.Project.Id);
            Assert.Equal("win8app", (string)response.Project.Name);
            Assert.Equal("-", (string)response.Project.Description);
            Assert.Equal(true, (bool)response.Project.Hidden);
        }

        [Fact]
        public void UpdateTasks()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var clientId = (string)browser.CreateClient().Client.Id;
            var projectId = (string)browser.CreateProject(clientId).Project.Id;
            var response = browser.UpdateProjectTasks(projectId);

            Assert.Equal("Development", (string)response.Project.Tasks[0].Name);
            Assert.Equal(20, (decimal)response.Project.Tasks[0].Rate.Value);

            Assert.Equal("Blabla", (string)response.Project.Tasks[1].Name);
            Assert.Equal(10, (decimal)response.Project.Tasks[1].Rate.Value);

            Assert.Equal(2, (int)response.Project.Tasks.Count);
        }
    }
}
