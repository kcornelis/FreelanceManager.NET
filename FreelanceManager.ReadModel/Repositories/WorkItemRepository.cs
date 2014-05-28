using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace FreelanceManager.ReadModel.Repositories
{
    public interface IWorkItemRepository : IRepository<WorkItem> { }

    public class WorkItemRepository : Repository<WorkItem>, IWorkItemRepository
    {
        public WorkItemRepository(IMongoContext context, ITenantContext tenantContext)
            : base(context, tenantContext)
        {
        }

        protected override string GetCollectionName()
        {
            return "WorkItem";
        }
    }
}
