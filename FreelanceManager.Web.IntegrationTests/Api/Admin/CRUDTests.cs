using System;
using System.Globalization;
using Nancy.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace FreelanceManager.Web.Api.Admin
{
    public class CRUDTests
    {
        [Fact]
        public void Write_AccountCreate()
        {
            var browser = Context.CreateBrowserAndAuthenticateWithAdmin();

            var response = browser.Post("/write/admin/account/create", c =>
            {
                c.JsonBody(new
                {
                    Name = "johny bvba",
                    FirstName = "johny",
                    LastName = "turbo",
                    Email = "johny@turbo.com"
                });
            });

            dynamic result = JObject.Parse(response.Body.AsString());

            Assert.NotNull((string)result.Account.Id);
            Assert.Equal("johny bvba", (string)result.Account.Name);
            Assert.Equal("johny", (string)result.Account.FirstName);
            Assert.Equal("turbo", (string)result.Account.LastName);
            Assert.Equal("johny@turbo.com", (string)result.Account.Email);
            Assert.NotNull((string)result.Password);
        }

        [Fact]
        public void Read_Account()
        {
            var browser = Context.CreateBrowserAndAuthenticateWithAdmin();

            var id = (string)((dynamic)JObject.Parse(browser.Post("/write/admin/account/create", c =>
            {
                c.JsonBody(new
                {
                    Name = "johny bvba",
                    FirstName = "johny",
                    LastName = "turbo",
                    Email = "johny@turbo.com"
                });
            }).Body.AsString())).Account.Id;

            dynamic result = JObject.Parse(browser.Get("/read/admin/accounts/" + id).Body.AsString());

            Assert.Equal(false, (bool)result.Admin);
            Assert.Equal("johny bvba", (string)result.Name);
            Assert.Equal("johny", (string)result.FirstName);
            Assert.Equal("turbo", (string)result.LastName);
            Assert.Equal("johny turbo", (string)result.FullName);
            Assert.Equal("johny@turbo.com", (string)result.Email);
            Assert.Equal(id, (string)result.Id);
            Assert.Null((string)result.Tenant);
            Assert.Equal(DateTime.Today, DateTime.Parse((string)result.CreatedOn, CultureInfo.InvariantCulture).Date);
        }

        // TODO update account
        // TODO change password
    }
}
