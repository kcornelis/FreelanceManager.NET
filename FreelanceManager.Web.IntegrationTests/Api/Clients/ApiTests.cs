using System;
using System.Collections.Generic;
using System.Linq;
using Nancy.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace FreelanceManager.Web.Api.Clients
{
    public class ApiTests
    {
        [Fact]
        public void Create()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var response = CreateClient(browser);

            Assert.NotNull((string)response.Client.Id);
            Assert.Equal("johny bvba", (string)response.Client.Name);
            Assert.Equal(DateTime.Today, ((DateTime)response.Client.CreatedOn).Date);
        }

        [Fact]
        public void Read()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var id = (string)CreateClient(browser).Client.Id;
            var client = ReadClient(browser, id);

            Assert.Equal("johny bvba", (string)client.Name);
            Assert.Equal(id, (string)client.Id);
            Assert.NotNull((string)client.Tenant);
            Assert.Equal(DateTime.Today, ((DateTime)client.CreatedOn).Date);
        }

        [Fact]
        public void Read_All()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var id = (string)CreateClient(browser).Client.Id;
            IEnumerable<dynamic> accounts = Enumerable.Cast<dynamic>(ReadClients(browser));

            var account = accounts.Where(a => a.Id == id).FirstOrDefault();

            Assert.Equal("johny bvba", (string)account.Name);
        }

        [Fact]
        public void Update()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var id = (string)CreateClient(browser).Client.Id;
            var response = UpdateClient(browser, id);

            Assert.Equal(id, (string)response.Client.Id);
            Assert.Equal("jane bvba", (string)response.Client.Name);
        }

        private dynamic CreateClient(Browser browser)
        {
            return JObject.Parse(browser.Post("/write/client/create", c =>
            {
                c.JsonBody(new
                {
                    Name = "johny bvba"
                });
            }).Body.AsString());
        }

        private dynamic ReadClient(Browser browser, string id)
        {
            return JObject.Parse(browser.Get("/read/clients/" + id).Body.AsString());
        }

        private dynamic ReadClients(Browser browser)
        {
            return JArray.Parse(browser.Get("/read/clients").Body.AsString());
        }

        private dynamic UpdateClient(Browser browser, string id)
        {
            return JObject.Parse(browser.Post("/write/client/update/" + id, c =>
            {
                c.JsonBody(new
                {
                    Name = "jane bvba"
                });
            }).Body.AsString());
        }
    }
}
