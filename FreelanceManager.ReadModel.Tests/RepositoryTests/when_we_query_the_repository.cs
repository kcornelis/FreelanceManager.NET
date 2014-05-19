using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.ReadModel.RepositoryTests
{
    public class when_we_query_the_repository : Specification
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
        }

        [Fact]
        public void can_apply_a_where_clause()
        {
            _tenantContext.SetTenantId(_tenant);
            _repository.Where(m => m.Result == "1").Should().HaveCount(1);
        }

        [Fact]
        public void should_query_models_from_the_selected_tenant()
        {
            _tenantContext.SetTenantId(Guid.NewGuid().ToString());
            _repository.Where(m => m.Result == "1").Should().BeEmpty();
        }

        [Fact]
        public void can_apply_a_any_clause()
        {
            _tenantContext.SetTenantId(_tenant);
            _repository.Any(m => m.Result == "1").Should().BeTrue();
            _repository.Any(m => m.Result == "123").Should().BeFalse();
        }
    }
}
