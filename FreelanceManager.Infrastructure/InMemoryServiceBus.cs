using Autofac;
using FreelanceManager.Infrastructure.ServiceBus;

namespace FreelanceManager.Infrastructure
{
    /// <summary>
    /// Don't use this in production code, this is only good for testing
    /// </summary>
    public class InMemoryServiceBus : ServiceBusBase
    {
        public InMemoryServiceBus(ILifetimeScope container)
            :base(container)
        {
           
        }

        public override void Start(string name)
        {

        }

        protected override void Publish(DomainUpdateBusMessage message)
        {
            HandleDomainUpdate((DomainUpdateBusMessage)JsonSerializer.Deserialize(JsonSerializer.Serialize(message)));
        }

        public override void Dispose()
        {
            
        }
    }
}
