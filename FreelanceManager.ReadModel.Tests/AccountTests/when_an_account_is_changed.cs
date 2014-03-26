using System;
using FluentAssertions;
using FreelanceManager.Events.Account;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.AccountTests
{
    public class when_an_account_is_changed : Specification
    {
        private Guid _id = Guid.NewGuid();
        private Account _account;
        private AccountHandlers _handler;
        private IAccountRepository _accountRepository;

        protected override void Context()
        {
            _handler = Resolve<AccountHandlers>();
            _accountRepository = Resolve<IAccountRepository>();

            _handler.AsDynamic().Handle(new AccountCreated(_id, "John Doe BVBA", "John", "Doe", "john@doe.com", DateTime.UtcNow));
        }

        protected override void Because()
        {
            _handler.AsDynamic().Handle(new AccountDetailsChanged(_id, "Jane Turbo BVBA", "Jane", "Turbo", "jane@turbo.com"));

            _account = _accountRepository.GetById(_id);
        }

        [Fact]
        public void should_have_a_id()
        {
            _account.Id.Should().Be(_id);
        }

        [Fact]
        public void should_have_a_name()
        {
            _account.Name.Should().Be("Jane Turbo BVBA");
        }

        [Fact]
        public void should_have_a_fullname()
        {
            _account.FullName.Should().Be("Jane Turbo");
        }

        [Fact]
        public void should_have_a_firstname()
        {
            _account.FirstName.Should().Be("Jane");
        }

        [Fact]
        public void should_have_a_lastname()
        {
            _account.LastName.Should().Be("Turbo");
        }

        [Fact]
        public void should_have_an_email()
        {
            _account.Email.Should().Be("jane@turbo.com");
        }

        [Fact]
        public void should_not_be_admin()
        {
            _account.Admin.Should().BeFalse();
        }

        [Fact]
        public void should_have_no_tenant()
        {
            _account.Tenant.Should().BeNullOrEmpty();
        }

        [Fact]
        public void should_have_a_created_date()
        {
            _account.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }
    }
}
