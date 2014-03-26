using System;
using System.Linq;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.TimeRegistrationTests
{
    public class when_a_timeregistration_has_a_rate : Specification
    {
        private TimeRegistration _timeregistration;

        protected override void Because()
        {
            var client = new Client(Guid.NewGuid(), "John Doe BVBA");
            var project = new Project(Guid.NewGuid(), "Freelancemanager", "This app", client);
            project.ChangeTasks(new[] { new Task { Name = "Dev", Rate = 50M } });

            _timeregistration = new TimeRegistration(Guid.NewGuid(), client, project, project.Tasks.First(),
                                                    "Startup",
                                                    new Date(2013, 1, 2),
                                                    new Time(11, 0), new Time(13, 0));
        }

        [Fact]
        public void should_have_a_rate()
        {
            _timeregistration.Rate.Should().Be(50M);
        }

        [Fact]
        public void should_be_billable()
        {
            _timeregistration.Billable.Should().BeTrue();
        }

        [Fact]
        public void should_have_an_income()
        {
            _timeregistration.Income.Should().Be(100M);
        }
    }
}
