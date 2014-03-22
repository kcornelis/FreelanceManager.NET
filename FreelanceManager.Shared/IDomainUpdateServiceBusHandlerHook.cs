using System;
using System.Collections.Generic;

namespace FreelanceManager
{
    public interface IDomainUpdateServiceBusHandlerHook
    {
        void PreHandle(object[] events, DomainUpdateMetadate metadata);
        void PostHandle(object[] events, DomainUpdateMetadate metadata);

        void PreHandleEvent(object @event, DomainUpdateMetadate metadata);
        void PostHandleEvent(object @event, DomainUpdateMetadate metadata);

        void Exception(Exception ex, object[] events, DomainUpdateMetadate metadata);
    }
}
