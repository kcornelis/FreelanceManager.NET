using System;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using FreelanceManager.Events.Client;
using FreelanceManager.Events.Project;
using FreelanceManager.Events.TimeRegistration;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.TimeRegistrationPeriodInfoPerTaskTests
{
    public class when_a_timeregistration_is_created : Specification
    {
        private Guid _projectId = Guid.NewGuid();
        private Guid _clientId = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private ClientHandlers _clientHandler;
        private ProjectHandlers _projectHandler;
        private TimeRegistrationPeriodInfoPerTaskHandlers _timeregistrationPeriodInfoHandler;
        private ITimeRegistrationPeriodInfoPerTaskRepository _timeregistrationPeriodInfoRepository;
        private ITenantContext _tenantContext;

        private TimeRegistrationPeriodInfoPerTask _timeregistrationPeriodInfoJanDev;
        private TimeRegistrationPeriodInfoPerTask _timeregistrationPeriodInfoJanMeeting;
        private TimeRegistrationPeriodInfoPerTask _timeregistrationPeriodInfoFebDev;

        protected override void Context()
        {
            _clientHandler = Resolve<ClientHandlers>();
            _projectHandler = Resolve<ProjectHandlers>();
            _timeregistrationPeriodInfoHandler = Resolve<TimeRegistrationPeriodInfoPerTaskHandlers>();
            _timeregistrationPeriodInfoRepository = Resolve<ITimeRegistrationPeriodInfoPerTaskRepository>();
            _tenantContext = Resolve<ITenantContext>();

            _tenantContext.SetTenantId(_tenant);
            _clientHandler.AsDynamic().Handle(new ClientCreated(_clientId, "John Doe BVBA", DateTime.UtcNow));
            _projectHandler.AsDynamic().Handle(new ProjectCreated(_projectId, "Project 1", "A test project", _clientId, DateTime.UtcNow));
        }

        protected override void Because()
        {
            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(new TimeRegistrationCreated(Guid.NewGuid(), _clientId, _projectId,
                                                            "Development", 50M, "Doing some work",
                                                            Date.Parse("2012-01-30"),
                                                            Time.Parse("12:00"), Time.Parse("14:00"),
                                                            DateTime.UtcNow));

            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(new TimeRegistrationCreated(Guid.NewGuid(), _clientId, _projectId,
                                                "Development", 0M, "Doing some free work",
                                                Date.Parse("2012-01-25"),
                                                Time.Parse("10:00"), Time.Parse("14:00"),
                                                DateTime.UtcNow));

            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(new TimeRegistrationCreated(Guid.NewGuid(), _clientId, _projectId,
                                                "Meeting", 50M, "Doing some meetings",
                                                Date.Parse("2012-01-29"),
                                                Time.Parse("12:00"), Time.Parse("15:00"),
                                                DateTime.UtcNow));

            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(new TimeRegistrationCreated(Guid.NewGuid(), _clientId, _projectId,
                                                "Development", 50M, "Doing some work",
                                                Date.Parse("2012-02-01"),
                                                Time.Parse("08:00"), Time.Parse("14:00"),
                                                DateTime.UtcNow));

            _timeregistrationPeriodInfoJanDev = _timeregistrationPeriodInfoRepository.GetForMonth(2012, 01).First(t => t.Task == "Development");
            _timeregistrationPeriodInfoJanMeeting = _timeregistrationPeriodInfoRepository.GetForMonth(2012, 01).First(t => t.Task == "Meeting");
            _timeregistrationPeriodInfoFebDev = _timeregistrationPeriodInfoRepository.GetForMonth(2012, 02).First(t => t.Task == "Development");
        }

        [Fact]
        public void should_have_the_total_number_of_time_registrations()
        {
            _timeregistrationPeriodInfoJanDev.Count.Should().Be(2);
        }

        [Fact]
        public void should_have_the_total_billable_hours()
        {
            _timeregistrationPeriodInfoJanDev.BillableMinutes.Should().Be(120);
        }

        [Fact]
        public void should_have_the_total_income()
        {
            _timeregistrationPeriodInfoJanDev.Income.Should().Be(100);
        }

        [Fact]
        public void should_have_the_total_unbillable_hours()
        {
            _timeregistrationPeriodInfoJanDev.UnbillableMinutes.Should().Be(240);
        }

        [Fact]
        public void should_store_the_results_per_month_and_task()
        {
            _timeregistrationPeriodInfoJanDev.BillableMinutes.Should().Be(120);
            _timeregistrationPeriodInfoJanMeeting.BillableMinutes.Should().Be(180);
            _timeregistrationPeriodInfoFebDev.BillableMinutes.Should().Be(360);
        }

        [Fact]
        public void should_have_the_year()
        {
            _timeregistrationPeriodInfoJanDev.Year.Should().Be(2012);
        }

        [Fact]
        public void should_have_the_month()
        {
            _timeregistrationPeriodInfoJanDev.Month.Should().Be(1);
        }

        [Fact]
        public void should_have_a_tenant()
        {
            _timeregistrationPeriodInfoJanDev.Tenant.Should().Be(_tenant);
        }
    }
}
