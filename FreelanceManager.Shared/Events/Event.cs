using System;

namespace FreelanceManager.Events
{
    public abstract class Event : IEvent
    {
        public Guid Id { get; protected set; }
        public int Version { get; internal set; }
    }
}
