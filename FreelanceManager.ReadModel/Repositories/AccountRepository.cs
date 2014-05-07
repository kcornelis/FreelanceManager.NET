using MongoDB.Driver;
using MongoDB.Driver.Builders;
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

        protected override void CreateIndexes(MongoCollection<Account> collection)
        {
            base.CreateIndexes(collection);

            collection.EnsureIndex(IndexKeys<Account>.Ascending(a => a.Email), IndexOptions.SetName("IX_Email"));
        }
    }
}
