using System;
using FluentAssertions;
using FreelanceManager.Events.Client;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.ClientTests
{
    public class when_an_client_is_created : Specification
    {
        private Guid _id = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private ClientHandlers _handler;
        private IClientRepository _clientRepository;
        private ITenantContext _tenantContext;

        private Client _client;

        protected override void Context()
        {
            _handler = Resolve<ClientHandlers>();
            _clientRepository = Resolve<IClientRepository>();
            _tenantContext = Resolve<ITenantContext>();

            _tenantContext.SetTenantId(_tenant);
        }

        protected override void Because()
        {
            _handler.AsDynamic().Handle(new ClientCreated(_id, "John Doe BVBA", DateTime.UtcNow) { Version = 1 });

            _client = _clientRepository.GetById(_id);
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
        public void should_have_a_tenant()
        {
            _client.Tenant.Should().Be(_tenant);
        }

        [Fact]
        public void should_have_a_created_date()
        {
            _client.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }
    }
}
