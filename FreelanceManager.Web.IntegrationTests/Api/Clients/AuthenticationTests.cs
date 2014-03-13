using System;
using Nancy;
using Xunit;

namespace FreelanceManager.Web.Api.Clients
{
    public class AuthenticationTests
    {
        [Fact]
        public void Write_ClientCreate_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Post("/write/client/create");

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Write_ClientUpdate_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Post("/write/client/update/" + Guid.NewGuid());

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Read_Clients_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Get("/read/clients");

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Read_Client_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Get("/read/clients/" + Guid.NewGuid());

            Assert.True(response.RedirectedToLogin());
        }
    }
}
