﻿using System;
using FluentAssertions;
using FreelanceManager.Events.Account;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.AccountTests
{
    public class when_an_account_is_made_admin : Specification
    {
        private Guid _id = Guid.NewGuid();
        private Account _account;
        private AccountHandlers _handler;
        private IAccountRepository _accountRepository;

        protected override void Context()
        {
            _handler = Resolve<AccountHandlers>();
            _accountRepository = Resolve<IAccountRepository>();

            _handler.AsDynamic().Handle(new AccountCreated(_id, "John Doe BVBA", "John", "Doe", "john@doe.com", DateTime.UtcNow) { Version = 1 });
        }

        protected override void Because()
        {
            _handler.AsDynamic().Handle(new AccountMadeAdmin(_id) { Version = 2 });

            _account = _accountRepository.GetById(_id);
        }

        [Fact]
        public void should_be_admin()
        {
            _account.Admin.Should().BeTrue();
        }
    }
}
