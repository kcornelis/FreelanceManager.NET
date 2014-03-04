using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using FreelanceManager.Tools;

namespace FreelanceManager.Infrastructure
{
    // don't use this in production code, this is only good for testing
    public class InMemoryServiceBus : IServiceBus
    {        
        private readonly Dictionary<Type, List<Type>> _eventTypesWithHandlers = new Dictionary<Type, List<Type>>();
        private readonly IContainer _container;

        public InMemoryServiceBus(IContainer container)
        {
            _container = container;
        }

        public void Publish(object[] messages, Dictionary<string, string> headers)
        {
            Send(null, messages, headers);
        }

        public void Send(string endpoint, object[] messages, Dictionary<string, string> headers)
        {
            // NOTE: we should set the tenant context here if the message 
            // infrastructure get's decoupled from the web

            foreach (var message in messages)
            {
                var eventType = message.GetType();
                List<Type> handlerTypes;

                if (_eventTypesWithHandlers.TryGetValue(eventType, out handlerTypes))
                {
                    foreach (var handlerType in handlerTypes)
                    {
                        var handler = _container.Resolve(handlerType);

                        handler.AsDynamic().Handle(message);
                    }
                }
            }
        }

        public void RegisterHandlersFromAssembly(Assembly assembly)
        {
            foreach(var type in assembly.GetTypes())
            {
                foreach(var handlerInterfaceType in type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleEvent<>)))
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
    }
}
