using System;
using FreelanceManager.Events.Account;
using FreelanceManager.Tools;

namespace FreelanceManager.Domain
{
    public class Account : AggregateRoot
    {
        public Account() { }
        public Account(Guid id, string name, string firstName, string lastName, string email)
        {
            ApplyChange(new AccountCreated(id, name, firstName, lastName, email, DateTime.Now));
        }

        public string Name { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public bool Admin { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public int PasswordSalt { get; private set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public bool VerifyPassword(string password)
        {
            var givenPassword = new Password(password, PasswordSalt);
            return givenPassword.EqualsSaltedHash(PasswordHash);
        }

        public void ChangePassword(string password)
        {
            var salt = Password.CreateRandomSalt();
            var hashedPassword = new Password(password, salt);

            ApplyChange(new AccountPasswordChanged(Id, hashedPassword.ComputeSaltedHash(), salt));
        }

        public string GeneratePassword()
        {
            var password = Password.CreateRandomPassword(6);
            ChangePassword(password);
            return password;
        }

        public void ChangeDetails(string name, string firstName, string lastName, string email)
        {
            if (Name != name || FirstName != firstName ||
                LastName != lastName || Email != email)
            {
                ApplyChange(new AccountDetailsChanged(Id, name, firstName, lastName, email));
            }
        }

        public void MakeAdmin()
        {
            if (!Admin)
            {
                ApplyChange(new AccountMadeAdmin(Id));
            }
        }

        public void Apply(AccountCreated e)
        {
            Id = e.Id;
            Name = e.Name;
            FirstName = e.FirstName;
            LastName = e.LastName;
            Email = e.Email;
            CreatedOn = e.CreatedOn;
        }

        public void Apply(AccountDetailsChanged e)
        {
            Name = e.Name;
            FirstName = e.FirstName;
            LastName = e.LastName;
            Email = e.Email;
        }

        public void Apply(AccountPasswordChanged e)
        {
            PasswordSalt = e.Salt;
            PasswordHash = e.HashedPassword;
        }

        public void Apply(AccountMadeAdmin e)
        {
            Admin = true;
        }
    }
}