﻿using System;
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
    public class when_a_timeregistration_is_created : Specification
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
            _clientHandler.AsDynamic().Handle(new ClientCreated(_clientId, "John Doe BVBA", DateTime.UtcNow) { Version = 1 });
            _projectHandler.AsDynamic().Handle(new ProjectCreated(_projectId, "Project 1", "A test project", _clientId, DateTime.UtcNow) { Version = 1 });
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
        public void should_have_a_id()
        {
            _timeregistration.Id.Should().Be(_timeregistrationId);
        }

        [Fact]
        public void should_have_a_description()
        {
            _timeregistration.Description.Should().Be("Doing some work");
        }

        [Fact]
        public void should_have_a_task()
        {
            _timeregistration.Task.Should().Be("Development");
        }

        [Fact]
        public void should_have_a_rate()
        {
            _timeregistration.Rate.Should().Be(50M);
        }


        [Fact]
        public void should_have_a_income()
        {
            _timeregistration.Income.Should().Be(100M);
        }

        [Fact]
        public void should_have_a_client_id()
        {
            _timeregistration.ClientId.Should().Be(_clientId);
        }

        [Fact]
        public void should_have_a_client_name()
        {
            _timeregistration.ClientName.Should().Be("John Doe BVBA");
        }

        [Fact]
        public void should_have_a_project_id()
        {
            _timeregistration.ProjectId.Should().Be(_projectId);
        }

        [Fact]
        public void should_have_a_project_name()
        {
            _timeregistration.ProjectName.Should().Be("Project 1");
        }

        [Fact]
        public void should_have_a_date()
        {
            _timeregistration.Date.Should().Be(new Date(2012, 01, 30));
        }

        [Fact]
        public void should_have_a_start_time()
        {
            _timeregistration.From.Should().Be(new Time(12, 0));
        }

        [Fact]
        public void should_have_a_end_time()
        {
            _timeregistration.To.Should().Be(new Time(14, 0));
        }

        [Fact]
        public void should_have_the_total_minutes()
        {
            _timeregistration.Minutes.Should().Be(120);
        }

        [Fact]
        public void should_have_a_tenant()
        {
            _timeregistration.Tenant.Should().Be(_tenant);
        }

        [Fact]
        public void should_have_a_created_date()
        {
            _timeregistration.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }
    }
}
