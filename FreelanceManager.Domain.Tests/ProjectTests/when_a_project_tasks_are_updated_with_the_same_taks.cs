using System;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.ProjectTests
{
    public class when_a_project_tasks_are_updated_with_the_same_taks : Specification
    {
        private Guid _clientId = Guid.NewGuid();
        private Guid _id = Guid.NewGuid();
        private Project _project;

        protected override void Context()
        {
            var client = new Client(_clientId, "John Doe BVBA");
            _project = new Project(_id, "Freelancemanager", "This app", client);
        }

        protected override void Because()
        {
            _project.ChangeTasks(new[]{
                new Task{ Name = "Development", Rate = 0M },
                new Task{ Name = "Analyse", Rate = 0M },
                new Task{ Name = "Meeting", Rate = 0M }
            });
        }

        [Fact]
        public void version_should_not_be_updated()
        {
            _project.Version.Should().Be(2);
        }
    }
}
