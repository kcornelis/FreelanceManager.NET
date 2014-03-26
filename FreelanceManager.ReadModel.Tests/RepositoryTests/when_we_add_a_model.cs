using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.ReadModel.RepositoryTests
{
    public class when_we_add_a_model : Specification
    {
        private Guid _id = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();
        private Sequence _model;
        private ISequenceRepository _repository;
        private ITenantContext _tenantContext;

        protected override void Context()
        {
            _repository = Resolve<ISequenceRepository>();
            _tenantContext = Resolve<ITenantContext>();

            _tenantContext.SetTenantId(_tenant);
        }

        protected override void Because()
        {
            _repository.Add(new Sequence { Id = _id, Result = "1" });
            _model = _repository.GetById(_id);
        }

        [Fact]
        public void can_be_retrieved_by_id()
        {
            _model.Should().NotBeNull();
            _model.Result.Should().Be("1");
        }

        [Fact]
        public void should_have_a_tenant_id()
        {
            _model.Tenant.Should().Be(_tenant);
        }
    }
}
