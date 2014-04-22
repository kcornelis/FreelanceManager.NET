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

namespace FreelanceManager.ReadModel.TimeRegistrationPeriodInfoPerTaskTests
{
    public class when_a_timeregistration_is_deleted : Specification
    {
        private Guid _projectId = Guid.NewGuid();
        private Guid _clientId = Guid.NewGuid();
        private Guid _timeRegistration1Id = Guid.NewGuid();
        private Guid _timeRegistration2Id = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private ClientHandlers _clientHandler;
        private ProjectHandlers _projectHandler;
        private TimeRegistrationPeriodInfoPerTaskHandlers _timeregistrationPeriodInfoHandler;
        private ITimeRegistrationPeriodInfoPerTaskRepository _timeregistrationPeriodInfoRepository;
        private ITenantContext _tenantContext;

        private TimeRegistrationPeriodInfoPerTask _timeregistrationPeriodInfo;

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

            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(new TimeRegistrationCreated(_timeRegistration1Id, _clientId, _projectId,
                                                "Development", 50M, "Doing some work",
                                                Date.Parse("2012-01-30"),
                                                Time.Parse("12:00"), Time.Parse("14:00"),
                                                DateTime.UtcNow));

            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(new TimeRegistrationCreated(_timeRegistration2Id, _clientId, _projectId,
                                                "Development", 0M, "Doing some free work",
                                                Date.Parse("2012-01-25"),
                                                Time.Parse("12:00"), Time.Parse("15:00"),
                                                DateTime.UtcNow));

            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(new TimeRegistrationCreated(Guid.NewGuid(), _clientId, _projectId,
                                                "Development", 0M, "Doing some other free work",
                                                Date.Parse("2012-01-25"),
                                                Time.Parse("12:00"), Time.Parse("15:00"),
                                                DateTime.UtcNow));
        }

        protected override void Because()
        {
            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(new TimeRegistrationDeleted(_timeRegistration1Id, DateTime.UtcNow));
            _timeregistrationPeriodInfoHandler.AsDynamic().Handle(new TimeRegistrationDeleted(_timeRegistration2Id, DateTime.UtcNow));

            _timeregistrationPeriodInfo = _timeregistrationPeriodInfoRepository.GetForMonth(2012, 01).First(t => t.Task == "Development");
        }

        [Fact]
        public void should_have_a_new_billable_hours()
        {
            _timeregistrationPeriodInfo.BillableMinutes.Should().Be(0);
        }

        [Fact]
        public void should_have_a_new_total_income()
        {
            _timeregistrationPeriodInfo.Income.Should().Be(0);
        }

        [Fact]
        public void should_have_a_new_unbillable_hours()
        {
            _timeregistrationPeriodInfo.UnbillableMinutes.Should().Be(180);
        }

        [Fact]
        public void should_have_a_new_time_registration_count()
        {
            _timeregistrationPeriodInfo.Count.Should().Be(1);
        }
    }
}
