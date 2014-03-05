using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NLog;
using FreelanceManager.Tools;

namespace FreelanceManager.Infrastructure.ServiceBus
{
    public class AzureMessageReceiver
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ServiceBus);
        private Dictionary<Type, List<Type>> _eventTypesWithHandlers = new Dictionary<Type, List<Type>>();
        private readonly IContainer _container;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        // Thread-safe. Recommended that you cache rather than recreating it
        // on every request.
        private QueueClient _queue;

        // Obtain these values from the Management Portal
        private readonly string _namespace;
        private readonly string _issuerName;
        private readonly string _issuerKey;

        // The name of your queue
        private readonly string _queueName;

        private int _retryCount = 0;

        public AzureMessageReceiver(IContainer container, string endpoint)
        {
            _container = container;

            _queueName = endpoint;
            _namespace = ConfigurationManager.AppSettings["azure:serviceBusNamespace"];
            _issuerName = ConfigurationManager.AppSettings["azure:serviceBusIssuerName"];
            _issuerKey = ConfigurationManager.AppSettings["azure:serviceBusIssuerKey"];
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

        public void Start()
        {
            Initialize();

            _logger.Info("Starting processing of messages");

            _queue.OnMessage((receivedMessage) =>
            {
                try
                {
                    var message = receivedMessage.GetBody<BusMessage>();
                        
                        try
                        {
                            if (message != null)
                            {
                                _logger.Debug("Received new message.");

                                Process(message);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (_retryCount >= 5)
                            {
                                try
                                {
                                    //using (var errorQueue = new MessageQueue(@".\Private$\error"))
                                    //{
                                    //    errorQueue.Send(message, MessageQueueTransactionType.Automatic);
                                    //}
                                }
                                catch (Exception ex2)
                                {
                                    _logger.Error("Error moving faulted message.", ex2);
                                    throw;
                                }
                            }
                            else
                            {
                                throw;
                            }
                        }

                        _retryCount = 0;
                    }
            
                catch (Exception ex)
                {
                    _logger.Error("Error receiving message.", ex);
                    _retryCount++;
                }
            });

            CompletedEvent.WaitOne();
        }

        public void Stop()
        {
            _queue.Close();
            CompletedEvent.Set();
        }


        private void Process(BusMessage message)
        {
            if (message != null)
            {
                foreach (var @event in message.Messages)
                {
                    _logger.Debug("Message contains event with type " + @event.GetType().Name);

                    // todo rethink about the admin parameter
                    _container.Resolve<ITenantContext>().SetTenantId(message.Headers["Tenant"], false);

                    var eventType = @event.GetType();
                    List<Type> handlerTypes;

                    if (_eventTypesWithHandlers.TryGetValue(eventType, out handlerTypes))
                    {
                        _logger.Debug("Sending event to " + handlerTypes.Count + " handlers.");

                        foreach (var handlerType in handlerTypes)
                        {
                            var handler = _container.Resolve(handlerType);

                            handler.AsDynamic().Handle(@event);
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void RegisterHandlersFromAssembly(Assembly assembly)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Handlers")) // todo: implements ihandleevent
                .AsSelf();

            builder.Update(_container.ComponentRegistry);

            foreach (var type in assembly.GetTypes())
            {
                foreach (var handlerInterfaceType in type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleEvent<>)))
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
