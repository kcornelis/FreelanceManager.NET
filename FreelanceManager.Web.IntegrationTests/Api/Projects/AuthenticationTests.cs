using System;
using Nancy;
using Xunit;

namespace FreelanceManager.Web.Api.Projects
{
    public class AuthenticationTests
    {
        [Fact]
        public void Write_ProjectCreate_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Post("/write/project/create");

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Write_ProjectUpdate_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Post("/write/project/update/" + Guid.NewGuid());

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Write_ProjectUpdateTasks_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Post("/write/project/updatetasks/" + Guid.NewGuid());

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Read_Projects_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Get("/read/projects");

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Read_ActiveProjects_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Get("/read/projects/getactive");

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Read_Project_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Get("/read/projects/" + Guid.NewGuid());

            Assert.True(response.RedirectedToLogin());
        }
    }
}
