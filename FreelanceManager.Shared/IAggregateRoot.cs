using System;
using System.Collections.Generic;
using FreelanceManager.Events;

namespace FreelanceManager
{
    public interface IAggregateRoot
    {
        Guid Id { get; }
        int Version { get; }
        IEnumerable<Event> GetUncommittedChanges();
        void MarkChangesAsCommitted();
        void LoadFromHistory(IEnumerable<Event> history);
    }
}
