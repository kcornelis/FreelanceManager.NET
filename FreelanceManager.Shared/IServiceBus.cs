using System;
using System.Collections.Generic;
using System.Reflection;

namespace FreelanceManager
{
    public interface IServiceBus : IDisposable
    {
        void PublishDomainUpdate(object[] events, DomainUpdateMetadate metadata);
        void RegisterHandlers(Assembly assembly);
        void Start(string name);
    }
}
