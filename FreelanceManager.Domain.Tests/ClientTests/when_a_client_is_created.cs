using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.Domain.ClientTests
{
    public class when_a_client_is_created : Specification
    {
        private Guid _id = Guid.NewGuid();
        private Client _client;

        protected override void Because()
        {
            _client = new Client(_id, "John Doe BVBA");
        }

        [Fact]
        public void should_have_a_id()
        {
            _client.Id.Should().Be(_id);
        }

        [Fact]
        public void should_have_a_name()
        {
            _client.Name.Should().Be("John Doe BVBA");
        }

        [Fact]
        public void should_have_a_created_date()
        {
            _client.CreatedOn.Should().BeCloseTo(DateTime.Now, 1000);
        }

        [Fact]
        public void version_should_be_one()
        {
            _client.Version.Should().Be(1);
        }
    }
}
