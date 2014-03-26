using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.Domain.AccountTests
{
    public class when_an_account_is_created : Specification
    {
        private Guid _id = Guid.NewGuid();
        private Account _account;

        protected override void Because()
        {
            _account = new Account(_id, "John Doe BVBA", "John", "Doe", "john@doe.com");
        }

        [Fact]
        public void should_have_a_id()
        {
            _account.Id.Should().Be(_id);
        }

        [Fact]
        public void should_have_a_name()
        {
            _account.Name.Should().Be("John Doe BVBA");
        }

        [Fact]
        public void should_have_a_fullname()
        {
            _account.FullName.Should().Be("John Doe");
        }

        [Fact]
        public void should_have_a_firstname()
        {
            _account.FirstName.Should().Be("John");
        }

        [Fact]
        public void should_have_a_lastname()
        {
            _account.LastName.Should().Be("Doe");
        }

        [Fact]
        public void should_have_an_email()
        {
            _account.Email.Should().Be("john@doe.com");
        }

        [Fact]
        public void should_not_be_Admin()
        {
            _account.Admin.Should().BeFalse();
        }

        [Fact]
        public void should_have_no_password()
        {
            _account.PasswordHash.Should().BeNullOrEmpty();
            _account.PasswordSalt.Should().Be(0);
        }

        [Fact]
        public void should_have_a_created_date()
        {
            _account.CreatedOn.Should().BeCloseTo(DateTime.Now, 1000);
        }

        [Fact]
        public void version_should_be_one()
        {
            _account.Version.Should().Be(1);
        }
    }
}
