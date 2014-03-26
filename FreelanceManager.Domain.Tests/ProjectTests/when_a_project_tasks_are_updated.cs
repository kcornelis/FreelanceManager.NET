using System;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.ProjectTests
{
    public class when_a_project_tasks_are_updated : Specification
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
                new Task{ Name = "Development", Rate = 30M },
                new Task{ Name = "Analyse", Rate = 0M },
                new Task{ Name = "TeamBuilding", Rate = 40M }
            });
        }

        [Fact]
        public void should_update_the_rate_for_existing_tasks()
        {
            _project.Tasks.Should().Contain(t => t.Name == "Development" && t.Rate == 30M);
        }

        [Fact]
        public void should_leave_existing_tasks_untouched()
        {
            _project.Tasks.Should().Contain(t => t.Name == "Analyse" && t.Rate == 0M);
        }

        [Fact]
        public void should_remove_removed_tasks()
        {
            _project.Tasks.Should().HaveCount(3);
            _project.Tasks.Should().NotContain(t => t.Name == "Meeting");
        }

        [Fact]
        public void should_contain_new_taks()
        {
            _project.Tasks.Should().Contain(t => t.Name == "TeamBuilding" && t.Rate == 40M);
        }

        [Fact]
        public void version_should_be_updated()
        {
            _project.Version.Should().Be(3);
        }
    }
}
