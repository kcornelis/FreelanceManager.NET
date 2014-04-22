using System;
using System.Linq;
using FluentAssertions;
using FreelanceManager.Events.Client;
using FreelanceManager.Events.Project;
using FreelanceManager.Events.TimeRegistration;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.TimeRegistrationPeriodInfoPerTaskaTests
{
    public class when_a_timeregistration_corrected_income_is_cleared : Specification
    {
        private Guid _projectId = Guid.NewGuid();
        private Guid _clientId = Guid.NewGuid();
        private Guid _timeRegistrationId = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private ClientHandlers _clientHandler;
        private ProjectHandlers _projectHandler;
        private TimeRegistrationHandlers _timeregistrationHandler;
        private TimeRegistrationPeriodInfoPerTaskHandlers _timeregistrationPeriodInfoHandler;
        private ITimeRegistrationPeriodInfoPerTaskRepository _timeregistrationPeriodInfoRepository;
        private ITenantContext _tenantContext;

        private TimeRegistrationPeriodInfoPerTask _timeregistrationPeriodInfo;

        protected override void Context()
        {
            _clientHandler = Resolve<ClientHandlers>();
            _projectHandler = Resolve<ProjectHandlers>();
            _timeregistrationPeriodInfoHandler = Resolve<TimeRegistrationPeriodInfoPerTaskHandlers>();
            _timeregistrationHandler = Resolve<TimeRegistrationHandlers>();
            _timeregistrationPeriodInfoRepository = Resolve<ITimeRegistrationPeriodInfoPerTaskRepository>();
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

            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(new TimeRegistrationIncomeCorrected(_timeRegistrationId,
                                                                                                      300M, "A test"));
        }

        protected override void Because()
        {
            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(new TimeRegistrationCorrectedIncomeCleared(_timeRegistrationId));

            _timeregistrationPeriodInfo = _timeregistrationPeriodInfoRepository.GetForMonth(2012, 01).First(t => t.Task == "Development");
        }

        [Fact]
        public void should_have_the_same_total_billable_hours()
        {
            _timeregistrationPeriodInfo.BillableMinutes.Should().Be(180);
        }

        [Fact]
        public void should_have_the_new_total_income()
        {
            _timeregistrationPeriodInfo.Income.Should().Be(150);
        }

        [Fact]
        public void should_have_the_same_unbillable_hours()
        {
            _timeregistrationPeriodInfo.UnbillableMinutes.Should().Be(0);
        }
    }
}
