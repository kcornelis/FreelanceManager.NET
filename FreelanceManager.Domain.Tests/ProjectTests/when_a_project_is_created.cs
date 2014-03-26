using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.Domain.ProjectTests
{
    public class when_a_project_is_created : Specification
    {
        private Guid _clientId = Guid.NewGuid();
        private Guid _id = Guid.NewGuid();
        private Project _project;

        protected override void Because()
        {
            var client = new Client(_clientId, "John Doe BVBA");
            _project = new Project(_id, "Freelancemanager", "This app", client);
        }

        [Fact]
        public void should_have_a_id()
        {
            _project.Id.Should().Be(_id);
        }

        [Fact]
        public void should_have_a_name()
        {
            _project.Name.Should().Be("Freelancemanager");
        }

        [Fact]
        public void should_have_a_description()
        {
            _project.Description.Should().Be("This app");
        }

        [Fact]
        public void should_not_be_hidden()
        {
            _project.Hidden.Should().BeFalse();
        }

        [Fact]
        public void should_be_linked_to_a_client()
        {
            _project.ClientId.Should().Be(_clientId);
        }

        [Fact]
        public void should_have_a_created_date()
        {
            _project.CreatedOn.Should().BeCloseTo(DateTime.Now, 1000);
        }

        [Fact]
        public void should_have_3_default_tasks()
        {
            _project.Tasks.Should().Contain(t => t.Name == "Development");
            _project.Tasks.Should().Contain(t => t.Name == "Analyse");
            _project.Tasks.Should().Contain(t => t.Name == "Meeting");
        }

        [Fact]
        public void should_have_tasks_with_an_empty_rate()
        {
            _project.Tasks.Should().OnlyContain(t => t.Rate == 0M);
        }

        [Fact]
        public void version_should_be_two()
        {
            // project created / tasks updated
            _project.Version.Should().Be(2);
        }
    }
}
