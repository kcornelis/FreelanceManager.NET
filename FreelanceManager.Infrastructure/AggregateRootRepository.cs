using System;
using System.Collections.Generic;
using System.Linq;
using EventStore;
using FreelanceManager.Events;
using NLog;

namespace FreelanceManager.Infrastructure
{
    public class AggregateRootRepository : IAggregateRootRepository, IDisposable
    {
        private static readonly Logger _logger = LogManager.GetLogger(Loggers.EventStore);

        private readonly ITenantContext _tenantContext;
        private readonly IStoreEvents _eventStore;
        private readonly IIdGenerator _idGenerator;

        private readonly IDictionary<Guid, IEventStream> _streams = new Dictionary<Guid, IEventStream>();

        public AggregateRootRepository(IStoreEvents eventStore, ITenantContext tenantContext, IIdGenerator idGenerator)
        {
            _tenantContext = tenantContext;
            _eventStore = eventStore;
            _idGenerator = idGenerator;
        }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			lock (_streams)
			{
				foreach (var stream in _streams)
					stream.Value.Dispose();

				_streams.Clear();
			}
		}

	    public virtual T GetById<T>(Guid id) where T : IAggregateRoot, new()
		{
			var stream = this.OpenStream(id);
            var aggregate = new T();

			ApplyEventsToAggregate(stream, aggregate);

			return (T)aggregate;
		}

        private static void ApplyEventsToAggregate(IEventStream stream, IAggregateRoot aggregate)
        {
            aggregate.LoadFromHistory(stream.CommittedEvents.Select(x => x.Body).Cast<Event>());
        }

		private IEventStream OpenStream(Guid id)
		{
			IEventStream stream;
			if (_streams.TryGetValue(id, out stream))
				return stream;

            stream = _eventStore.OpenStream(id, 0, int.MaxValue);

			return _streams[id] = stream;
		}

        public virtual void Save(IAggregateRoot aggregate)
        {
            var commit = _idGenerator.NextGuid();
            Save(aggregate, commit);
        }

        public virtual void Save(IAggregateRoot aggregate, Guid commitId)
        {
            var headers = PrepareHeaders(aggregate);

            var stream = PrepareStream(aggregate, headers);
            var commitEventCount = stream.CommittedEvents.Count;

            try
            {
                stream.CommitChanges(commitId);
                aggregate.MarkChangesAsCommitted();
            }
            catch (DuplicateCommitException)
            {
                stream.ClearChanges();
            }
        }

        private IEventStream PrepareStream(IAggregateRoot aggregate, Dictionary<string, object> headers)
		{
			IEventStream stream;
			if (!_streams.TryGetValue(aggregate.Id, out stream))
                _streams[aggregate.Id] = stream = _eventStore.CreateStream(aggregate.Id);

			foreach (var item in headers)
				stream.UncommittedHeaders[item.Key] = item.Value;

            var changes = aggregate.GetUncommittedChanges();

            changes.Select(x => new EventMessage { Body = x })
				   .ToList()
				   .ForEach(stream.Add);

			return stream;
		}

        private Dictionary<string, object> PrepareHeaders(IAggregateRoot aggregate)
		{
			var headers = new Dictionary<string, object>();

            headers[AggregateRootMetadata.AggregateTypeHeader] = aggregate.GetType().FullName;
            headers[AggregateRootMetadata.TenantHeader] = _tenantContext.GetTenantId();
            headers[AggregateRootMetadata.AggregateIdHeader] = aggregate.Id.ToString();

			return headers;
		}
    }
}
