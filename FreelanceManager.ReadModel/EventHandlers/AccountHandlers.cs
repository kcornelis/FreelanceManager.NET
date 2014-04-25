using System;
using FreelanceManager.Events.Account;
using FreelanceManager.ReadModel.Repositories;

namespace FreelanceManager.ReadModel.EventHandlers
{
    public class AccountHandlers : IHandleEvent<AccountCreated>,
                                   IHandleEvent<AccountDetailsChanged>,
                                   IHandleEvent<AccountMadeAdmin>,
                                   IHandleEvent<AccountPasswordChanged>
    {
        private readonly IAccountRepository _accountRepository;

        public AccountHandlers(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void Handle(AccountCreated @event)
        {
            var account = new Account
            {
                Id = @event.Id,
                FirstName = @event.FirstName,
                LastName = @event.LastName,
                FullName = @event.FirstName + " " + @event.LastName,
                CreatedOn = @event.CreatedOn,
                Email = @event.Email,
                Name = @event.Name
            };

            _accountRepository.Add(account);
        }

        public void Handle(AccountDetailsChanged @event)
        {
            var account = _accountRepository.GetById(@event.Id);

            if (account != null)
            {
                account.FirstName = @event.FirstName;
                account.LastName = @event.LastName;
                account.FullName = @event.FirstName + " " + @event.LastName;
                account.Email = @event.Email;
                account.Name = @event.Name;

                _accountRepository.Update(account, @event.Version);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(AccountPasswordChanged @event)
        {
            var account = _accountRepository.GetById(@event.Id);

            if (account == null)
            {
                throw new ModelNotFoundException();
            }
            else
            {
                _accountRepository.Update(account, @event.Version);
            }
        }

        public void Handle(AccountMadeAdmin @event)
        {
            var account = _accountRepository.GetById(@event.Id);

            if (account != null)
            {
                account.Admin = true;

                _accountRepository.Update(account, @event.Version);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }
    }
}
