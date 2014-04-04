using System;
using System.Linq;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.TimeRegistrationTests
{
    public class when_a_timeregistration_is_deleted : Specification
    {
        private TimeRegistration _timeregistration;

        protected override void Context()
        {
            var client = new Client(Guid.NewGuid(), "John Doe BVBA");
            var project = new Project(Guid.NewGuid(), "Freelancemanager", "This app", client);
            project.ChangeTasks(new[] { new Task { Name = "Dev", Rate = 50M } });

            _timeregistration = new TimeRegistration(Guid.NewGuid(), client, project, project.Tasks.First(),
                                                    "Startup",
                                                    new Date(2013, 1, 2),
                                                    new Time(11, 0), new Time(13, 0));
        }

        protected override void Because()
        {
            _timeregistration.Delete();
        }

        [Fact]
        public void should_not_be_billable()
        {
            _timeregistration.Billable.Should().BeFalse();
        }

        [Fact]
        public void should_have_no_income()
        {
            _timeregistration.Income.Should().Be(0M);
        }

        [Fact]
        public void should_have_no_total_minutes()
        {
            _timeregistration.TotalMinutes.Should().Be(0);
        }

        [Fact]
        public void should_be_marked_as_deleted()
        {
            _timeregistration.Deleted.Should().BeTrue();
        }

        [Fact]
        public void should_have_a_deleted_date()
        {
            _timeregistration.DeletedOn.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }
    }
}
