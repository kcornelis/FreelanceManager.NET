using System;

namespace FreelanceManager.Events.Account
{
    public class AccountPasswordChanged : Event
    {
        public AccountPasswordChanged(Guid id, string hashedPassword, int salt)
        {
            Id = id;
            HashedPassword = hashedPassword;
            Salt = salt;
        }

        public string HashedPassword
        {
            get;
            private set;
        }

        public int Salt
        {
            get;
            private set;
        }
    }
}
