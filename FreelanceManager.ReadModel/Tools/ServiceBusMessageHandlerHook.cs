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

        public void PreHandle(object @event, DomainUpdateMetadate metadata)
        {
            _tenantContext.SetTenantId(metadata.Tenant);

            _info = _collection.FindOneByIdAs<ReadModelInfo>(BsonValue.Create(metadata.AggregateId));

            if (_info == null)
            {
                _info = CreateNewInfo(metadata);
                _collection.Insert(_info);
            }

            if (_info.Version != (metadata.Version - 1))
                throw new InvalidVersionException(metadata.AggregateType, metadata.AggregateId, _info.Version, metadata.Version);
        }

        public void PostHandle(object @event, DomainUpdateMetadate metadata)
        {
            _info.Errors = 0;
            _info.Version = metadata.Version;

            _collection.Save(_info);
        }

        public void Exception(Exception ex, object @event, DomainUpdateMetadate metadata)
        {
            // todo rebuild if errors = 5
            _info.Errors += 1;

            _collection.Save(_info);
        }
    }
}
