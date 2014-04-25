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
    public class when_a_timeregistration_is_created_with_an_unexisting_client_and_project : Specification
    {
        private Guid _projectId = Guid.NewGuid();
        private Guid _clientId = Guid.NewGuid();
        private Guid _timeregistrationId = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private TimeRegistrationHandlers _timeregistrationHandler;
        private ITimeRegistrationRepository _timeregistrationRepository;
        private ITenantContext _tenantContext;

        private TimeRegistration _timeregistration;

        protected override void Context()
        {
            _timeregistrationHandler = Resolve<TimeRegistrationHandlers>();
            _timeregistrationRepository = Resolve<ITimeRegistrationRepository>();
            _tenantContext = Resolve<ITenantContext>();

            _tenantContext.SetTenantId(_tenant);
        }

        protected override void Because()
        {
            _timeregistrationHandler.AsDynamic().Handle(new TimeRegistrationCreated(_timeregistrationId, _clientId, _projectId,
                                                            "Development", 50M, "Doing some work",
                                                            Date.Parse("2012-01-30"),
                                                            Time.Parse("12:00"), Time.Parse("14:00"),
                                                            DateTime.UtcNow) { Version = 1 });

            _timeregistration = _timeregistrationRepository.GetById(_timeregistrationId);
        }

        [Fact]
        public void should_have_a_client_id()
        {
            _timeregistration.ClientId.Should().Be(_clientId);
        }

        [Fact]
        public void should_have_an_empty_client_name()
        {
            _timeregistration.ClientName.Should().BeEmpty();
        }

        [Fact]
        public void should_have_a_project_id()
        {
            _timeregistration.ProjectId.Should().Be(_projectId);
        }

        [Fact]
        public void should_have_a_project_name()
        {
            _timeregistration.ProjectName.Should().BeEmpty();
        }
    }
}
