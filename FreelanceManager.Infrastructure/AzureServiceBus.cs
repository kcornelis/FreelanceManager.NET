using System;
using System.Configuration;
using Autofac;
using MassTransit;
using MassTransit.NLogIntegration;
using MassTransit.Transports.AzureServiceBus;
using NLog;

namespace FreelanceManager.Infrastructure
{
    public class AzureServiceBus : ServiceBusBase
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ServiceBus);
        private MassTransit.IServiceBus _bus;

        public AzureServiceBus(ILifetimeScope container)
            : base(container)
        {
        }

        public override void Start(string name)
        {
            if (_bus != null)
                throw new Exception("Bus already started");

            _logger.Info("Starting service bus, enpoint name " + name);

            _bus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseNLog();

                sbc.SetCreateTransactionalQueues(true);

                sbc.ReceiveFrom("azure-sb://" + ConfigurationManager.AppSettings["azure:serviceBusIssuerName"] +
                                ":" + ConfigurationManager.AppSettings["azure:serviceBusIssuerKey"] +
                                "@" + ConfigurationManager.AppSettings["azure:serviceBusNamespace"] + 
                                "/" + name);
                
                

                sbc.UseAzureServiceBus();
                sbc.UseAzureServiceBusRouting();

                sbc.UseJsonSerializer();
          
                sbc.Subscribe(c =>
                {
                    if (BusHasHandlers)
                    {
                        c.Handler<BusMessage>(HandleBusMessage);
                    }
                });
            });
        }

        protected override void Publish(BusMessage message)
        {
            _bus.Publish(message);
        }

        public override void Dispose()
        {
            _bus.Dispose();
        }
    }
}
