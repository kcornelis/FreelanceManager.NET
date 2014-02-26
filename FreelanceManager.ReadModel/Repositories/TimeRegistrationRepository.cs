using System;
using System.Collections.Generic;
using System.Linq;

namespace FreelanceManager.ReadModel.Repositories
{
    public interface ITimeRegistrationRepository : IRepository<TimeRegistration> 
    {
        IEnumerable<TimeRegistration> FindForClient(Guid clientId);
        IEnumerable<TimeRegistration> FindForProject(Guid projectId);
        IEnumerable<TimeRegistration> GetForMonth(int year, int month);
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
    }
}
