using System;
using System.Collections.Generic;
using FreelanceManager.Events;
using FreelanceManager.Tools;

namespace FreelanceManager
{
    public interface IEntity<TKey>
    {
        TKey Id { get; }
    }

    public interface IEntity : IEntity<Guid>
    {
    }

    public abstract class AggregateRoot : IEntity, IAggregateRoot
    {
        private int? _cachedHashcode;
        private const int HASH_MULTIPLIER = 31;
        private readonly List<Event> _changes = new List<Event>();

        public Guid Id { get; protected set; }
        public DateTime CreatedOn { get; protected set; }
        public int Version { get; private set; }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history) 
                ApplyChange(e, false);
        }

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

        private void ApplyChange(Event @event, bool isNew)
        {
            this.AsDynamic().Apply(@event);

            Version += 1;

            if (isNew)
            {
                _changes.Add(@event);
            }
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as AggregateRoot;

            if (ReferenceEquals(this, compareTo))
                return true;

            if (compareTo == null)
                return false;

            return Id != null &&
                   compareTo.Id != null &&
                   Id.Equals(compareTo.Id);
        }

        public override int GetHashCode()
        {
            if (_cachedHashcode.HasValue)
                return _cachedHashcode.Value;

            if (Id == null)
            {
                _cachedHashcode = base.GetHashCode();
            }
            else
            {
                unchecked
                {
                    // It's possible for two objects to return the same hash code based on 
                    // identically valued properties, even if they're of two different types, 
                    // so we include the object's type in the hash calculation
                    int hashCode = GetType().GetHashCode();
                    _cachedHashcode = (hashCode * HASH_MULTIPLIER) ^ Id.GetHashCode();
                }
            }

            return _cachedHashcode.Value;
        }

        public static bool operator ==(AggregateRoot e1, AggregateRoot e2)
        {
            if (ReferenceEquals(e1, null))
                return ReferenceEquals(e2, null);

            return e1.Equals(e2);
        }

        public static bool operator !=(AggregateRoot e1, AggregateRoot e2)
        {
            return !(e1 == e2);
        }
    }
}
