using System;
using System.Linq;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.WorkItemTests
{
    public class when_a_workitem_is_created : Specification
    {
        private Guid _id = Guid.NewGuid();
        private WorkItem _workItem;

        protected override void Because()
        {
            _workItem = new WorkItem(_id, "My WorkItem", new Date(2013, 1, 2), new Time(11, 0));
        }

        [Fact]
        public void should_have_a_id()
        {
            _workItem.Id.Should().Be(_id);
        }

        [Fact]
        public void should_have_a_description()
        {
            _workItem.Description.Should().Be("My WorkItem");
        }

        [Fact]
        public void should_have_a_duedate()
        {
            _workItem.DueDate.Numeric.Should().Be(20130102);
        }

        [Fact]
        public void should_have_a_duetime()
        {
            _workItem.DueTime.Display.Should().Be("11:00");
        }

        [Fact]
        public void should_have_a_created_date()
        {
            _workItem.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }

        [Fact]
        public void should_not_be_marked_as_deleted()
        {
            _workItem.DeletedOn.Should().Be(null);
            _workItem.Deleted.Should().BeFalse();
        }

        [Fact]
        public void version_should_be_one()
        {
            _workItem.Version.Should().Be(1);
        }
    }
}
