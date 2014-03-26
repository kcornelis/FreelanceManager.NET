using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.ReadModel.RepositoryTests
{
    public class when_we_add_multiple_models : Specification
    {
        private string _tenant = Guid.NewGuid().ToString();
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
            _repository.Add(new[]
            {
                new Sequence { Id = Guid.NewGuid(), Result = "1" },
                new Sequence { Id = Guid.NewGuid(), Result = "2" },
                new Sequence { Id = Guid.NewGuid(), Result = "3" }
            });
        }

        [Fact]
        public void should_have_a_tenant_id()
        {
            _repository.Count(m => m.Tenant == _tenant).Should().Be(3);
        }
    }
}
