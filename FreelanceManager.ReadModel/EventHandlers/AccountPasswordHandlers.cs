using FreelanceManager.Events.Account;
using FreelanceManager.ReadModel.Repositories;

namespace FreelanceManager.ReadModel.EventHandlers
{
    public class AccountPasswordHandlers : IHandleEvent<AccountPasswordChanged>
    {
        private readonly IAccountPasswordRepository _accountRepository;

        public AccountPasswordHandlers(IAccountPasswordRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void Handle(AccountPasswordChanged @event)
        {
            var account = _accountRepository.GetById(@event.Id);

            if (account == null)
            {
                account = new AccountPassword
                {
                    Id = @event.Id,
                    Password = @event.HashedPassword,
                    PasswordSalt = @event.Salt
                };

                _accountRepository.Add(account);
            }
            else
            {
                account.Password = @event.HashedPassword;
                account.PasswordSalt = @event.Salt;

                _accountRepository.Update(account);
            }
        }
    }
}
