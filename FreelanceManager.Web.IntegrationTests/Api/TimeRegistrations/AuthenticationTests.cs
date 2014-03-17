using System;
using Nancy;
using Xunit;

namespace FreelanceManager.Web.Api.TimeRegistrations
{
    public class AuthenticationTests
    {
        [Fact]
        public void Write_TimeRegistrationCreate_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Post("/write/timeregistration/create");

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Write_TimeRegistrationUpdate_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Post("/write/timeregistration/update/" + Guid.NewGuid());

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Write_TimeRegistrationDelete_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Post("/write/timeregistration/delete/" + Guid.NewGuid());

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Read_TimeRegistrations_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Get("/read/timeregistrations");

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Read_TimeRegistration_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Get("/read/timeregistrations/" + Guid.NewGuid());

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Read_TimeRegistrationGetForDate_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Get("/read/timeregistrations/getfordate/2012-01-20");

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Read_TimeRegistrationGetInfoForMonth_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Get("/read/timeregistrations/getinfoformonth/2012/01");

            Assert.True(response.RedirectedToLogin());
        }

        [Fact]
        public void Read_TimeRegistrationGetInfoPerTaskForMonth_Needs_Authentication()
        {
            var browser = Context.CreateBrowser();

            var response = browser.Get("/read/timeregistrations/getinfopertaskformonth/2012/01");

            Assert.True(response.RedirectedToLogin());
        }
    }
}
