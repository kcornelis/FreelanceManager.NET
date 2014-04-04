using System;
using System.Linq;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.TimeRegistrationTests
{
    public class when_a_timeregistration_is_created : Specification
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
                                                    new Time(11, 0), new Time(12, 0));
        }

        [Fact]
        public void should_have_a_id()
        {
            _timeregistration.Id.Should().Be(_id);
        }

        [Fact]
        public void should_have_a_name()
        {
            _timeregistration.Description.Should().Be("Startup");
        }

        [Fact]
        public void should_be_linked_to_a_client()
        {
            _timeregistration.ClientId.Should().Be(_clientId);
        }

        [Fact]
        public void should_be_linked_to_a_project()
        {
            _timeregistration.ProjectId.Should().Be(_projectId);
        }

        [Fact]
        public void should_have_a_task()
        {
            _timeregistration.Task.Should().Be("Dev");
        }

        [Fact]
        public void should_have_a_rate()
        {
            _timeregistration.Rate.Should().Be(50M);
        }

        [Fact]
        public void should_have_a_date()
        {
            _timeregistration.Date.Numeric.Should().Be(20130102);
        }

        [Fact]
        public void should_have_a_start()
        {
            _timeregistration.From.Display.Should().Be("11:00");
        }

        [Fact]
        public void should_have_an_end()
        {
            _timeregistration.To.Display.Should().Be("12:00");
        }

        [Fact]
        public void should_have_a_total_minutes()
        {
            _timeregistration.TotalMinutes.Should().Be(60);
        }

        [Fact]
        public void should_have_a_created_date()
        {
            _timeregistration.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }

        [Fact]
        public void should_not_be_marked_as_deleted()
        {
            _timeregistration.DeletedOn.Should().Be(null);
            _timeregistration.Deleted.Should().BeFalse();
        }

        [Fact]
        public void should_have_no_corrected_income()
        {
            _timeregistration.CorrectedIncome.Should().BeNull();
        }

        [Fact]
        public void version_should_be_one()
        {
            _timeregistration.Version.Should().Be(1);
        }
    }
}
