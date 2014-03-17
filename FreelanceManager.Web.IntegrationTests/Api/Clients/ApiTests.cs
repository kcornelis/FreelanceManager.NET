using System;
using System.Collections.Generic;
using System.Linq;
using FreelanceManager.Web.Tools;
using Xunit;

namespace FreelanceManager.Web.Api.Clients
{
    public class ApiTests
    {
        [Fact]
        public void Create()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var response = browser.CreateClient();

            Assert.NotNull((string)response.Client.Id);
            Assert.Equal("johny bvba", (string)response.Client.Name);
            Assert.Equal(DateTime.Today, ((DateTime)response.Client.CreatedOn).Date);
        }

        [Fact]
        public void Read()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var id = (string)browser.CreateClient().Client.Id;
            var client = browser.ReadClient(id);

            Assert.Equal("johny bvba", (string)client.Name);
            Assert.Equal(id, (string)client.Id);
            Assert.NotNull((string)client.Tenant);
            Assert.Equal(DateTime.Today, ((DateTime)client.CreatedOn).Date);
        }

        [Fact]
        public void Read_All()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var id = (string)browser.CreateClient().Client.Id;
            IEnumerable<dynamic> accounts = Enumerable.Cast<dynamic>(browser.ReadClients());

            var account = accounts.Where(a => a.Id == id).FirstOrDefault();

            Assert.Equal("johny bvba", (string)account.Name);
        }

        [Fact]
        public void Update()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var id = (string)browser.CreateClient().Client.Id;
            var response = browser.UpdateClient(id);

            Assert.Equal(id, (string)response.Client.Id);
            Assert.Equal("jane bvba", (string)response.Client.Name);
        }
    }
}
