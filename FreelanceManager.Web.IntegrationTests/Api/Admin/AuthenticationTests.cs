using System;
using Nancy;
using Xunit;

namespace FreelanceManager.Web.Api.Admin
{
    public class AuthenticationTests
    {
        [Fact]
        public void Write_AccountCreate_Needs_The_Admin_Claim()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var code = browser.Post("/write/admin/account/create")
                              .StatusCode;

            Assert.Equal(HttpStatusCode.Forbidden, code);
        }

        [Fact]
        public void Write_AccountUpdate_Needs_The_Admin_Claim()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var code = browser.Post("/write/admin/account/update/" + Guid.NewGuid())
                              .StatusCode;

            Assert.Equal(HttpStatusCode.Forbidden, code);
        }

        [Fact]
        public void Write_AccountChangePassword_Needs_The_Admin_Claim()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var code = browser.Post("/write/admin/account/" + Guid.NewGuid() + "/newpassword")
                              .StatusCode;

            Assert.Equal(HttpStatusCode.Forbidden, code);
        }

        [Fact]
        public void Read_Accounts_Needs_The_Admin_Claim()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var code = browser.Get("/read/admin/accounts")
                              .StatusCode;

            Assert.Equal(HttpStatusCode.Forbidden, code);
        }

        [Fact]
        public void Read_Account_Needs_The_Admin_Claim()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var code = browser.Get("/read/admin/accounts/" + Guid.NewGuid())
                              .StatusCode;

            Assert.Equal(HttpStatusCode.Forbidden, code);
        }
    }
}
