using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.Domain.ProjectTests
{
    public class when_a_project_is_changed : Specification
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
            _project.ChangeDetails("Windows 8 app", "A windows 8 app");
        }

        [Fact]
        public void should_have_a_new_name()
        {
            _project.Name.Should().Be("Windows 8 app");
        }

        [Fact]
        public void should_have_a_new_description()
        {
            _project.Description.Should().Be("A windows 8 app");
        }

        [Fact]
        public void version_should_be_updated()
        {
            _project.Version.Should().Be(3);
        }
    }
}
