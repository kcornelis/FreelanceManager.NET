namespace FreelanceManager.ReadModel.Repositories
{
    public interface IClientRepository : IRepository<Client> { }

    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(IMongoContext context, ITenantContext tenantContext)
            : base(context, tenantContext)
        {
        }

        protected override string GetCollectionName()
        {
            return "Client";
        }
    }
}
