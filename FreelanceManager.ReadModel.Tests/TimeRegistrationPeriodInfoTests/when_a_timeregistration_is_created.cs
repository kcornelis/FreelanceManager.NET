using System;
using FluentAssertions;
using FreelanceManager.Events.Client;
using FreelanceManager.Events.Project;
using FreelanceManager.Events.TimeRegistration;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.TimeRegistrationPeriodInfoTests
{
    public class when_a_timeregistration_is_created : Specification
    {
        private Guid _projectId = Guid.NewGuid();
        private Guid _clientId = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private ClientHandlers _clientHandler;
        private ProjectHandlers _projectHandler;
        private TimeRegistrationPeriodInfoHandlers _timeregistrationPeriodInfoHandler;
        private ITimeRegistrationPeriodInfoRepository _timeregistrationPeriodInfoRepository;
        private ITenantContext _tenantContext;

        private TimeRegistrationPeriodInfo _timeregistrationPeriodInfoJan;
        private TimeRegistrationPeriodInfo _timeregistrationPeriodInfoFeb;

        protected override void Context()
        {
            _clientHandler = Resolve<ClientHandlers>();
            _projectHandler = Resolve<ProjectHandlers>();
            _timeregistrationPeriodInfoHandler = Resolve<TimeRegistrationPeriodInfoHandlers>();
            _timeregistrationPeriodInfoRepository = Resolve<ITimeRegistrationPeriodInfoRepository>();
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
                                                "Development", 50M, "Doing some work",
                                                Date.Parse("2012-02-01"),
                                                Time.Parse("08:00"), Time.Parse("14:00"),
                                                DateTime.UtcNow));

            _timeregistrationPeriodInfoJan = _timeregistrationPeriodInfoRepository.GetForMonth(2012, 01);
            _timeregistrationPeriodInfoFeb = _timeregistrationPeriodInfoRepository.GetForMonth(2012, 02);
        }

        [Fact]
        public void should_have_the_total_number_of_time_registrations()
        {
            _timeregistrationPeriodInfoJan.Count.Should().Be(2);
        }

        [Fact]
        public void should_have_the_total_billable_hours()
        {
            _timeregistrationPeriodInfoJan.BillableMinutes.Should().Be(120);
        }

        [Fact]
        public void should_have_the_total_income()
        {
            _timeregistrationPeriodInfoJan.Income.Should().Be(100);
        }

        [Fact]
        public void should_have_the_total_unbillable_hours()
        {
            _timeregistrationPeriodInfoJan.UnbillableMinutes.Should().Be(240);
        }

        [Fact]
        public void should_store_the_results_per_month()
        {
            _timeregistrationPeriodInfoJan.BillableMinutes.Should().Be(120);
            _timeregistrationPeriodInfoFeb.BillableMinutes.Should().Be(360);
        }

        [Fact]
        public void should_have_the_year()
        {
            _timeregistrationPeriodInfoJan.Year.Should().Be(2012);
        }

        [Fact]
        public void should_have_the_month()
        {
            _timeregistrationPeriodInfoJan.Month.Should().Be(1);
        }

        [Fact]
        public void should_have_a_tenant()
        {
            _timeregistrationPeriodInfoJan.Tenant.Should().Be(_tenant);
        }
    }
}
