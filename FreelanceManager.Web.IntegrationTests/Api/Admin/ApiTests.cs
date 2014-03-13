using System;
using System.Collections.Generic;
using System.Linq;
using Nancy.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace FreelanceManager.Web.Api.Admin
{
    public class ApiTests
    {
        [Fact]
        public void Create()
        {
            var browser = Context.CreateBrowserAndAuthenticateWithAdmin();

            var account = CreateAccount(browser);

            Assert.NotNull((string)account.Account.Id);
            Assert.Equal("johny bvba", (string)account.Account.Name);
            Assert.Equal("johny", (string)account.Account.FirstName);
            Assert.Equal("turbo", (string)account.Account.LastName);
            Assert.Equal("johny@turbo.com", (string)account.Account.Email);
            Assert.NotNull((string)account.Password);
            Assert.Equal(DateTime.Today, ((DateTime)account.Account.CreatedOn).Date);
        }

        [Fact]
        public void Read()
        {
            var browser = Context.CreateBrowserAndAuthenticateWithAdmin();

            var id = (string)CreateAccount(browser).Account.Id;
            var account = ReadAccount(browser, id);

            Assert.Equal(false, (bool)account.Admin);
            Assert.Equal("johny bvba", (string)account.Name);
            Assert.Equal("johny", (string)account.FirstName);
            Assert.Equal("turbo", (string)account.LastName);
            Assert.Equal("johny turbo", (string)account.FullName);
            Assert.Equal("johny@turbo.com", (string)account.Email);
            Assert.Equal(id, (string)account.Id);
            Assert.Null((string)account.Tenant);
            Assert.Equal(DateTime.Today, ((DateTime)account.CreatedOn).Date);
        }

        [Fact]
        public void Read_All()
        {
            var browser = Context.CreateBrowserAndAuthenticateWithAdmin();

            var id = (string)CreateAccount(browser).Account.Id;
            IEnumerable<dynamic> accounts = Enumerable.Cast<dynamic>(ReadAccounts(browser));

            var account = accounts.Where(a => a.Id == id).FirstOrDefault();

            Assert.Equal("johny bvba", (string)account.Name);
            Assert.Equal("johny", (string)account.FirstName);
            Assert.Equal("turbo", (string)account.LastName);
            Assert.Equal("johny turbo", (string)account.FullName);
            Assert.Equal("johny@turbo.com", (string)account.Email);
        }

        [Fact]
        public void Update()
        {
            var browser = Context.CreateBrowserAndAuthenticateWithAdmin();

            var id = (string)CreateAccount(browser).Account.Id;
            var account = UpdateAccount(browser, id);

            Assert.Equal(id, (string)account.Account.Id);
            Assert.Equal("jane bvba", (string)account.Account.Name);
            Assert.Equal("jane", (string)account.Account.FirstName);
            Assert.Equal("doe", (string)account.Account.LastName);
            Assert.Equal("jane@turbo.com", (string)account.Account.Email);
        }

        [Fact]
        public void ChangePassword()
        {
            var browser = Context.CreateBrowserAndAuthenticateWithAdmin();

            var response = CreateAccount(browser);
            ChangeAccountPassword(browser, (string)response.Account.Id);

            // TODO verify the result
            //var test = Context.CreateBrowser().Post("/account/login", c =>
            //{
            //    c.FormValue("email", "johny@turbo.com");
            //    c.FormValue("password", (string)response.Password);
            //});
        }

        private dynamic CreateAccount(Browser browser)
        {
            return JObject.Parse(browser.Post("/write/admin/account/create", c =>
            {
                c.JsonBody(new
                {
                    Name = "johny bvba",
                    FirstName = "johny",
                    LastName = "turbo",
                    Email = "johny@turbo.com"
                });
            }).Body.AsString());
        }

        private dynamic ReadAccount(Browser browser, string id)
        {
            return JObject.Parse(browser.Get("/read/admin/accounts/" + id).Body.AsString());
        }

        private dynamic ReadAccounts(Browser browser)
        {
            return JArray.Parse(browser.Get("/read/admin/accounts/").Body.AsString());
        }

        private dynamic UpdateAccount(Browser browser, string id)
        {
            return JObject.Parse(browser.Post("/write/admin/account/update/" + id, c =>
            {
                c.JsonBody(new
                {
                    Name = "jane bvba",
                    FirstName = "jane",
                    LastName = "doe",
                    Email = "jane@turbo.com"
                });
            }).Body.AsString());
        }

        private void ChangeAccountPassword(Browser browser, string id)
        {
            browser.Post("/write/admin/account/" + id + "/changepassword", c =>
            {
                c.JsonBody(new
                {
                    Password = "blabla"
                });
            });
        }
    }
}
