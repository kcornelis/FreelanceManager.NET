using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FreelanceManager.ReadModel.Tools
{
    public class DomainUpdateServiceBusHandlerHook : IDomainUpdateServiceBusHandlerHook
    {
        private MongoCollection<ReadModelInfo> _collection;
        private ITenantContext _tenantContext;
        private ReadModelInfo _info;

        public DomainUpdateServiceBusHandlerHook(IMongoContext mongoContext, ITenantContext tenantContext)
        {
            _collection = mongoContext.GetDatabase().GetCollection<ReadModelInfo>("ReadModelInfo");
            _tenantContext = tenantContext;
        }

        private ReadModelInfo CreateNewInfo(DomainUpdateMetadate metadata)
        {
            return new ReadModelInfo
            {
                Id = metadata.AggregateId,
                Tenant = metadata.Tenant,
                Type = metadata.AggregateType
            };
        }

        public void PreHandle(object[] events, DomainUpdateMetadate metadata)
        {
            _tenantContext.SetTenantId(metadata.Tenant);

            _info = _collection.FindOneByIdAs<ReadModelInfo>(BsonValue.Create(metadata.AggregateId));

            if (_info == null)
            {
                _info = CreateNewInfo(metadata);
                _collection.Insert(_info);
            }

            if (_info.Locked != null && _info.Locked.Value.AddSeconds(5) > DateTime.Now)
                throw new Exception("Item locked");

            if (_info.Version != (metadata.LastVersion - events.Length))
                throw new InvalidVersionException(metadata.AggregateType, metadata.AggregateId, _info.Version, metadata.LastVersion);

            _info.Locked = DateTime.Now;
            _collection.Save(_info);
        }

        public void PostHandle(object[] events, DomainUpdateMetadate metadata)
        {
            _info.Errors = 0;
            _info.Version = metadata.LastVersion;
            _info.Locked = null;

            _collection.Save(_info);
        }

        public void PreHandleEvent(object @event, DomainUpdateMetadate metadata)
        {
        }

        public void PostHandleEvent(object @event, DomainUpdateMetadate metadata)
        {
        }

        public void Exception(Exception ex, object[] events, DomainUpdateMetadate metadata)
        {
            // todo rebuild if errors = 5
            _info.Errors += 1;
            _info.Locked = null;

            _collection.Save(_info);
        }
    }
}
