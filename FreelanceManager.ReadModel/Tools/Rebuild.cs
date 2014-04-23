using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using EventStore;
using FreelanceManager.Tools;

namespace FreelanceManager.ReadModel.Tools
{
    public interface IRebuild
    {
        void FromEventStore();
    }

    public class Rebuild : IRebuild
    {
        private readonly Dictionary<Type, List<Type>> _eventTypesWithHandlers = new Dictionary<Type, List<Type>>();
        private IStoreEvents _eventStore;
        private IDomainUpdateServiceBusHandlerHook _hook;
        private ILifetimeScope _container;
        private IMongoContext _mongo;

        public Rebuild(IStoreEvents eventStore, IDomainUpdateServiceBusHandlerHook hook, ILifetimeScope container, IMongoContext mongo)
        {
            _eventStore = eventStore;
            _hook = hook;
            _container = container;
            _mongo = mongo;
        }

        public void FromEventStore()
        {
            _mongo.GetDatabase()
                  .Drop();

            RegisterHandlers(typeof(Rebuild).Assembly);

            foreach (var commit in _eventStore.Advanced.GetFrom(DateTime.MinValue)
                .OrderBy(c => c.CommitStamp))
            {
                var metadata = new DomainUpdateMetadate
                {
                    AggregateId = Guid.Parse(commit.Headers[AggregateRootMetadata.AggregateIdHeader] as string),
                    AggregateType = commit.Headers[AggregateRootMetadata.AggregateTypeHeader] as string,
                    Tenant = commit.Headers[AggregateRootMetadata.TenantHeader] as string,
                    ApplicationService = "Rebuilder",
                    LastVersion = commit.StreamRevision
                };

                HandleDomainUpdate(commit.Events.Select(e => e.Body).ToArray(), metadata);
            }
        }

        private void RegisterHandlers(Assembly assembly)
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

        private void HandleDomainUpdate(object[] events, DomainUpdateMetadate metadata)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var hook = scope.ResolveOptional<IDomainUpdateServiceBusHandlerHook>();

                if (hook != null)
                    hook.PreHandle(events, metadata);

                try
                {
                    foreach (var @event in events)
                    {
                        if (hook != null)
                            hook.PreHandleEvent(@event, metadata);

                        var eventType = @event.GetType();
                        List<Type> handlerTypes;

                        if (_eventTypesWithHandlers.TryGetValue(eventType, out handlerTypes))
                        {
                            foreach (var handlerType in handlerTypes)
                            {
                                var handler = scope.Resolve(handlerType);

                                handler.AsDynamic().Handle(@event);
                            }
                        }

                        if (hook != null)
                            hook.PostHandleEvent(@event, metadata);
                    }

                    if (hook != null)
                        hook.PostHandle(events, metadata);
                }
                catch (Exception ex)
                {
                    if (hook != null)
                        hook.Exception(ex, events, metadata);

                    throw;
                }
            }
        }
    }
}
