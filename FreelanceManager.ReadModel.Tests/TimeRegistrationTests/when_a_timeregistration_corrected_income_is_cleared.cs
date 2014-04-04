using System;
using FluentAssertions;
using FreelanceManager.Events.Client;
using FreelanceManager.Events.Project;
using FreelanceManager.Events.TimeRegistration;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.TimeRegistrationTests
{
    public class when_a_timeregistration_corrected_income_is_cleared : Specification
    {
        private Guid _projectId = Guid.NewGuid();
        private Guid _clientId = Guid.NewGuid();
        private Guid _timeregistrationId = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private ClientHandlers _clientHandler;
        private ProjectHandlers _projectHandler;
        private TimeRegistrationHandlers _timeregistrationHandler;
        private ITimeRegistrationRepository _timeregistrationRepository;
        private ITenantContext _tenantContext;

        private TimeRegistration _timeregistration;

        protected override void Context()
        {
            _clientHandler = Resolve<ClientHandlers>();
            _projectHandler = Resolve<ProjectHandlers>();
            _timeregistrationHandler = Resolve<TimeRegistrationHandlers>();
            _timeregistrationRepository = Resolve<ITimeRegistrationRepository>();
            _tenantContext = Resolve<ITenantContext>();

            _tenantContext.SetTenantId(_tenant);
            _clientHandler.AsDynamic().Handle(new ClientCreated(_clientId, "John Doe BVBA", DateTime.UtcNow));
            _projectHandler.AsDynamic().Handle(new ProjectCreated(_projectId, "Project 1", "A test project", _clientId, DateTime.UtcNow));
            _timeregistrationHandler.AsDynamic().Handle(new TimeRegistrationCreated(_timeregistrationId, _clientId, _projectId,
                                                "Development", 50M, "Doing some work",
                                                Date.Parse("2012-01-30"),
                                                Time.Parse("12:00"), Time.Parse("14:00"),
                                                DateTime.UtcNow));
            _timeregistrationHandler.AsDynamic().Handle(new TimeRegistrationIncomeCorrected(_timeregistrationId,
                                                   500M, "Weekend work"));
        }

        protected override void Because()
        {
            _timeregistrationHandler.AsDynamic().Handle(new TimeRegistrationCorrectedIncomeCleared(_timeregistrationId));

            _timeregistration = _timeregistrationRepository.GetById(_timeregistrationId);
        }

        [Fact]
        public void should_have_the_same_rate()
        {
            _timeregistration.Rate.Should().Be(50M);
        }

        [Fact]
        public void should_have_a_new_income()
        {
            _timeregistration.Income.Should().Be(100M);
        }

        [Fact]
        public void should_have_no_corrected_income()
        {
            _timeregistration.CorrectedIncome.Should().BeNull();
        }

        [Fact]
        public void should_have_no_corrected_income_message()
        {
            _timeregistration.CorrectedIncomeMessage.Should().BeNull();
        }
    }
}
