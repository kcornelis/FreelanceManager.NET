using System;
using FluentAssertions;
using Xunit;
using TPL = System.Threading.Tasks;

namespace FreelanceManager.ReadModel.ToolTests
{
    public class when_the_repository_needs_to_handle_a_lot_of_updates_for_the_same_aggregate : Specification
    {
        private const int _repeat = 100;

        private Guid _id = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private Sequence _model;
        private ISequenceRepository _repository;
        private ITenantContext _tenantContext;
        private SequenceAddNumberHandlers _handler;

        protected override void Context()
        {
            _repository = Resolve<ISequenceRepository>();
            _tenantContext = Resolve<ITenantContext>();
            _handler = Resolve<SequenceAddNumberHandlers>();
        }

        protected override void Because()
        {
            var task1 = TPL.Task.Factory.StartNew(() =>
            {
                _tenantContext.SetTenantId(_tenant);

                for (int i = 1; i < _repeat; )
                {
                    try
                    {
                        _handler.Handle(new SequenceNumberAdded(_id, i){ Version = i });

                        i += 3;
                    }
                    catch { }
                }
            });

            var task2 = TPL.Task.Factory.StartNew(() =>
            {
                _tenantContext.SetTenantId(_tenant);

                for (int i = 2; i < _repeat; )
                {
                    try
                    {
                        _handler.Handle(new SequenceNumberAdded(_id, i) { Version = i });

                        i += 3;
                    }
                    catch { }
                }
            });

            var task3 = TPL.Task.Factory.StartNew(() =>
            {
                _tenantContext.SetTenantId(_tenant);

                for (int i = 3; i < _repeat; )
                {
                    try
                    {
                        _handler.Handle(new SequenceNumberAdded(_id, i) { Version = i });

                        i += 3;
                    }
                    catch { }
                }
            });

            TPL.Task.WaitAll(task1, task2, task3);

            _tenantContext.SetTenantId(_tenant);
            _model = _repository.GetById(_id);
        }

        [Fact]
        public void should_process_the_updates_in_order()
        {
            var result = "";
            for (int i = 1; i < _repeat; i++) { result += i.ToString(); }

            _model.Result.Should().Be(result);
        }
    }
}
