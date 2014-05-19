using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace FreelanceManager.ReadModel.Repositories
{
    public abstract class Repository<T, TKey> : IRepository<T, TKey>
        where T : Model
    {
        private MongoCollection<T> _collection;
        private ITenantContext _tenantContext;
        private static bool _initialized;
        private object _lock = new object();

        public Repository(IMongoContext mongoContext, ITenantContext tenantContext)
        {
            _collection = mongoContext.GetDatabase().GetCollection<T>(GetCollectionName());
            _tenantContext = tenantContext;

            if (!_initialized)
            {
                lock (_lock)
                {
                    if (!_initialized)
                    {
                        CreateIndexes(_collection);
                        _initialized = true;
                    }
                }
            }
        }

        protected virtual void CreateIndexes(MongoCollection<T> collection)
        {
            collection.EnsureIndex(IndexKeys.Ascending("Tenant"), IndexOptions.SetName("IX_Tenant"));
        }

        protected string CurrentTenant
        {
            get { return _tenantContext.GetTenantId(); }
        }

        protected bool TenantIndependant
        {
            get;
            set;
        }

        private bool VerifyTenant(string tenant)
        {
            if(TenantIndependant)
            {
                return true;
            }
            else
            {
                return string.Equals(tenant, CurrentTenant, StringComparison.OrdinalIgnoreCase);
            }
        }

        protected abstract string GetCollectionName();

        private MongoCollection<T> Collection
        {
            get
            {
                return _collection;
            }
        }

        public virtual IQueryable<T> Context
        {
            get
            {
                if(TenantIndependant)
                    return Collection.AsQueryable<T>().Where(e => e.Tenant == null || e.Tenant == "");
                else
                    return Collection.AsQueryable<T>().Where(e => e.Tenant == CurrentTenant);
            }
        }

        public virtual T GetById(TKey id)
        {
            T entity = default(T);

            if (id is string)
            {
                entity = Collection.FindOneByIdAs<T>(new ObjectId(id as string));
            }
            else
            {
                entity = Collection.FindOneByIdAs<T>(BsonValue.Create(id));
            }

            if (entity != null && !VerifyTenant(entity.Tenant))
                return default(T);

            return entity;
        }

        public virtual T Add(T entity, int version)
        {
            if(!TenantIndependant)
                entity.Tenant = CurrentTenant;

            // a new entity should always have version 1
            if (version != 1)
                throw new InvalidVersionException(entity.GetType().Name, entity.Id, entity.Version, 1);

            entity.Version = 1;

            try
            {
                Collection.Insert<T>(entity);
            }
            catch (WriteConcernException ex)
            {
                throw new DatabaseException(ex.Message);
            }

            return entity;
        }

        /// <summary>
        /// Update the entity but don't touch the version.
        /// </summary>
        public virtual T Update(T entity)
        {
            return InternalUpdate(entity, null);
        }

        /// <summary>
        /// Verify the version and update the entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="newVersion"></param>
        /// <returns></returns>
        public virtual T Update(T entity, int newVersion)
        {
            return InternalUpdate(entity, newVersion);
        }

        private T InternalUpdate(T entity, int? newVersion)
        {
            if (entity != null && !VerifyTenant(entity.Tenant))
                return entity;

            if (newVersion.HasValue)
            {
                if (entity.Version + 1 != newVersion.Value)
                    throw new InvalidVersionException(entity.GetType().Name, entity.Id, entity.Version, newVersion.Value);
            }

            var originalDBVersion = entity.Version;
            entity.Version = newVersion.HasValue ? newVersion.Value : entity.Version;

            IMongoQuery versionCheck = Query.And(Query.EQ("_id", entity.Id), Query.EQ("Version", originalDBVersion));

            var wrap = new BsonDocumentWrapper(entity);
            var document = new UpdateDocument(wrap.ToBsonDocument());
            WriteConcernResult res = Collection.Update(versionCheck, document);

            if (res.DocumentsAffected == 1 && res.Ok)
            {
                return entity;
            }

            if (!res.Ok)
            {
                throw new Exception(res.ErrorMessage);
            }

            bool isConcurrencyError = Collection.Find(Query.EQ("_id", entity.Id)).Any();
            if (isConcurrencyError)
            {
                throw new ConcurrencyException();
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public virtual void Delete(TKey id, int versionToDelete)
        {
            if (id is string)
            {
                Collection.Remove(Query.And(Query.EQ("_id", new ObjectId(id as string)), Query.EQ("Tenant", CurrentTenant)));
            }
            else
            {
                Collection.Remove(Query.And(Query.EQ("_id", BsonValue.Create(id)), Query.EQ("Tenant", CurrentTenant)));
            }
        }

        public virtual void Delete(ObjectId id, int versionToDelete)
        {
            Collection.Remove(Query.And(Query.EQ("_id", id), Query.EQ("Tenant", CurrentTenant)));
        }

        public void Delete(T entity, int versionToDelete)
        {
            Delete(((dynamic)entity).Id, versionToDelete);
        }

        public virtual long Count()
        {
            if (TenantIndependant)
                return Collection.Count();

            return Collection.Count(Query.EQ("Tenant", CurrentTenant));
        }

        #region IQueryable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return Context.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Context.GetEnumerator();
        }

        public Type ElementType
        {
            get { return Context.ElementType; }
        }

        public Expression Expression
        {
            get { return Context.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return Context.Provider; }
        }

        #endregion
    }

    public abstract class Repository<T> : Repository<T, Guid>, IRepository<T>
        where T : Model
    {
        public Repository(IMongoContext context, ITenantContext tenantContext)
            : base(context, tenantContext)
        {
        }
    }
}