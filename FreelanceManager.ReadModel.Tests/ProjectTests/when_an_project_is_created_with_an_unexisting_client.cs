using System;
using FluentAssertions;
using FreelanceManager.Events.Client;
using FreelanceManager.Events.Project;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.ClientTests
{
    public class when_an_project_is_created_with_an_unexisting_client : Specification
    {
        private Guid _clientId = Guid.NewGuid();
        private Guid _projectId = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private ProjectHandlers _projectHandler;
        private IProjectRepository _projectRepository;
        private ITenantContext _tenantContext;

        private Project _project;

        protected override void Context()
        {
            _projectHandler = Resolve<ProjectHandlers>();
            _projectRepository = Resolve<IProjectRepository>();
            _tenantContext = Resolve<ITenantContext>();

            _tenantContext.SetTenantId(_tenant);
        }

        protected override void Because()
        {
            _projectHandler.AsDynamic().Handle(new ProjectCreated(_projectId, "Project 1", "A test project", _clientId, DateTime.UtcNow));

            _project = _projectRepository.GetById(_projectId);
        }

        [Fact]
        public void should_have_a_client_id()
        {
            _project.ClientId.Should().Be(_clientId);
        }

        [Fact]
        public void should_have_an_empty_client_name()
        {
            _project.ClientName.Should().BeEmpty();
        }
    }
}
