using System;
using System.Configuration;
using Autofac;
using MassTransit;
using MassTransit.NLogIntegration;
using MassTransit.Transports.AzureServiceBus;
using NLog;

using MassTransit.Transports.AzureServiceBus.Configuration;
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

            var credentials = new Credentials(ConfigurationManager.AppSettings["azure:serviceBusIssuerName"],
                ConfigurationManager.AppSettings["azure:serviceBusIssuerKey"], 
                ConfigurationManager.AppSettings["azure:serviceBusNamespace"], 
                name);

            _bus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseNLog();

                var uri = credentials.BuildUri(name);
                Console.WriteLine(uri);

                sbc.ReceiveFrom(uri);

                sbc.Subscribe(c =>
                {
                    if (BusHasHandlers)
                    {
                        c.Handler<BusMessage>(HandleBusMessage);
                    }
                });

                sbc.UseAzureServiceBus(x =>
                {
                    x.ConfigureNamespace(uri.Host, h =>
                    {
                        h.SetKeyName(ConfigurationManager.AppSettings["azure:serviceBusIssuerName"]);
                        h.SetKey(ConfigurationManager.AppSettings["azure:serviceBusIssuerKey"]);
                    });
                });
                sbc.UseAzureServiceBusRouting();
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
