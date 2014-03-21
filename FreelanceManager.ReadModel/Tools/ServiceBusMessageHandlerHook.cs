using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace FreelanceManager.ReadModel.Tools
{
    public class ServiceBusMessageHandlerHook : IServiceBusMessageHandlerHook
    {
        private MongoCollection<ReadModelInfo> _collection;
        private ITenantContext _tenantContext;

        public ServiceBusMessageHandlerHook(IMongoContext mongoContext, ITenantContext tenantContext)
        {
            _collection = mongoContext.GetDatabase().GetCollection<ReadModelInfo>("ReadModelInfo");
            _tenantContext = tenantContext;
        }

        public void PreHandleBusMessage(BusMessage busMessage)
        {
            _tenantContext.SetTenantId(busMessage.Headers["Tenant"]);


        }

        public void PostHandleBusMessage(BusMessage busMessage)
        {
        
        }

        public void PreHandleMessage(object message, Dictionary<string, string> headers)
        {
          
        }

        public void PostHandleMessage(object message, Dictionary<string, string> headers)
        {
          
        }

        public void Exception(Exception ex, object message, Dictionary<string, string> headers)
        {
        
        }
    }
}
