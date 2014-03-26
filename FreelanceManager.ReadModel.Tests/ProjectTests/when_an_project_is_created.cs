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
    public class when_an_project_is_created : Specification
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
        }

        protected override void Because()
        {
            _projectHandler.AsDynamic().Handle(new ProjectCreated(_projectId, "Project 1", "A test project", _clientId, DateTime.Now));

            _project = _projectRepository.GetById(_projectId);
        }

        [Fact]
        public void should_have_a_id()
        {
            _project.Id.Should().Be(_projectId);
        }

        [Fact]
        public void should_have_a_name()
        {
            _project.Name.Should().Be("Project 1");
        }

        [Fact]
        public void should_have_a_description()
        {
            _project.Description.Should().Be("A test project");
        }

        [Fact]
        public void should_have_a_client_id()
        {
            _project.ClientId.Should().Be(_clientId);
        }

        [Fact]
        public void should_have_a_client_name()
        {
            _project.ClientName.Should().Be("John Doe BVBA");
        }

        [Fact]
        public void should_have_a_tenant()
        {
            _project.Tenant.Should().Be(_tenant);
        }

        [Fact]
        public void should_have_a_created_date()
        {
            _project.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }
    }
}
