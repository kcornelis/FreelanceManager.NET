using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.Domain.Tests
{
    public class when_an_account_password_is_generated : Specification
    {
        private Account _account;
        private string _password;

        protected override void Context()
        {
            _account = new Account(Guid.NewGuid(), "John Doe BVBA", "John", "Doe", "john@doe.com");
        }

        protected override void Because()
        {
            _password = _account.GeneratePassword();
        }

        [Fact]
        public void should_have_a_password_with_length_6()
        {
            _password.Should().HaveLength(6);
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
        public void should_match_verify_password()
        {
            _account.VerifyPassword(_password).Should().BeTrue();
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
