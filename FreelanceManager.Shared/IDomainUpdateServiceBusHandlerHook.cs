using System;
using System.Collections.Generic;

namespace FreelanceManager
{
    public interface IDomainUpdateServiceBusHandlerHook
    {
        void PreHandle(object @event, DomainUpdateMetadate metadata);
        void PostHandle(object @event, DomainUpdateMetadate metadata);
        void Exception(Exception ex, object @event, DomainUpdateMetadate metadata);
    }
}
