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
        protected abstract void Publish(DomainUpdateBusMessage message);

        public void PublishDomainUpdate(object @event, DomainUpdateMetadate metadata)
        {
            _logger.Debug("Publishing a domain update for " + metadata.AggregateType + ", " + @event.GetType().Name);

            Publish(new DomainUpdateBusMessage
            {
                Event = @event,
                Metadata = metadata
            });
        }

        protected bool BusHasHandlers { get { return _eventTypesWithHandlers.Any(); } }

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

        protected void HandleDomainUpdate(DomainUpdateBusMessage domainUpdate)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var hook = scope.ResolveOptional<IDomainUpdateServiceBusHandlerHook>();

                _logger.Debug("Received message contains event with type " + domainUpdate.Event.GetType().Name);

                if (hook != null)
                    hook.PreHandle(domainUpdate.Event, domainUpdate.Metadata);

                try
                {
                    var eventType = domainUpdate.Event.GetType();
                    List<Type> handlerTypes;

                    if (_eventTypesWithHandlers.TryGetValue(eventType, out handlerTypes))
                    {
                        _logger.Debug("Sending received event to " + handlerTypes.Count + " handlers.");

                        foreach (var handlerType in handlerTypes)
                        {
                            var handler = scope.Resolve(handlerType);

                            handler.AsDynamic().Handle(domainUpdate.Event);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (hook != null)
                        hook.Exception(ex, domainUpdate.Event, domainUpdate.Metadata);

                    throw;
                }

                if (hook != null)
                    hook.PostHandle(domainUpdate.Event, domainUpdate.Metadata);
            }
        }
    }
}
