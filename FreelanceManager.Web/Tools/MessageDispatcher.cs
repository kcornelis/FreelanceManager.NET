using System;
using System.Configuration;
using System.Linq;
using Autofac;
using EventStore.Dispatcher;
using NLog;

namespace FreelanceManager.Web.Tools
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

                    _bus.PublishDomainUpdate(commit.Events.Select(e => e.Body).ToArray(),
                        new DomainUpdateMetadate
                        {
                            AggregateId = Guid.Parse(commit.Headers[AggregateRootMetadata.AggregateIdHeader] as string),
                            AggregateType = commit.Headers[AggregateRootMetadata.AggregateTypeHeader] as string,
                            Tenant = commit.Headers[AggregateRootMetadata.TenantHeader] as string,
                            ApplicationService = _endpoint,
                            LastVersion = commit.StreamRevision
                        });
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