using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelanceManager.Infrastructure
{
    public class AzureServiceBus : IServiceBus
    {
        public void RegisterReceiver(string endpoint)
        {
            throw new NotImplementedException();
        }

        public void Publish(object[] messages, Dictionary<string, string> headers)
        {
            
        }

        public void Send(string endpoint, object[] messages, Dictionary<string, string> headers)
        {
            
        }


    }
}
