using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.Domain.AccountTests
{
    public class when_an_account_password_is_changed : Specification
    {
        private Account _account;

        protected override void Context()
        {
            _account = new Account(Guid.NewGuid(), "John Doe BVBA", "John", "Doe", "john@doe.com");
        }

        protected override void Because()
        {
            _account.ChangePassword("abcd");
        }

        [Fact]
        public void should_have_a_password_hash()
        {
            _account.PasswordHash.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void should_have_a_password_salt()
        {
            _account.PasswordSalt.Should().NotBe(0);
        }

        [Fact]
        public void should_match_the_correct_password()
        {
            _account.VerifyPassword("abcd").Should().BeTrue();
        }

        [Fact]
        public void should_not_match_a_wrong_password()
        {
            _account.VerifyPassword("ab").Should().BeFalse();
        }

        [Fact]
        public void version_should_be_updated()
        {
            _account.Version.Should().Be(2);
        }
    }
}
