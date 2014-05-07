using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace FreelanceManager.ReadModel.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        IEnumerable<Project> FindForClient(Guid clientId);
        IEnumerable<Project> GetActive();
    }

    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(IMongoContext context, ITenantContext tenantContext)
            : base(context, tenantContext)
        {
        }

        protected override string GetCollectionName()
        {
            return "Project";
        }

        protected override void CreateIndexes(MongoCollection<Project> collection)
        {
            base.CreateIndexes(collection);

            collection.EnsureIndex(IndexKeys<Project>.Ascending(p => p.ClientId), IndexOptions.SetName("IX_ClientId"));
            collection.EnsureIndex(IndexKeys<Project>.Ascending(p => p.Hidden), IndexOptions.SetName("IX_Hidden"));
        }

        public IEnumerable<Project> FindForClient(Guid clientId)
        {
            return Context.Where(p => p.ClientId == clientId).ToList();
        }

        public IEnumerable<Project> GetActive()
        {
            return Context.Where(p => !p.Hidden).ToList();
        }
    }
}
