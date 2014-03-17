using System;
using System.Collections.Generic;
using System.Linq;
using FreelanceManager.Web.Tools;
using Xunit;

namespace FreelanceManager.Web.Api.TimeRegistrations
{
    public class ApiTests
    {
        [Fact]
        public void Create()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var client = (string)browser.CreateClient().Client.Id;
            var project = (string)browser.CreateProject(client).Project.Id;
            var response = browser.CreateTimeRegistration(client, project);

            Assert.NotNull((string)response.TimeRegistration.Id);

            Assert.Equal(client, (string)response.TimeRegistration.ClientId);
            Assert.Equal("johny bvba", (string)response.TimeRegistration.ClientName);

            Assert.Equal(project, (string)response.TimeRegistration.ProjectId);
            Assert.Equal("windows 8 app", (string)response.TimeRegistration.ProjectName);
            Assert.Equal("Development", (string)response.TimeRegistration.Task);
            Assert.Equal(0, (decimal)response.TimeRegistration.Rate.Value);

            Assert.Equal("worked a bit", (string)response.TimeRegistration.Description);
            Assert.Equal("2014-02-20", (string)response.TimeRegistration.Date.Display);
            Assert.Equal("10:00", (string)response.TimeRegistration.From.Display);
            Assert.Equal("12:00", (string)response.TimeRegistration.To.Display);

            Assert.Null(response.TimeRegistration.CorrectedIncome.Value);
            Assert.Null(response.TimeRegistration.CorrectedIncomeMessage.Value);

            Assert.Equal(DateTime.Today, ((DateTime)response.TimeRegistration.CreatedOn).Date);
        }

        [Fact]
        public void Read()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var clientId = (string)browser.CreateClient().Client.Id;
            var projectId = (string)browser.CreateProject(clientId).Project.Id;
            var timeRegistrationId = (string)browser.CreateTimeRegistration(clientId, projectId).TimeRegistration.Id;
            var timeRegistration = browser.ReadTimeRegistration(timeRegistrationId);

            Assert.NotNull((string)timeRegistration.Id);

            Assert.Equal(clientId, (string)timeRegistration.ClientId);
            Assert.Equal("johny bvba", (string)timeRegistration.ClientName);

            Assert.Equal(projectId, (string)timeRegistration.ProjectId);
            Assert.Equal("windows 8 app", (string)timeRegistration.ProjectName);
            Assert.Equal("Development", (string)timeRegistration.Task);
            Assert.Equal(0, (decimal)timeRegistration.Rate.Value);

            Assert.Equal("worked a bit", (string)timeRegistration.Description);
            Assert.Equal("2014-02-20", (string)timeRegistration.Date.Display);
            Assert.Equal("10:00", (string)timeRegistration.From.Display);
            Assert.Equal("12:00", (string)timeRegistration.To.Display);

            Assert.Null(timeRegistration.CorrectedIncome.Value);
            Assert.Null(timeRegistration.CorrectedIncomeMessage.Value);

            Assert.Equal(DateTime.Today, ((DateTime)timeRegistration.CreatedOn).Date);
        }

        [Fact]
        public void Read_All()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var clientId = (string)browser.CreateClient().Client.Id;
            var projectId = (string)browser.CreateProject(clientId).Project.Id;
            var timeRegistrationId = (string)browser.CreateTimeRegistration(clientId, projectId).TimeRegistration.Id;

            IEnumerable<dynamic> timeRegistrations = Enumerable.Cast<dynamic>(browser.ReadTimeRegistrations());

            var timeRegistration = timeRegistrations.Where(a => a.Id == timeRegistrationId).FirstOrDefault();

            Assert.Equal("worked a bit", (string)timeRegistration.Description);
        }

        [Fact]
        public void Update()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var clientId = (string)browser.CreateClient().Client.Id;
            var projectId = (string)browser.CreateProject(clientId).Project.Id;
            var timeRegistrationId = (string)browser.CreateTimeRegistration(clientId, projectId).TimeRegistration.Id;

            var response = browser.UpdateTimeRegistration(timeRegistrationId, clientId, projectId);

            Assert.Equal(timeRegistrationId, (string)response.TimeRegistration.Id);

            Assert.Equal(clientId, (string)response.TimeRegistration.ClientId);
            Assert.Equal("johny bvba", (string)response.TimeRegistration.ClientName);

            Assert.Equal(projectId, (string)response.TimeRegistration.ProjectId);
            Assert.Equal("windows 8 app", (string)response.TimeRegistration.ProjectName);
            Assert.Equal("Meeting", (string)response.TimeRegistration.Task);
            Assert.Equal(0, (decimal)response.TimeRegistration.Rate.Value);

            Assert.Equal("worked some more", (string)response.TimeRegistration.Description);
            Assert.Equal("2014-02-20", (string)response.TimeRegistration.Date.Display);
            Assert.Equal("12:00", (string)response.TimeRegistration.From.Display);
            Assert.Equal("13:00", (string)response.TimeRegistration.To.Display);

            Assert.Equal(50, (decimal)response.TimeRegistration.CorrectedIncome.Value);
            Assert.Equal("override rate", (string)response.TimeRegistration.CorrectedIncomeMessage);

            Assert.Equal(DateTime.Today, ((DateTime)response.TimeRegistration.CreatedOn).Date);
        }

        [Fact]
        public void Delete()
        {
            var browser = Context.CreateBrowserAndAuthenticate();

            var clientId = (string)browser.CreateClient().Client.Id;
            var projectId = (string)browser.CreateProject(clientId).Project.Id;
            var timeRegistrationId = (string)browser.CreateTimeRegistration(clientId, projectId).TimeRegistration.Id;

            browser.DeleteTimeRegistration(timeRegistrationId);

            IEnumerable<dynamic> timeRegistrations = Enumerable.Cast<dynamic>(browser.ReadTimeRegistrations());

            Assert.Null(timeRegistrations.Where(a => a.Id == timeRegistrationId).FirstOrDefault());
        }

        // TODO GetForMonth
        // TODO GetFor...
    }
}
