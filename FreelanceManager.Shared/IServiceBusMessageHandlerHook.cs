using System;
using System.Collections.Generic;

namespace FreelanceManager
{
    public interface IServiceBusMessageHandlerHook
    {
        void PreHandleBusMessage(BusMessage busMessage);
        void PostHandleBusMessage(BusMessage busMessage);
        void PreHandleMessage(object message, Dictionary<string, string> headers);
        void PostHandleMessage(object message, Dictionary<string, string> headers);
        void Exception(Exception ex, object message, Dictionary<string, string> headers);
    }
}
