using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.ReadModel.RepositoryTests
{
    public class when_we_add_a_model_twice : Specification
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
            _repository.Add(new Sequence { Id = _id, Result = "1", Version = 1 }, 1);
            _model = _repository.GetById(_id);
        }

        [Fact]
        public void should_throw_an_exception()
        {
            Assert.Throws<DatabaseException>(() =>
            {
                _repository.Add(new Sequence { Id = _id, Result = "1", Version = 1 }, 1);
            });
        }

        //[Fact]
        //public void should_also_throw_an_exception_when_the_item_is_part_of_a_batch()
        //{
        //    Assert.Throws<DatabaseException>(() =>
        //    {
        //        _repository.Add(new []{ new Sequence { Id = _id, Result = "1" , Version = 1 } }, 1);
        //    });
        //}
    }
}
