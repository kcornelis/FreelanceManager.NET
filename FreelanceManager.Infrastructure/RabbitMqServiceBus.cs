﻿using System;
using System.Configuration;
using Autofac;
using MassTransit;
using MassTransit.NLogIntegration;
using NLog;

namespace FreelanceManager.Infrastructure
{
    public class RabbitMqServiceBus : ServiceBusBase
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ServiceBus);
        private MassTransit.IServiceBus _bus;

        public RabbitMqServiceBus(ILifetimeScope container)
            : base(container)
        {
        }

        public override void Start(string name)
        {
            if (_bus != null)
                throw new Exception("Bus already started");

            _logger.Info("Starting service bus, enpoint name " + name);
            _logger.Info("Service bus type: rabbit mq");

            _bus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseNLog();
                sbc.UseJsonSerializer();

                sbc.UseRabbitMq();

                sbc.Subscribe(c =>
                {
                    c.Handler<DomainUpdateBusMessage>(HandleDomainUpdate);
                });

                sbc.ReceiveFrom(ConfigurationManager.AppSettings["rabbitmq:host"] + name);
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
