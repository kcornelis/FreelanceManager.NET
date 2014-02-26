using System;

namespace FreelanceManager.Events.TimeRegistration
{
    public class TimeRegistrationCorrectedIncomeCleared : Event
    {
        public TimeRegistrationCorrectedIncomeCleared(Guid id)
        {
            Id = id;
        }
    }
}
