using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using FreelanceManager.Infrastructure.ServiceBus;
using FreelanceManager.Tools;
using NLog;

namespace FreelanceManager.Infrastructure
{
    public abstract class ServiceBusBase : IServiceBus
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ServiceBus);
        private readonly Dictionary<Type, List<Type>> _eventTypesWithHandlers = new Dictionary<Type, List<Type>>();
        private ILifetimeScope _container;

        public ServiceBusBase(ILifetimeScope container)
        {
            _container = container;
        }

        public abstract void Start(string name);
        public abstract void Dispose();
        protected abstract void Publish(BusMessage message);

        protected bool BusHasHandlers { get { return _eventTypesWithHandlers.Any(); } }

        public void Publish(object[] messages, Dictionary<string, string> headers)
        {
            _logger.Debug("Publishing " + messages.Length + " messages.");

            var busMessage = new BusMessage
            {
                Messages = messages.Select(m => JsonSerializer.Serialize(m)).ToArray(),
                Headers = headers
            };

            Publish(busMessage);
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

        protected void HandleBusMessage(BusMessage busMessage)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var hook = scope.ResolveOptional<IServiceBusMessageHandlerHook>();

                if (hook != null)
                    hook.PreHandleBusMessage(busMessage);

                var messages = busMessage.Messages.Select(m => JsonSerializer.Deserialize((string)m));

                foreach (var message in messages)
                {
                    _logger.Debug("Received message contains event with type " + message.GetType().Name);

                    if (hook != null)
                        hook.PreHandleMessage(message, busMessage.Headers);

                    try
                    {
                        var eventType = message.GetType();
                        List<Type> handlerTypes;

                        if (_eventTypesWithHandlers.TryGetValue(eventType, out handlerTypes))
                        {
                            _logger.Debug("Sending received event to " + handlerTypes.Count + " handlers.");

                            foreach (var handlerType in handlerTypes)
                            {
                                var handler = scope.Resolve(handlerType);

                                handler.AsDynamic().Handle(message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (hook != null)
                            hook.Exception(ex, message, busMessage.Headers);

                        throw;
                    }

                    if (hook != null)
                        hook.PostHandleMessage(message, busMessage.Headers);
                }

                if (hook != null)
                    hook.PostHandleBusMessage(busMessage);
            }
        }
    }
}
