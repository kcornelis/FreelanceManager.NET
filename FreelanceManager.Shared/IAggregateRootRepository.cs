using System;

namespace FreelanceManager
{
    public interface IAggregateRootRepository
    {
        T GetById<T>(Guid id) where T : IAggregateRoot, new();
        void Save(IAggregateRoot aggregate, Guid commitId);
        void Save(IAggregateRoot aggregate);
    }
}
