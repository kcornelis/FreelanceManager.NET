using System;
using System.Configuration;
using System.Transactions;
using Autofac;
using FreelanceManager.Infrastructure.ServiceBus;
using MassTransit;
using MassTransit.NLogIntegration;
using NLog;

namespace FreelanceManager.Infrastructure
{
    public class MsmqServiceBus : ServiceBusBase
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ServiceBus);
        private MassTransit.IServiceBus _bus;

        public MsmqServiceBus(ILifetimeScope container)
            : base(container)
        {
        }

        public override void Start(string name)
        {
            if (_bus != null)
                throw new Exception("Bus already started");

            _logger.Info("Starting service bus, enpoint name " + name);
            _logger.Info("Service bus type: msmq");

            _bus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseNLog();
                sbc.UseJsonSerializer();

                sbc.UseMsmq(c =>
                {
                    c.VerifyMsmqConfiguration();
                    c.UseMulticastSubscriptionClient();
                });

                sbc.Subscribe(c =>
                {
                    if (BusHasHandlers)
                    {
                        c.Handler<DomainUpdateBusMessage>(HandleDomainUpdate);
                    }
                });

                sbc.ReceiveFrom(ConfigurationManager.AppSettings["msmq:host"] + name);
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
