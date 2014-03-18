using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using FreelanceManager.Tools;

namespace FreelanceManager.Infrastructure
{
    // don't use this in production code, this is only good for testing
    public class InMemoryServiceBus : ServiceBusBase
    {        
        public InMemoryServiceBus(IContainer container)
            :base(container)
        {
           
        }

        public override void Start(string name)
        {

        }

        protected override void Publish(BusMessage busMessage)
        {
            HandleBusMessage(busMessage);
        }

        public override void Dispose()
        {
            
        }
    }
}
