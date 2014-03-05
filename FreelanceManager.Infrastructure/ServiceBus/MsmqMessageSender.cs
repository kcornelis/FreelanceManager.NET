using System.Collections.Generic;
using System.Messaging;
using NLog;

namespace FreelanceManager.Infrastructure.ServiceBus
{
    internal class MsmqMessageSender
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.ServiceBus);
        private readonly string _endpoint;
        private readonly string _queuePath;

        public MsmqMessageSender(string endpoint)
        {
            _endpoint = endpoint;
            _queuePath = @".\Private$\" + endpoint;

            if (!MessageQueue.Exists(_queuePath))
            {
                _logger.Info("Creating queue " + _queuePath);

                using (MessageQueue.Create(_queuePath, true)) { }
            }
        }

        public void Send(object[] messages, Dictionary<string, string> headers)
        {
            using (var mq = new MessageQueue(_queuePath))
            {
                Message m = new Message();
                m.Formatter = new JsonMessageFormatter<BusMessage>();
                m.Recoverable = true;
                m.Body = new BusMessage
                {
                    Headers = headers,
                    Messages = messages
                };

                mq.Send(m, MessageQueueTransactionType.Automatic);
            }
        }
    }
}
