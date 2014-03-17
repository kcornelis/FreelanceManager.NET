using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using Autofac;
using FreelanceManager.Infrastructure.ServiceBus;
using FreelanceManager.Tools;
using MassTransit;
using NLog;
using MassTransit.Transports.AzureServiceBus;
using MassTransit.Transports.AzureServiceBus.Configuration;
using MassTransit.NLogIntegration;

namespace FreelanceManager.Infrastructure
{
    public enum MassTransitTransport
    {
        Msmq,
        Azure
    }

    public class MassTransitServiceBus : IServiceBus, IDisposable
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ServiceBus);

        private readonly Dictionary<Type, List<Type>> _eventTypesWithHandlers = new Dictionary<Type, List<Type>>();
        private MassTransit.IServiceBus _bus;
        private ILifetimeScope _container;

        public MassTransitServiceBus(MassTransitTransport transport, ILifetimeScope container)
        {
            _container = container;
        }

        public void Start(string name)
        {
            if (_bus != null)
                throw new Exception("Bus already started");

            _logger.Info("Starting service bus, enpoint name " + name);

            _bus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseNLog();
                
                //sbc.ReceiveFrom("azure-sb://RootManageSharedAccessKey:7moM1rrMPZHZw6K1WdpUlqQ+TkZtiE2nje4hQybQzTE=@myNamespace/my-application");
                //RootManageSharedAccessKey 7moM1rrMPZHZw6K1WdpUlqQ+TkZtiE2nje4hQybQzTE=
                
                sbc.SetCreateTransactionalQueues(true);
                
                sbc.UseMsmq(c =>
                {
                    c.UseMulticastSubscriptionClient();
                    c.VerifyMsmqConfiguration();
                   
                });

                sbc.UseJsonSerializer();
          
                sbc.Subscribe(c =>
                {
                    if (_eventTypesWithHandlers.Any())
                    {
                        c.Handler<BusMessage>(HandleBusMessage);
                    }
                });

                sbc.ReceiveFrom("msmq://localhost/" + name);
            });
        }

        public void Send(string endpoint, object[] messages, Dictionary<string, string> headers)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Sending " + messages.Length + " messages to " + endpoint);
            }

            if (_logger.IsTraceEnabled)
            {
                foreach (var m in messages)
                {
                    _logger.Trace("Sending " + JsonSerializer.Serialize(m));
                }
            }

            var busMessage = new BusMessage
            {
                Messages = messages.Select(m => JsonSerializer.Serialize(m)).ToArray(),
                Headers = headers
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                _bus.GetEndpoint(new Uri("msmq://localhost/" + endpoint)).Send(busMessage);

                scope.Complete();
            }
        }

        public void Publish(object[] messages, Dictionary<string, string> headers)
        {
            _logger.Debug("Publishing " + messages.Length + " messages.");

            var busMessage = new BusMessage
            {
                Messages = messages.Select(m => JsonSerializer.Serialize(m)).ToArray(),
                Headers = headers
            };

            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                _bus.Publish(busMessage);

                scope.Complete();
            }
        }

        public void RegisterHandlers(Assembly assembly)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(assembly)
                   .Where(t => t.Name.EndsWith("Handlers")) // todo: implements ihandleevent
                   .AsSelf();

            builder.Update(_container.ComponentRegistry);

            foreach (var type in assembly.GetTypes())
            {
                foreach (var handlerInterfaceType in type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleEvent<>)))
                {
                    var messageType = handlerInterfaceType.GetGenericArguments().First();

                    if (!_eventTypesWithHandlers.ContainsKey(messageType))
                    {
                        _eventTypesWithHandlers.Add(messageType, new List<Type>());
                    }

                    _eventTypesWithHandlers[messageType].Add(type);
                }
            }
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        private void HandleBusMessage(BusMessage busMessage)
        {
            using (var lifeTimeScope = _container.BeginLifetimeScope())
            {
                var messages = busMessage.Messages.Select(m => JsonSerializer.Deserialize((string)m));
                foreach (var message in messages)
                {
                    _logger.Debug("Received message contains event with type " + message.GetType().Name);

                    // todo rethink about the admin parameter
                    lifeTimeScope.Resolve<ITenantContext>().SetTenantId(busMessage.Headers["Tenant"], false);

                    var eventType = message.GetType();
                    List<Type> handlerTypes;

                    if (_eventTypesWithHandlers.TryGetValue(eventType, out handlerTypes))
                    {
                        _logger.Debug("Sending received event to " + handlerTypes.Count + " handlers.");

                        foreach (var handlerType in handlerTypes)
                        {
                            var handler = lifeTimeScope.Resolve(handlerType);

                            handler.AsDynamic().Handle(message);
                        }
                    }
                }
            }
        }
    }
}
