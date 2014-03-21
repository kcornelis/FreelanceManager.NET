using System;
using System.Collections.Generic;
using System.Reflection;

namespace FreelanceManager
{
    public interface IServiceBus : IDisposable
    {
        void PublishDomainUpdate(object @event, DomainUpdateMetadate metadata);
        void RegisterHandlers(Assembly assembly);
        void Start(string name);
    }
}
