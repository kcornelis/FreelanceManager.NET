namespace FreelanceManager.ReadModel.Repositories
{
    public interface IAccountRepository : IRepository<Account> { }

    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(IMongoContext context, ITenantContext tenantContext)
            : base(context, tenantContext)
        {
            TenantIndependant = true;
        }

        protected override string GetCollectionName()
        {
            return "Account";
        }
    }
}
