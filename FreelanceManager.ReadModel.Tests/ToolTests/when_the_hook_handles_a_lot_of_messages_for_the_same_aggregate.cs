using System;
using FluentAssertions;
using Xunit;
using TPL = System.Threading.Tasks;

namespace FreelanceManager.ReadModel.ToolTests
{
    public class when_the_hook_handles_a_lot_of_messages_for_the_same_aggregate : Specification
    {
        private const int _repeat = 100;

        private Guid _id = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();

        private Sequence _model;
        private ISequenceRepository _repository;
        private ITenantContext _tenantContext;
        private IServiceBus _bus;

        protected override void Context()
        {
            _bus = Resolve<IServiceBus>();
            _repository = Resolve<ISequenceRepository>();
            _tenantContext = Resolve<ITenantContext>();
        }

        protected override void Because()
        {
            var task1 = TPL.Task.Factory.StartNew(() =>
            {
                for (int i = 1; i < _repeat; )
                {
                    try
                    {
                        _bus.PublishDomainUpdate(new[] { new SequenceNumberAdded(_id, i) },
                                                 Metadata(i));

                        i += 3;
                    }
                    catch { }
                }
            });

            var task2 = TPL.Task.Factory.StartNew(() =>
            {
                for (int i = 2; i < _repeat; )
                {
                    try
                    {
                        _bus.PublishDomainUpdate(new[] { new SequenceNumberAdded(_id, i) },
                                                 Metadata(i));

                        i += 3;
                    }
                    catch { }
                }
            });

            var task3 = TPL.Task.Factory.StartNew(() =>
            {
                for (int i = 3; i < _repeat; )
                {
                    try
                    {
                        _bus.PublishDomainUpdate(new[] { new SequenceNumberAdded(_id, i) },
                                                 Metadata(i));

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
        public void should_process_the_events_in_order()
        {
            var result = "";
            for (int i = 1; i < _repeat; i++) { result += i.ToString(); }

            _model.Result.Should().Be(result);
        }

        private DomainUpdateMetadate Metadata(int version)
        {
            return new DomainUpdateMetadate
            {
                Tenant = _tenant,
                AggregateId = _id,
                AggregateType = "Sequence",
                ApplicationService = "ReadModelTests",
                LastVersion = version
            };
        }
    }
}
