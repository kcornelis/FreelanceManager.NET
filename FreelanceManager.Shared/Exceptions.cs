using System;

namespace FreelanceManager
{
    public class AggregateNotFoundException : Exception
    {
    }

    public class ConcurrencyException : Exception
    {
    }

    public class SecurityException : Exception
    {

    }

    public class ModelNotFoundException : Exception
    {

    }

    public class InvalidVersionException : Exception
    {
        public InvalidVersionException(string type, Guid id, int currentVersion, int eventVersion)
            :base(string.Format("Handling {0} failed ({1}), current version = {2}, event version = {3}", type, id, currentVersion, eventVersion))
        {

        }
    }

    public class MessageAlreadyHandledException : Exception
    {
        public MessageAlreadyHandledException(string type, string id)
            : base(string.Format("Message for {0} already handled ({1})", type, id))
        {

        }
    }

    public class AggregateDeletedException : Exception
    {

    }
}
