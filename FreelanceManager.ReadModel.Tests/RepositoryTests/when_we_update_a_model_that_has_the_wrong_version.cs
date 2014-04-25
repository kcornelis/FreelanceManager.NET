using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.ReadModel.RepositoryTests
{
    public class when_we_update_a_model_that_has_the_wrong_version : Specification
    {
        private Guid _id = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();
        private ISequenceRepository _repository;
        private ITenantContext _tenantContext;

        protected override void Context()
        {
            _repository = Resolve<ISequenceRepository>();
            _tenantContext = Resolve<ITenantContext>();

            _tenantContext.SetTenantId(_tenant);
            _repository.Add(new Sequence { Id = _id, Result = "1" });
        }

        [Fact]
        public void should_throw_an_exception()
        {
            Assert.Throws<InvalidVersionException>(() =>
            {
                _repository.Update(new Sequence { Id = Guid.NewGuid(), Result = "1", Tenant = _tenant, Version = 0 }, 2);
            });
        }

        [Fact]
        public void should_throw_an_exception_2()
        {
            Assert.Throws<InvalidVersionException>(() =>
            {
                _repository.Update(new Sequence { Id = Guid.NewGuid(), Result = "1", Tenant = _tenant, Version = 1 }, 3);
            });
        }

        [Fact]
        public void should_throw_an_exception_3()
        {
            Assert.Throws<InvalidVersionException>(() =>
            {
                _repository.Update(new Sequence { Id = Guid.NewGuid(), Result = "1", Tenant = _tenant, Version = 3 }, 3);
            });
        }

        [Fact]
        public void should_throw_an_exception_4()
        {
            Assert.Throws<InvalidVersionException>(() =>
            {
                _repository.Update(new Sequence { Id = Guid.NewGuid(), Result = "1", Tenant = _tenant, Version = 4 }, 3);
            });
        }
    }
}

