using System;
using System.Collections.Generic;
using System.Reflection;
using System.Transactions;
using Autofac;
using FreelanceManager.Infrastructure.ServiceBus;
using NLog;

namespace FreelanceManager.Bus
{
    public class MsmqServiceBus : IServiceBus
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ServiceBus);
        private readonly Dictionary<string, MsmqMessageSender> _senders = new Dictionary<string, MsmqMessageSender>();
        private readonly List<string> _publishTo = new List<string>();
        private readonly IContainer _container;
        private MessageReceiver _receiver;
        private object _lock = new object();

        public MsmqServiceBus() { }

        public MsmqServiceBus(IContainer container)
        {
            _container = container;
        }

        public void RegisterPublishTo(string endpoint)
        {
            if (!_publishTo.Contains(endpoint))
            {
                lock (_lock)
                {
                    if (!_publishTo.Contains(endpoint))
                    {
                        _publishTo.Add(endpoint);
                    }
                }
            }
        }

        public void Publish(object[] messages, Dictionary<string, string> headers)
        {
            _logger.Trace("Publishing " + messages.Length + " messages.");

            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    foreach (var endpoint in _publishTo)
                    {
                        GetSender(endpoint).Send(messages, headers);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while publishing a message.", ex);
                throw;
            }
        }

        public void Send(string endpoint, object[] messages, Dictionary<string, string> headers)
        {
            _logger.Trace("Sending message to " + endpoint);

            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    GetSender(endpoint).Send(messages, headers); 

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while sending a message.", ex);
                throw;
            }
        }

        private MsmqMessageSender GetSender(string endpoint)
        {
            if (!_senders.ContainsKey(endpoint))
            {
                lock (_lock)
                {
                    if (!_senders.ContainsKey(endpoint))
                    {
                        _senders.Add(endpoint, new MsmqMessageSender(endpoint));
                    }
                }
            }

            return _senders[endpoint];
        }

        public void Start()
        {
            _logger.Info("Starting servicebus, not listening to any enpoint.");
        }

        public void Start(string listenTo, Assembly handlers)
        {
            _logger.Info("Starting servicebus, listening to " + listenTo);

            _receiver = new MessageReceiver(_container, listenTo);
            _receiver.RegisterHandlersFromAssembly(handlers);
            _receiver.Start();
        }
    }
}
