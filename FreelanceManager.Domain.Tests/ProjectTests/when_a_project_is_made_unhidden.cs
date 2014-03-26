using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.Domain.ProjectTests
{
    public class when_a_project_is_made_unhidden : Specification
    {
        private Guid _clientId = Guid.NewGuid();
        private Guid _id = Guid.NewGuid();
        private Project _project;

        protected override void Context()
        {
            var client = new Client(_clientId, "John Doe BVBA");
            _project = new Project(_id, "Freelancemanager", "This app", client);
            _project.Hide();
        }

        protected override void Because()
        {
            _project.Unhide();
        }

        [Fact]
        public void should_be_hidden()
        {
            _project.Hidden.Should().BeFalse();
        }

        [Fact]
        public void version_should_be_updated()
        {
            _project.Version.Should().Be(4);
        }
    }
}
