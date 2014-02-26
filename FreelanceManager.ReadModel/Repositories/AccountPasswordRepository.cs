namespace FreelanceManager.ReadModel.Repositories
{
    public interface IAccountPasswordRepository : IRepository<AccountPassword> { }

    public class AccountPasswordRepository : Repository<AccountPassword>, IAccountPasswordRepository
    {
        public AccountPasswordRepository(IMongoContext context, ITenantContext tenantContext)
            : base(context, tenantContext)
        {
            TenantIndependant = true;
        }

        protected override string GetCollectionName()
        {
            return "AccountPassword";
        }
    }
}
