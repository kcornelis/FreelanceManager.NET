using System.Collections.Generic;

namespace FreelanceManager
{
    public interface IServiceBus
    {
        void Publish(object[] messages, Dictionary<string, string> headers);
        void Send(string endpoint, object[] messages, Dictionary<string, string> headers);
    }
}
