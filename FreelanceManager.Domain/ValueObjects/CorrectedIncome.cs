using System;

namespace FreelanceManager.Domain.ValueObjects
{
    public class CorrectedIncome
    {
        public CorrectedIncome(Money amount, string message)
        {
            Total = amount;
            Message = message;
        }

        public Money Total
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }
    }
}
