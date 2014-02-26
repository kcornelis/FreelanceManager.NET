using System;
using System.Linq;
using Autofac;
using EventStore.Dispatcher;
using NLog;

namespace FreelanceManager.Web.Shared
{
    public class MessageDispatcher : IDispatchCommits
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.EventStore);
        private readonly ILifetimeScope _container;

        public MessageDispatcher(ILifetimeScope container)
        {
            _container = container;
        }

        public void Dispatch(EventStore.Commit commit)
        {
            try
            {
                _logger.Debug("Dispatching commit, number of events: " + commit.Events.Count);

                var _bus = _container.Resolve<IServiceBus>();
                _bus.Send(BusEndpoints.ReadModelHandlers,
                    commit.Events.Select(e => e.Body).ToArray(),
                    commit.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
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