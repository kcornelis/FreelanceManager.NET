using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NLog;

namespace FreelanceManager.Infrastructure.ServiceBus
{
    public class AzureMessageSender
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ServiceBus);

        // Thread-safe. Recommended that you cache rather than recreating it
        // on every request.
        private QueueClient _queue;

        // Obtain these values from the Management Portal
        private readonly string _namespace;
        private readonly string _issuerName;
        private readonly string _issuerKey;

        // The name of your queue
        private readonly string _queueName;

        public AzureMessageSender(string endpoint)
        {
            _queueName = endpoint;
            _namespace = ConfigurationManager.AppSettings["azure:serviceBusNamespace"];
            _issuerName = ConfigurationManager.AppSettings["azure:serviceBusIssuerName"];
            _issuerKey = ConfigurationManager.AppSettings["azure:serviceBusIssuerKey"];

            Initialize();
        }

        private NamespaceManager CreateNamespaceManager()
        {
            // Create the namespace manager which gives you access to management operations
            var uri = ServiceBusEnvironment.CreateServiceUri("sb", _namespace, String.Empty);
            var tP = TokenProvider.CreateSharedSecretTokenProvider(_issuerName, _issuerKey);
            return new NamespaceManager(uri, tP);
        }

        private void Initialize()
        {
            // Using Http to be friendly with outbound firewalls
            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Http;

            // Create the namespace manager which gives you access to management operations
            var namespaceManager = CreateNamespaceManager();

            // Create the queue if it does not exist already
            if (!namespaceManager.QueueExists(_queueName))
            {
                namespaceManager.CreateQueue(_queueName);
            }

            // Get a client to the queue
            var messagingFactory = MessagingFactory.Create(namespaceManager.Address, namespaceManager.Settings.TokenProvider);
            
            _queue = messagingFactory.CreateQueueClient(_queueName);
        }

        public void Send(object[] messages, Dictionary<string, string> headers)
        {
            var message = new BrokeredMessage(new BusMessage
            {
                Headers = headers,
                Messages = messages
            });

            _queue.Send(message);
        }
    }
}
