using System;
using System.Linq;

namespace FreelanceManager.ReadModel.Repositories
{
    public interface ITimeRegistrationPeriodInfoRepository : IRepository<TimeRegistrationPeriodInfo>
    {
        TimeRegistrationPeriodInfo GetForMonth(int year, int month);
        TimeRegistrationPeriodInfo GetForTimeRegistration(Guid id);
    }

    public class TimeRegistrationPeriodInfoRepository : Repository<TimeRegistrationPeriodInfo>, ITimeRegistrationPeriodInfoRepository
    {
        public TimeRegistrationPeriodInfoRepository(IMongoContext context, ITenantContext tenantContext)
            : base(context, tenantContext)
        {
        }

        protected override string GetCollectionName()
        {
            return "TimeRegistrationPeriodInfo";
        }

        public TimeRegistrationPeriodInfo GetForMonth(int year, int month)
        {
            return Context.Where(i => i.Year == year && i.Month == month).FirstOrDefault();
        }

        public TimeRegistrationPeriodInfo GetForTimeRegistration(Guid id)
        {
            return Context.Where(s => s.Items.Any(i => i.Id == id)).FirstOrDefault();
        }
    }
}
