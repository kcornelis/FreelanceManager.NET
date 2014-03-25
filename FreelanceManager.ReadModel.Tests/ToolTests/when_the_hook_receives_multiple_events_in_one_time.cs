using System;
using FluentAssertions;
using FreelanceManager.Events.Account;
using FreelanceManager.ReadModel.EventHandlers;
using FreelanceManager.ReadModel.Repositories;
using FreelanceManager.Tools;
using Xunit;

namespace FreelanceManager.ReadModel.ToolTests
{
    public class when_the_hook_receives_multiple_events_in_one_time : Specification
    {
        private Guid _id = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();
        private Sequence _sequence;
        private ISequenceRepository _sequenceRepository;
        private IServiceBus _bus;
        private ITenantContext _tenantContext;

        protected override void Context()
        {
            _bus = Resolve<IServiceBus>();
            _sequenceRepository = Resolve<ISequenceRepository>();
            _tenantContext = Resolve<ITenantContext>();

            _bus.PublishDomainUpdate(new [] { new SequenceNumberAdded(_id, 1) },
                                     Metadata(1));

        
        }

        protected override void Because()
        {
            _bus.PublishDomainUpdate(new[] 
                                    { 
                                        new SequenceNumberAdded(_id, 2),
                                        new SequenceNumberAdded(_id, 3),
                                        new SequenceNumberAdded(_id, 4),
                                        new SequenceNumberAdded(_id, 5)
                                    },
                                     Metadata(5));

            _sequence = _sequenceRepository.GetById(_id);
        }

        [Fact]
        public void should_send_all_events_to_the_handlers()
        {
            _sequence.Result.Should().Be("12345");
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
