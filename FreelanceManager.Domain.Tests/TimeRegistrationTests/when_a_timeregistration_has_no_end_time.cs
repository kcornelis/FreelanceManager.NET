using System;
using System.Linq;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.TimeRegistrationTests
{
    public class when_a_timeregistration_has_no_end_time : Specification
    {
        private Guid _clientId = Guid.NewGuid();
        private Guid _projectId = Guid.NewGuid();
        private Guid _id = Guid.NewGuid();
        private TimeRegistration _timeregistration;

        protected override void Because()
        {
            var client = new Client(_clientId, "John Doe BVBA");
            var project = new Project(_projectId, "Freelancemanager", "This app", client);
            project.ChangeTasks(new[] { new Task { Name = "Dev", Rate = 50M } });

            _timeregistration = new TimeRegistration(_id, client, project, project.Tasks.First(),
                                                    "Startup",
                                                    new Date(2013, 1, 2),
                                                    new Time(11, 0), null);
        }

        [Fact]
        public void should_have_no_income()
        {
            _timeregistration.Income.Should().Be(0);
        }

        [Fact]
        public void should_have_a_total_minutes_of_0()
        {
            _timeregistration.TotalMinutes.Should().Be(0);
        }
    }
}