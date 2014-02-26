using System;

namespace FreelanceManager.Events.TimeRegistration
{
    public class TimeRegistrationRateRefreshed : Event
    {
        public TimeRegistrationRateRefreshed(Guid id, Money rate)
        {
            Id = id;
            Rate = rate;
        }

        public Money Rate { get; private set; }
    }
}
