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
    public class when_an_project_tasks_are_changed : Specification
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
            _projectHandler.AsDynamic().Handle(new ProjectTasksChanged(_projectId, new Dtos.Task[]
            {
                new Dtos.Task{ Name = "Initial task", Rate = 50M },
            }) { Version = 2 });
        }

        protected override void Because()
        {
            _projectHandler.AsDynamic().Handle(new ProjectTasksChanged(_projectId, new Dtos.Task[]
            {
                new Dtos.Task{ Name = "Task 1", Rate = 0M },
                new Dtos.Task{ Name = "Task 2", Rate = 30M },
            }) { Version = 3 });
            
            _project = _projectRepository.GetById(_projectId);
        }

        [Fact]
        public void should_have_new_tasks()
        {
            _project.Tasks.Should().HaveCount(2);

            _project.Tasks.Should().Contain(t => t.Name == "Task 1" && t.Rate == 0M);
            _project.Tasks.Should().Contain(t => t.Name == "Task 2" && t.Rate == 30M);
        }
    }
}
