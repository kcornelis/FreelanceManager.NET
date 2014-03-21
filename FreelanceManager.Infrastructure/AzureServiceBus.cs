using System;
using System.Configuration;
using Autofac;
using FreelanceManager.Infrastructure.ServiceBus;
using MassTransit;
using MassTransit.NLogIntegration;
using MassTransit.Transports.AzureServiceBus;
using MassTransit.Transports.AzureServiceBus.Configuration;
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

            var credentials = new Credentials(ConfigurationManager.AppSettings["azure:serviceBusIssuerName"],
                ConfigurationManager.AppSettings["azure:serviceBusIssuerKey"], 
                ConfigurationManager.AppSettings["azure:serviceBusNamespace"], 
                name);

            _bus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseNLog();

                sbc.UseJsonSerializer();
                sbc.ConfigureJsonSerializer(c => JsonSerializer.Settings);
                sbc.ConfigureJsonDeserializer(c => JsonSerializer.Settings);

                var uri = credentials.BuildUri(name);

                sbc.ReceiveFrom(uri);

                sbc.Subscribe(c =>
                {
                    if (BusHasHandlers)
                    {
                        c.Handler<DomainUpdateBusMessage>(HandleDomainUpdate);
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

        protected override void Publish(DomainUpdateBusMessage message)
        {
            _bus.Publish(message);
        }

        public override void Dispose()
        {
            _bus.Dispose();
        }
    }
}
