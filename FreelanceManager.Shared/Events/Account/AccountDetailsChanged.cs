using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelanceManager.Events.Account
{
    public class AccountDetailsChanged : Event
    {
        public AccountDetailsChanged(Guid id, string name, string firstName, string lastName, string email)
        {
            Id = id;
            Name = name;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public string Name { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
    }
}
