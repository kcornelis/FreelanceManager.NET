using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.ReadModel.RepositoryTests
{
    public class when_we_count_models : Specification
    {
        private string _tenant = Guid.NewGuid().ToString();
        private int _count;
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
            _repository.Add(new Sequence { Id = Guid.NewGuid(), Result = "1", Version = 1 }, 1);
            _repository.Add(new Sequence { Id = Guid.NewGuid(), Result = "2", Version = 1 }, 1);
            _repository.Add(new Sequence { Id = Guid.NewGuid(), Result = "3", Version = 1 }, 1);

            _tenantContext.SetTenantId(Guid.NewGuid().ToString());
            _repository.Add(new Sequence { Id = Guid.NewGuid(), Result = "4", Version = 1 }, 1);

            _tenantContext.SetTenantId(_tenant);
            _count = (int)_repository.Count();
        }

        [Fact]
        public void should_only_count_models_from_the_current_tenant()
        {
            _count.Should().Be(3);
        }
    }
}
