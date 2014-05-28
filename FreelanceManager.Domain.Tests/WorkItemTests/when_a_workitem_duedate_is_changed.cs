using System;
using System.Linq;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.WorkItemTests
{
    public class when_a_workitem_duedate_is_changed : Specification
    {
        private Guid _id = Guid.NewGuid();
        private WorkItem _workItem;

        protected override void Context()
        {
            _workItem = new WorkItem(_id, "My WorkItem", null, null);
        }

        protected override void Because()
        {
            _workItem.ChangeDueDate(new Date(2013, 1, 2), new Time(11, 0));
        }

        [Fact]
        public void should_have_a_new_duedate()
        {
            _workItem.DueDate.Numeric.Should().Be(20130102);
        }

        [Fact]
        public void should_have_a_new_duetime()
        {
            _workItem.DueTime.Display.Should().Be("11:00");
        }

        [Fact]
        public void version_should_be_updated()
        {
            _workItem.Version.Should().Be(2);
        }
    }
}

