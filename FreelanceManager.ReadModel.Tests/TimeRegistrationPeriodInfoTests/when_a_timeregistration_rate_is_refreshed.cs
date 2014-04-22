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
    public class when_a_timeregistration_rate_is_refreshed : Specification
    {
        private Guid _projectId = Guid.NewGuid();
        private Guid _clientId = Guid.NewGuid();
        private Guid _timeRegistrationId = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private ClientHandlers _clientHandler;
        private ProjectHandlers _projectHandler;
        private TimeRegistrationHandlers _timeregistrationHandler;
        private TimeRegistrationPeriodInfoHandlers _timeregistrationPeriodInfoHandler;
        private ITimeRegistrationPeriodInfoRepository _timeregistrationPeriodInfoRepository;
        private ITenantContext _tenantContext;

        private TimeRegistrationPeriodInfo _timeregistrationPeriodInfo;

        protected override void Context()
        {
            _clientHandler = Resolve<ClientHandlers>();
            _projectHandler = Resolve<ProjectHandlers>();
            _timeregistrationPeriodInfoHandler = Resolve<TimeRegistrationPeriodInfoHandlers>();
            _timeregistrationHandler = Resolve<TimeRegistrationHandlers>();
            _timeregistrationPeriodInfoRepository = Resolve<ITimeRegistrationPeriodInfoRepository>();
            _tenantContext = Resolve<ITenantContext>();

            _tenantContext.SetTenantId(_tenant);
            _clientHandler.AsDynamic().Handle(new ClientCreated(_clientId, "John Doe BVBA", DateTime.UtcNow));
            _projectHandler.AsDynamic().Handle(new ProjectCreated(_projectId, "Project 1", "A test project", _clientId, DateTime.UtcNow));

            var @event = new TimeRegistrationCreated(_timeRegistrationId, _clientId, _projectId,
                                                    "Development", 50M, "Doing some work",
                                                    Date.Parse("2012-01-30"),
                                                    Time.Parse("12:00"), Time.Parse("15:00"),
                                                    DateTime.UtcNow);

            _timeregistrationHandler.AsDynamic().Handle(@event);
            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(@event);
        }

        protected override void Because()
        {
            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(new TimeRegistrationRateRefreshed(_timeRegistrationId, 100M));

            _timeregistrationPeriodInfo = _timeregistrationPeriodInfoRepository.GetForMonth(2012, 01);
        }

        [Fact]
        public void should_have_the_same_billable_hours()
        {
            _timeregistrationPeriodInfo.BillableMinutes.Should().Be(180);
        }

        [Fact]
        public void should_have_the_new_total_income()
        {
            _timeregistrationPeriodInfo.Income.Should().Be(300);
        }

        [Fact]
        public void should_have_the_same_unbillable_hours()
        {
            _timeregistrationPeriodInfo.UnbillableMinutes.Should().Be(0);
        }
    }
}
