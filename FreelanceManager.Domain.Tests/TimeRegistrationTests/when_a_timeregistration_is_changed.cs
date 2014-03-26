using System;
using System.Linq;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.TimeRegistrationTests
{
    public class when_a_timeregistration_is_updated : Specification
    {
        private Guid _clientId = Guid.NewGuid();
        private Guid _projectId = Guid.NewGuid();
        private Guid _id = Guid.NewGuid();
        private TimeRegistration _timeregistration;

        protected override void Context()
        {
            var client = new Client(Guid.NewGuid(), "John Doe BVBA");
            var project = new Project(Guid.NewGuid(), "Freelancemanager", "This app", client);
            project.ChangeTasks(new[] { new Task { Name = "Dev", Rate = 50M } });

            _timeregistration = new TimeRegistration(_id, client, project, project.Tasks.First(),
                                                    "Startup",
                                                    new Date(2013, 1, 2),
                                                    new Time(11, 0), new Time(12, 0));
        }

        protected override void Because()
        {
            var client = new Client(_clientId, "Jane Doe BVBA");
            var project = new Project(_projectId, "Windows 8 app", "This app", client);

            _timeregistration.ChangeDetails(client, project, project.Tasks.First(),
                                            "Startup other project",
                                            new Date(2014, 2, 3),
                                            new Time(14, 0), new Time(16, 0));
        }

        [Fact]
        public void should_have_a_new_name()
        {
            _timeregistration.Description.Should().Be("Startup other project");
        }

        [Fact]
        public void should_be_linked_to_a_new_client()
        {
            _timeregistration.ClientId.Should().Be(_clientId);
        }

        [Fact]
        public void should_be_linked_to_a_new_project()
        {
            _timeregistration.ProjectId.Should().Be(_projectId);
        }

        [Fact]
        public void should_have_a_new_task()
        {
            _timeregistration.Task.Should().Be("Development");
        }

        [Fact]
        public void should_not_have_a_new_rate()
        {
            // rate is updated with another method
            // in the ui we ask if the rate should be updated
            _timeregistration.Rate.Should().Be(50M);
        }

        [Fact]
        public void should_have_a_new_date()
        {
            _timeregistration.Date.Numeric.Should().Be(20140203);
        }

        [Fact]
        public void should_have_a_new_start()
        {
            _timeregistration.From.Display.Should().Be("14:00");
        }

        [Fact]
        public void should_have_a_new_end()
        {
            _timeregistration.To.Display.Should().Be("16:00");
        }

        [Fact]
        public void should_have_a_total_minutes()
        {
            _timeregistration.TotalMinutes.Should().Be(120);
        }

        [Fact]
        public void version_should_be_updated()
        {
            _timeregistration.Version.Should().Be(2);
        }
    }
}
