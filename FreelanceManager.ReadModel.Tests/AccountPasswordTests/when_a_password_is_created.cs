using System;
using FluentAssertions;
using FreelanceManager.Events.Account;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.AccountPasswordTests
{
    public class when_a_password_is_created : Specification
    {
        private Guid _id = Guid.NewGuid();
        private AccountPassword _account;
        private AccountPasswordHandlers _handler;
        private IAccountPasswordRepository _accountPasswordRepository;

        protected override void Context()
        {
            _handler = Resolve<AccountPasswordHandlers>();
            _accountPasswordRepository = Resolve<IAccountPasswordRepository>();
        }

        protected override void Because()
        {
            _handler.AsDynamic().Handle(new AccountPasswordChanged(_id, "hash", 20));

            _account = _accountPasswordRepository.GetById(_id);
        }

        [Fact]
        public void should_have_a_id()
        {
            _account.Id.Should().Be(_id);
        }

        [Fact]
        public void should_have_a_password()
        {
            _account.Password.Should().Be("hash");
        }

        [Fact]
        public void should_have_a_password_salt()
        {
            _account.PasswordSalt.Should().Be(20);
        }
    }
}
