using System;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.ProjectTests
{
    public class when_searching_a_task_in_a_project : Specification
    {
        private Guid _clientId = Guid.NewGuid();
        private Guid _id = Guid.NewGuid();
        private Project _project;
        private Task _task;

        protected override void Context()
        {
            var client = new Client(_clientId, "John Doe BVBA");
            _project = new Project(_id, "Freelancemanager", "This app", client);
        }

        protected override void Because()
        {
            _task = _project.FindTask("dEveloPment");
        }

        [Fact]
        public void should_find_it_case_insensitive()
        {
            _task.Name.Should().Be("Development");
        }
    }
}
