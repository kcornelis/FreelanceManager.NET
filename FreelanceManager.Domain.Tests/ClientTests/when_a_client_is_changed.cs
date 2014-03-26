using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.Domain.ClientTests
{
    public class when_a_client_is_changed : Specification
    {
        private Client _client;

        protected override void Context()
        {
            _client = new Client(Guid.NewGuid(), "John Doe BVBA");
        }

        protected override void Because()
        {
            _client.ChangeDetails("Jane BVBA");
        }

        [Fact]
        public void should_have_a_new_name()
        {
            _client.Name.Should().Be("Jane BVBA");
        }

        [Fact]
        public void version_should_be_updated()
        {
            _client.Version.Should().Be(2);
        }
    }
}
