using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.ReadModel.RepositoryTests
{
    public class when_we_update_a_model : Specification
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
            _repository.Add(new Sequence { Id = _id, Result = "1", Version = 1 }, 1);
        }

        protected override void Because()
        {
            _model = _repository.GetById(_id);

            _model.Result = "12";

            _repository.Update(_model, 2);

            _model = _repository.GetById(_id);
        }

        [Fact]
        public void should_have_updated_properties()
        {
            _model.Result.Should().Be("12");
        }

        [Fact]
        public void should_have_a_new_version()
        {
            _model.Version.Should().Be(2);
        }
    }
}
