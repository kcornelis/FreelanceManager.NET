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

    public class ModelInvalidVersionException : Exception
    {
        public ModelInvalidVersionException(string type, Guid id, int currentVersion, int eventVersion)
            :base(string.Format("Changing {0} failed ({1}), current version = {2}, event version = {3}", type, id, currentVersion, eventVersion))
        {

        }
    }

    public class ModelDuplicateException : Exception
    {
        public ModelDuplicateException(string type, Guid id)
            : base(string.Format("Creating {0} failed ({1}), key already found.", type, id))
        {

        }
    }

    public class AggregateDeletedException : Exception
    {

    }
}
