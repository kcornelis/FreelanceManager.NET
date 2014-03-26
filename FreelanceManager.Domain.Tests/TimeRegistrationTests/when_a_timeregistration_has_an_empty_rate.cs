using System;
using System.Linq;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.TimeRegistrationTests
{
    public class when_a_timeregistration_has_an_empty_rate : Specification
    {
        private TimeRegistration _timeregistration;

        protected override void Because()
        {
            var client = new Client(Guid.NewGuid(), "John Doe BVBA");
            var project = new Project(Guid.NewGuid(), "Freelancemanager", "This app", client);

            _timeregistration = new TimeRegistration(Guid.NewGuid(), client, project, project.Tasks.First(),
                                                    "Startup",
                                                    new Date(2013, 1, 2),
                                                    new Time(11, 0), new Time(13, 0));
        }

        [Fact]
        public void should_have_an_empty_rate()
        {
            _timeregistration.Rate.Should().Be(0M);
        }

        [Fact]
        public void should_not_be_billable()
        {
            _timeregistration.Billable.Should().BeFalse();
        }

        [Fact]
        public void should_have_an_empty_income()
        {
            _timeregistration.Income.Should().Be(0M);
        }
    }
}
