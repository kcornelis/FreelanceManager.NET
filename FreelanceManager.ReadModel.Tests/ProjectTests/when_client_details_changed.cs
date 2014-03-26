﻿using System;
using FluentAssertions;
using FreelanceManager.Events.Client;
using FreelanceManager.Events.Project;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.ClientTests
{
    public class when_client_details_changed : Specification
    {
        private Guid _projectId = Guid.NewGuid();
        private Guid _clientId = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private ClientHandlers _clientHandler;
        private ProjectHandlers _projectHandler;
        private IProjectRepository _projectRepository;
        private ITenantContext _tenantContext;

        private Project _project;

        protected override void Context()
        {
            _clientHandler = Resolve<ClientHandlers>();
            _projectHandler = Resolve<ProjectHandlers>();
            _projectRepository = Resolve<IProjectRepository>();
            _tenantContext = Resolve<ITenantContext>();

            _tenantContext.SetTenantId(_tenant);
            _clientHandler.AsDynamic().Handle(new ClientCreated(_clientId, "John Doe BVBA", DateTime.UtcNow));
            _projectHandler.AsDynamic().Handle(new ProjectCreated(_projectId, "Project 1", "A test project", _clientId, DateTime.Now));
        }

        protected override void Because()
        {
            _projectHandler.AsDynamic().Handle(new ClientDetailsChanged(_clientId, "Jane Turbo BVBA"));
            
            _project = _projectRepository.GetById(_projectId);
        }

        [Fact]
        public void should_have_the_new_client_details()
        {
            _project.ClientName.Should().Be("Jane Turbo BVBA");
        }
    }
}
