﻿using System;
using FluentAssertions;
using Xunit;

namespace FreelanceManager.ReadModel.RepositoryTests
{
    public class when_we_delete_a_model_from_another_tenant : Specification
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
            _tenantContext.SetTenantId(Guid.NewGuid().ToString());
            _repository.Delete(_id, 1);

            _tenantContext.SetTenantId(_tenant);
            _model = _repository.GetById(_id);
        }

        [Fact]
        public void should_not_be_deleted()
        {
            _model.Should().NotBeNull();
        }
    }
}
