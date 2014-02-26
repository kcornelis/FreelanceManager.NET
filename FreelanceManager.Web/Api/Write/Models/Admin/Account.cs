using System;

namespace FreelanceManager.Web.Api.Write.Models.Admin
{
    public class Account
    {
        public Account() { }
        public Account(Domain.Account account)
        {
            Populate(account);
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }

        public void Populate(Domain.Account account)
        {
            Id = account.Id;
            Name = account.Name;
            FirstName = account.FirstName;
            LastName = account.LastName;
            Email = account.Email;
            CreatedOn = account.CreatedOn;
        }
    }
}