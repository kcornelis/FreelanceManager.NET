using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Reflection;
using System.Text;
using System.Transactions;
using Autofac;
using FreelanceManager.Tools;
using NLog;

namespace FreelanceManager.Infrastructure.ServiceBus
{
    public class MsmqMessageReceiver
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ServiceBus);
        private Dictionary<Type, List<Type>> _eventTypesWithHandlers = new Dictionary<Type, List<Type>>();
        private readonly IContainer _container;
        private readonly string _endpoint;
        private readonly string _queuePath;
        private MessageQueue _queue;
        private int _retryCount = 0;

        public MsmqMessageReceiver(IContainer container, string endpoint)
        {
            _container = container;
            _endpoint = endpoint;
            _queuePath = @".\Private$\" + endpoint;
        }

        public void Start()
        {
            // open or create message queue
            if (MessageQueue.Exists(_queuePath))
                _queue = new MessageQueue(_queuePath);
            else
                _queue = MessageQueue.Create(_queuePath, true);

            _queue.Formatter = new JsonMessageFormatter<BusMessage>();
            _queue.PeekCompleted += PeekCompleted;
            _queue.BeginPeek();
        }

        public void Stop()
        {
            _queue.Close();
            _queue.Dispose();
        }

        private void PeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            _queue.EndPeek(e.AsyncResult);

            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    Message message = null;
                    try
                    {
                        message = _queue.Receive(TimeSpan.FromSeconds(5), MessageQueueTransactionType.Automatic);
                    }
                    catch (MessageQueueException messageQueueException)
                    {
                        if (messageQueueException.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                        {
                            throw;
                        }
                    }

                    try
                    {
                        if (message != null)
                        {
                            _logger.Debug("Received new message.");

                            Process(message.Body as BusMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (_retryCount >= 5)
                        {
                            try
                            {
                                using (var errorQueue = new MessageQueue(@".\Private$\error"))
                                {
                                    errorQueue.Send(message, MessageQueueTransactionType.Automatic);
                                }
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

                    scope.Complete();

                    _retryCount = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error receiving message.", ex);
                _retryCount++;
            }

            _queue.BeginPeek();
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
