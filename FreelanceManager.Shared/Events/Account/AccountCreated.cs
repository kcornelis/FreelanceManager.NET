using System;

namespace FreelanceManager.Events.Account
{
    public class AccountCreated : Event
    {
        public AccountCreated(Guid id, string name, string firstName, string lastName, string email, DateTime createdOn)
        {
            Id = id;
            Name = name;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            CreatedOn = createdOn;
        }

        public string Name { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public DateTime CreatedOn { get; private set; }
    }
}