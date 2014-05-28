using System;
using System.Linq;
using FluentAssertions;
using FreelanceManager.Domain.ValueObjects;
using Xunit;

namespace FreelanceManager.Domain.WorkItemTests
{
    public class when_a_workitem_is_deleted : Specification
    {
        private WorkItem _workItem;

        protected override void Context()
        {
            _workItem = new WorkItem(Guid.NewGuid(), "My WorkItem", null, null);
        }

        protected override void Because()
        {
            _workItem.Delete();
        }

        [Fact]
        public void should_be_marked_as_deleted()
        {
            _workItem.Deleted.Should().BeTrue();
        }

        [Fact]
        public void should_have_a_deleted_date()
        {
            _workItem.DeletedOn.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }
    }
}
