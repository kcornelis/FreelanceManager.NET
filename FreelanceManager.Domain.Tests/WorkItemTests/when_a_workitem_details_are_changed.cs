using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.Domain.WorkItemTests
{
    public class when_a_workitem_details_are_changed : Specification
    {
        private Guid _id = Guid.NewGuid();
        private WorkItem _workItem;

        protected override void Context()
        {
            _workItem = new WorkItem(_id, "My WorkItem", new Date(2013, 1, 2), new Time(11, 0));
        }

        protected override void Because()
        {
            _workItem.ChangeDetails("Other WorkItem");
        }

        [Fact]
        public void should_have_a_new_description()
        {
            _workItem.Description.Should().Be("Other WorkItem");
        }

        [Fact]
        public void version_should_be_updated()
        {
            _workItem.Version.Should().Be(2);
        }
    }
}
