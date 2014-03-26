using System;
using System.Linq;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.TimeRegistrationTests
{
    public class when_a_timeregistration_has_a_corrected_income : Specification
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
            _timeregistration.CorrectIncome(200, "Correct income test");
        }

        [Fact]
        public void should_have_a_corrected_income()
        {
            _timeregistration.CorrectedIncome.Total.Should().Be(200M);
        }

        [Fact]
        public void should_have_a_corrected_income_message()
        {
            _timeregistration.CorrectedIncome.Message.Should().Be("Correct income test");
        }

        [Fact]
        public void should_have_a_new_income()
        {
            _timeregistration.Income.Should().Be(200M);
        }

        [Fact]
        public void should_be_billable()
        {
            _timeregistration.Billable.Should().BeTrue();
        }

        [Fact]
        public void should_have_a_new_version()
        {
            _timeregistration.Version.Should().Be(2);
        }
    }
}
