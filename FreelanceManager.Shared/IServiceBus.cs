using System;
using System.Collections.Generic;
using System.Reflection;

namespace FreelanceManager
{
    public interface IServiceBus : IDisposable
    {
        void Send(string endpoint, object[] messages, Dictionary<string, string> headers);
        void Publish(object[] messages, Dictionary<string, string> headers);
        void RegisterHandlers(Assembly assembly);
        void Start(string name);
    }
}
