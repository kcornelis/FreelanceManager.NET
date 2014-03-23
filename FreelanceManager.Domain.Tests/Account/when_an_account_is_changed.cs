using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.Domain.Tests
{
    public class when_an_account_is_changed : Specification
    {
        private Account _account;

        protected override void Context()
        {
            _account = new Account(Guid.NewGuid(), "John Doe BVBA", "John", "Doe", "john@doe.com");
        }

        protected override void Because()
        {
            _account.ChangeDetails("Jane BVBA", "Jane", "Turbo", "jane@turbo.com");
        }

        [Fact]
        public void should_have_a_new_name()
        {
            _account.Name.Should().Be("Jane BVBA");
        }

        [Fact]
        public void should_have_a_new_fullname()
        {
            _account.FullName.Should().Be("Jane Turbo");
        }

        [Fact]
        public void should_have_a_firstname()
        {
            _account.FirstName.Should().Be("Jane");
        }

        [Fact]
        public void should_have_a_new_lastname()
        {
            _account.LastName.Should().Be("Turbo");
        }

        [Fact]
        public void should_have_a_new_email()
        {
            _account.Email.Should().Be("jane@turbo.com");
        }

        [Fact]
        public void version_should_be_updated()
        {
            _account.Version.Should().Be(2);
        }
    }
}
