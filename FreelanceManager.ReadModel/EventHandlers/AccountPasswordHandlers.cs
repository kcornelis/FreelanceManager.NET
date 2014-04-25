using FreelanceManager.Events.Account;
using FreelanceManager.ReadModel.Repositories;

namespace FreelanceManager.ReadModel.EventHandlers
{
    public class AccountPasswordHandlers : IHandleEvent<AccountCreated>,
                                           IHandleEvent<AccountDetailsChanged>,
                                           IHandleEvent<AccountMadeAdmin>,
                                           IHandleEvent<AccountPasswordChanged>
    {
        private readonly IAccountPasswordRepository _accountRepository;

        public AccountPasswordHandlers(IAccountPasswordRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void Handle(AccountCreated @event)
        {
            var account = new AccountPassword
            {
                Id = @event.Id,
                Password = "",
                PasswordSalt = 0
            };

            _accountRepository.Add(account);
        }

        public void Handle(AccountDetailsChanged @event)
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

        public void Handle(AccountPasswordChanged @event)
        {
            var account = _accountRepository.GetById(@event.Id);

            if (account == null)
            {
                throw new ModelNotFoundException();
            }
            else
            {
                account.Password = @event.HashedPassword;
                account.PasswordSalt = @event.Salt;

                _accountRepository.Update(account, @event.Version);
            }
        }

        public void Handle(AccountMadeAdmin @event)
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
    }
}
