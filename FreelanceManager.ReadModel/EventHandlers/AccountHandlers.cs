using System;
using FreelanceManager.Events.Account;
using FreelanceManager.ReadModel.Repositories;

namespace FreelanceManager.ReadModel.EventHandlers
{
    public class AccountHandlers : IHandleEvent<AccountCreated>,
                                   IHandleEvent<AccountDetailsChanged>,
                                   IHandleEvent<AccountPasswordChanged>,
                                   IHandleEvent<AccountMadeAdmin>
    {
        private readonly IAccountRepository _accountRepository;

        public AccountHandlers(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void Handle(AccountCreated @event)
        {
            if (_accountRepository.GetById(@event.Id) != null)
                throw new ModelDuplicateException(GetType().Name, @event.Id);

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

            account.VerifyAndUpdateVersion(@event.Version);

            _accountRepository.Add(account);
        }

        public void Handle(AccountPasswordChanged @event)
        {
            var account = _accountRepository.GetById(@event.Id);

            if (account != null)
            {
                account.VerifyAndUpdateVersion(@event.Version);
                _accountRepository.Update(account);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(AccountDetailsChanged @event)
        {
            var account = _accountRepository.GetById(@event.Id);

            if (account != null)
            {
                account.VerifyAndUpdateVersion(@event.Version);

                account.FirstName = @event.FirstName;
                account.LastName = @event.LastName;
                account.FullName = @event.FirstName + " " + @event.LastName;
                account.Email = @event.Email;
                account.Name = @event.Name;

                _accountRepository.Update(account);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(AccountMadeAdmin @event)
        {
            var account = _accountRepository.GetById(@event.Id);

            if (account != null)
            {
                account.VerifyAndUpdateVersion(@event.Version);

                account.Admin = true;

                _accountRepository.Update(account);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }
    }
}
