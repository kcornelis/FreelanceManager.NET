using System;

namespace FreelanceManager.Events.TimeRegistration
{
    public class TimeRegistrationDeleted: Event
    {
        public TimeRegistrationDeleted(Guid id, DateTime deletedOn)
        {
            Id = id;
            DeletedOn = deletedOn;
        }
 
        public DateTime DeletedOn { get; private set; }
    }
}
