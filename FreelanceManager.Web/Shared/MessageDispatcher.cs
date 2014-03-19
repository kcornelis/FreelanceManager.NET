using System;
using System.Configuration;
using System.Linq;
using Autofac;
using EventStore.Dispatcher;
using NLog;

namespace FreelanceManager.Web.Shared
{
    public class MessageDispatcher : IDispatchCommits
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.EventStore);
        private static readonly string _endpoint = ConfigurationManager.AppSettings["serviceBusEndpoint"];
        private readonly ILifetimeScope _container;

        public MessageDispatcher(ILifetimeScope container)
        {
            _container = container;
        }

        public void Dispatch(EventStore.Commit commit)
        {
            try
            {
                using (var scope = _container.BeginLifetimeScope())
                {
                    var _bus = scope.Resolve<IServiceBus>();

                    if (_logger.IsDebugEnabled)
                    {
                        _logger.Debug("Dispatching commit, number of events: " + commit.Events.Count);
                    }

                    var @events = commit.Events.Select(e => e.Body).ToArray();
                    var headers = commit.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
                    headers.Add("ApplicationService", _endpoint);
                    headers.Add("MessageType", "DomainEvent");
                    headers.Add("LastEventRevision", commit.StreamRevision.ToString());

                    _bus.Publish(@events, headers);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Could not dispatch message.", ex);
                throw;
            }
        }

        public void Dispose()
        {
        }
    }
}