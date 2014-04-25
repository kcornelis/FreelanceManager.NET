namespace FreelanceManager.ReadModel.Tools
{
    public class DomainUpdateServiceBusHandlerHook : IDomainUpdateServiceBusHandlerHook
    {
        private ITenantContext _tenantContext;

        public DomainUpdateServiceBusHandlerHook(IMongoContext mongoContext, ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        public void PreHandle(object[] events, DomainUpdateMetadate metadata)
        {
            _tenantContext.SetTenantId(metadata.Tenant);
        }


        public void PostHandle(object[] events, DomainUpdateMetadate metadata)
        {
        }

        public void PreHandleEvent(object @event, DomainUpdateMetadate metadata)
        {
        }

        public void PostHandleEvent(object @event, DomainUpdateMetadate metadata)
        {
        }

        public void Exception(System.Exception ex, object[] events, DomainUpdateMetadate metadata)
        {
        }
    }
}
