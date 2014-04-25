using System;
using FluentAssertions;
using FreelanceManager.Events.Account;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.AccountPasswordTests
{
    public class when_a_password_is_updated : Specification
    {
        private Guid _id = Guid.NewGuid();
        private AccountPassword _account;
        private AccountPasswordHandlers _handler;
        private IAccountPasswordRepository _accountPasswordRepository;

        protected override void Context()
        {
            _handler = Resolve<AccountPasswordHandlers>();
            _accountPasswordRepository = Resolve<IAccountPasswordRepository>();

            _handler.AsDynamic().Handle(new AccountCreated(_id, "John Doe BVBA", "John", "Doe", "john@doe.com", DateTime.UtcNow) { Version = 1 });

            _handler.AsDynamic().Handle(new AccountPasswordChanged(_id, "hash", 20) { Version = 2 });
        }

        protected override void Because()
        {
            _handler.AsDynamic().Handle(new AccountPasswordChanged(_id, "newhash", 30) { Version = 3 });

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
            _account.Password.Should().Be("newhash");
        }

        [Fact]
        public void should_have_a_password_salt()
        {
            _account.PasswordSalt.Should().Be(30);
        }
    }
}
