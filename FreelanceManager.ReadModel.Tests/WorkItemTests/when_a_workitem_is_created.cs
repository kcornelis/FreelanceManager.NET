﻿using System;
using FluentAssertions;
using FreelanceManager.Events.WorkItem;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.WorkItemTests
{
    public class when_an_workItem_is_created : Specification
    {
        private Guid _id = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private WorkItemHandlers _handler;
        private IWorkItemRepository _workItemRepository;
        private ITenantContext _tenantContext;

        private WorkItem _workItem;

        protected override void Context()
        {
            _handler = Resolve<WorkItemHandlers>();
            _workItemRepository = Resolve<IWorkItemRepository>();
            _tenantContext = Resolve<ITenantContext>();

            _tenantContext.SetTenantId(_tenant);
        }

        protected override void Because()
        {
            _handler.AsDynamic().Handle(new WorkItemCreated(_id, "WorkItem Test", new Date(2000, 1, 2), new Time(12, 0), DateTime.UtcNow) { Version = 1 });

            _workItem = _workItemRepository.GetById(_id);
        }

        [Fact]
        public void should_have_a_id()
        {
            _workItem.Id.Should().Be(_id);
        }

        [Fact]
        public void should_have_a_description()
        {
            _workItem.Description.Should().Be("WorkItem Test");
        }

        [Fact]
        public void should_have_a_duedate()
        {
            _workItem.DueDate.Should().Be(new Date(2000, 1, 2));
        }

        [Fact]
        public void should_have_a_duetime()
        {
            _workItem.DueTime.Should().Be(new Time(12, 0));
        }

        [Fact]
        public void should_have_a_tenant()
        {
            _workItem.Tenant.Should().Be(_tenant);
        }

        [Fact]
        public void should_have_a_created_date()
        {
            _workItem.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }
    }
}