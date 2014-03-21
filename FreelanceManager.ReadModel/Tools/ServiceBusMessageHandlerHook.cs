using System;
using MongoDB.Driver;

namespace FreelanceManager.ReadModel.Tools
{
    public class DomainUpdateServiceBusHandlerHook : IDomainUpdateServiceBusHandlerHook
    {
        private MongoCollection<ReadModelInfo> _collection;
        private ITenantContext _tenantContext;

        public DomainUpdateServiceBusHandlerHook(IMongoContext mongoContext, ITenantContext tenantContext)
        {
            _collection = mongoContext.GetDatabase().GetCollection<ReadModelInfo>("ReadModelInfo");
            _tenantContext = tenantContext;
        }

        public void PreHandle(object @event, DomainUpdateMetadate metadata)
        {
            _tenantContext.SetTenantId(metadata.Tenant);
        }

        public void PostHandle(object @event, DomainUpdateMetadate metadata)
        {
        
        }

        public void Exception(Exception ex, object @event, DomainUpdateMetadate metadata)
        {
            
        }
    }
}
