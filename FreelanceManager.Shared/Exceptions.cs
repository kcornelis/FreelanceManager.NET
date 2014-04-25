using System;

namespace FreelanceManager
{
    public class ModelNotFoundException : Exception
    {

    }

    public class DatabaseException : Exception
    {
        public DatabaseException(string message)
            :base(string.Format("The operation failed with {0}", message))
        {

        }
    }

    public class ModelLockedException : Exception
    {
        public ModelLockedException(string type, Guid id)
            :base(string.Format("Model {0} locked ({1})", type, id))
        {

        }
    }

    public class InvalidVersionException : Exception
    {
        public InvalidVersionException(string type, Guid id, int currentVersion, int eventVersion)
            :base(string.Format("Updating {0} failed ({1}), current version = {2}, event version = {3}", type, id, currentVersion, eventVersion))
        {

        }
    }

    public class AggregateDeletedException : Exception
    {

    }

    public class ConcurrencyException : Exception
    {

    }
}
