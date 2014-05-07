using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace FreelanceManager.ReadModel.Repositories
{
    public interface ITimeRegistrationRepository : IRepository<TimeRegistration> 
    {
        IEnumerable<TimeRegistration> FindForClient(Guid clientId);
        IEnumerable<TimeRegistration> FindForProject(Guid projectId);
        IEnumerable<TimeRegistration> GetForMonth(int year, int month);
        IEnumerable<TimeRegistration> GetAll();
        IEnumerable<TimeRegistration> GetForPeriod(DateTime from, DateTime to);
    }

    public class TimeRegistrationRepository : Repository<TimeRegistration>, ITimeRegistrationRepository
    {
        public TimeRegistrationRepository(IMongoContext context, ITenantContext tenantContext)
            : base(context, tenantContext)
        {
        }

        protected override string GetCollectionName()
        {
            return "TimeRegistration";
        }

        protected override void CreateIndexes(MongoCollection<TimeRegistration> collection)
        {
            base.CreateIndexes(collection);

            collection.EnsureIndex(IndexKeys<TimeRegistration>.Ascending(t => t.ProjectId), IndexOptions.SetName("IX_ProjectId"));
            collection.EnsureIndex(IndexKeys<TimeRegistration>.Ascending(t => t.ClientId), IndexOptions.SetName("IX_ClientId"));
            collection.EnsureIndex(IndexKeys<TimeRegistration>.Ascending(t => t.Date.Numeric), IndexOptions.SetName("IX_DateNumeric"));
            collection.EnsureIndex(IndexKeys<TimeRegistration>.Ascending(t => t.Date.Year).Ascending(t => t.Date.Month), IndexOptions.SetName("IX_DateYearMonth"));
        }

        public IEnumerable<TimeRegistration> FindForClient(Guid clientId)
        {
            return Context.Where(p => p.ClientId == clientId).ToList();
        }

        public IEnumerable<TimeRegistration> FindForProject(Guid projectId)
        {
            return Context.Where(p => p.ProjectId == projectId).ToList();
        }

        public IEnumerable<TimeRegistration> GetForMonth(int year, int month)
        {
            return Context.Where(t => t.Date.Year == year && t.Date.Month == month).ToList();
        }

        public IEnumerable<TimeRegistration> GetAll()
        {
            return Context.ToList();
        }

        public IEnumerable<TimeRegistration> GetForPeriod(DateTime from, DateTime to)
        {
            var min = from.GetNumericValue();
            var max = to.GetNumericValue();

            return Context.Where(t => t.Date.Numeric >= min && t.Date.Numeric < max)
                          .OrderBy(t => t.Date.Numeric)
                          .ThenBy(t => t.From.Numeric)
                          .ToList();
        }
    }
}
