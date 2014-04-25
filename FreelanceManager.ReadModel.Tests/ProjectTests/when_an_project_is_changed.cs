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
    public class when_an_project_is_changed : Specification
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
            _clientHandler.AsDynamic().Handle(new ClientCreated(_clientId, "John Doe BVBA", DateTime.UtcNow) { Version = 1 });
            _projectHandler.AsDynamic().Handle(new ProjectCreated(_projectId, "Project 1", "A test project", _clientId, DateTime.UtcNow) { Version = 1 });
        }

        protected override void Because()
        {
            _projectHandler.AsDynamic().Handle(new ProjectDetailsChanged(_projectId, "Project 2", "A project") { Version = 2 });
            
            _project = _projectRepository.GetById(_projectId);
        }

        [Fact]
        public void should_have_a_new_name()
        {
            _project.Name.Should().Be("Project 2");
        }

        [Fact]
        public void should_have_a_new_description()
        {
            _project.Description.Should().Be("A project");
        }
    }
}
