using System;
using System.Collections.Generic;
using System.Linq;
using FreelanceManager.Web.Tools;
using Xunit;

namespace FreelanceManager.Web.Api.Admin
{
    public class ApiTests
    {
        [Fact]
        public void Create()
        {
            var browser = Context.CreateBrowserAndAuthenticateWithAdmin();

            var account = browser.CreateAccount();

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

            var id = (string)browser.CreateAccount().Account.Id;
            var account = browser.ReadAccount(id);

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

            var id = (string)browser.CreateAccount().Account.Id;
            IEnumerable<dynamic> accounts = Enumerable.Cast<dynamic>(browser.ReadAccounts());

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

            var id = (string)browser.CreateAccount().Account.Id;
            var account = browser.UpdateAccount(id);

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

            var response = browser.CreateAccount();
            browser.ChangeAccountPassword((string)response.Account.Id);

            // TODO verify the result
            //var test = Context.CreateBrowser().Post("/account/login", c =>
            //{
            //    c.FormValue("email", "johny@turbo.com");
            //    c.FormValue("password", (string)response.Password);
            //});
        }
    }
}
