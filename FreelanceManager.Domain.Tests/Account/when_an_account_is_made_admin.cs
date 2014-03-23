using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.Domain.Tests
{
    public class when_an_account_is_made_admin : Specification
    {
        private Account _account;

        protected override void Context()
        {
            _account = new Account(Guid.NewGuid(), "John Doe BVBA", "John", "Doe", "john@doe.com");
        }

        protected override void Because()
        {
            _account.MakeAdmin();
        }

        [Fact]
        public void should_be_admin()
        {
            _account.Admin.Should().BeTrue();
        }

        [Fact]
        public void version_should_be_updated()
        {
            _account.Version.Should().Be(2);
        }
    }
}
