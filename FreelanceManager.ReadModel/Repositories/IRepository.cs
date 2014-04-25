using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FreelanceManager.ReadModel.Repositories
{
    public interface IRepository<T, TKey> : IQueryable<T>
        where T : Model
    {
        IQueryable<T> Context { get; }

        T GetById(TKey id);

        T Add(T entity);

        void Add(IEnumerable<T> entities);

        T Update(T entity);

        T Update(T entity, int newVersion);

        void Delete(TKey id, int versionToDelete);

        void Delete(T entity, int versionToDelete);

        long Count();
    }

    public interface IRepository<T> : IQueryable<T>, IRepository<T, Guid>
        where T : Model
    {
    }
}