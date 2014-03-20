using System;
using System.Configuration;
using System.Transactions;
using Autofac;
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

            _bus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseNLog();
                sbc.UseJsonSerializer();

                sbc.ReceiveFrom(ConfigurationManager.AppSettings["msmq:host"] + name);

                sbc.UseMsmq();
                sbc.VerifyMsmqConfiguration();
                sbc.UseMulticastSubscriptionClient();

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
