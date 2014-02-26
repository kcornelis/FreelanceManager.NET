using System;

namespace FreelanceManager.Events.Account
{
    public class AccountMadeAdmin : Event
    {
        public AccountMadeAdmin(Guid id)
        {
            Id = id;
        }
    }
}
