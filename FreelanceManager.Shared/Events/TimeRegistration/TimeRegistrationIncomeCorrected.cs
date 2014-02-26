using System;

namespace FreelanceManager.Events.TimeRegistration
{
    public class TimeRegistrationIncomeCorrected : Event
    {
        public TimeRegistrationIncomeCorrected(Guid id, Money amount, string message)
        {
            Id = id;
            Amount = amount;
            Message = message;
        }

        public Money Amount { get; private set; }
        public string Message { get; private set; }
    }
}
