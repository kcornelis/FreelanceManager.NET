using System;
using System.Collections.Generic;
using System.Linq;

namespace FreelanceManager.ReadModel.Repositories
{
    public interface ITimeRegistrationPeriodInfoPerTaskRepository : IRepository<TimeRegistrationPeriodInfoPerTask>
    {
        TimeRegistrationPeriodInfoPerTask GetForProjectAndMonth(Guid projectId, string task, int year, int month);
        TimeRegistrationPeriodInfoPerTask GetForTimeRegistration(Guid id);
        IEnumerable<TimeRegistrationPeriodInfoPerTask> GetForMonth(int year, int month);
        IEnumerable<TimeRegistrationPeriodInfoPerTask> FindForClient(Guid clientId);
        IEnumerable<TimeRegistrationPeriodInfoPerTask> FindForProject(Guid projectId);
    }

    public class TimeRegistrationPeriodInfoPerTaskRepository : Repository<TimeRegistrationPeriodInfoPerTask>, ITimeRegistrationPeriodInfoPerTaskRepository
    {
        private readonly ITenantContext _tenantContext;

        public TimeRegistrationPeriodInfoPerTaskRepository(IMongoContext context, ITenantContext tenantContext)
            : base(context, tenantContext)
        {
            _tenantContext = tenantContext;
        }

        protected override string GetCollectionName()
        {
            return "TimeRegistrationPeriodInfoPerTask";
        }

        public IEnumerable<TimeRegistrationPeriodInfoPerTask> GetForMonth(int year, int month)
        {
            return Context.Where(i => i.Year == year && i.Month == month).ToList();
        }

        public IEnumerable<TimeRegistrationPeriodInfoPerTask> FindForClient(Guid clientId)
        {
            return Context.Where(p => p.ClientId == clientId).ToList();
        }

        public IEnumerable<TimeRegistrationPeriodInfoPerTask> FindForProject(Guid projectId)
        {
            return Context.Where(p => p.ProjectId == projectId).ToList();
        }

        public TimeRegistrationPeriodInfoPerTask GetForProjectAndMonth(Guid projectId, string task, int year, int month)
        {
            return Context.Where(i => i.ProjectId == projectId && i.Task == task)
                .Where(i => i.Year == year && i.Month == month).FirstOrDefault();
        }

        public TimeRegistrationPeriodInfoPerTask GetForTimeRegistration(Guid id)
        {
            return Context.Where(s => s.Items.Any(i => i.Id == id)).FirstOrDefault();
        }
    }
}
