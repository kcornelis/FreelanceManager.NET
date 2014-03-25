using System;
using Xunit;

namespace FreelanceManager.ReadModel.ToolTests
{
    public class when_the_hook_receives_a_event_that_is_not_the_next_one : Specification
    {
        private Guid _id = Guid.NewGuid();
        private string _tenant = Guid.NewGuid().ToString();
        private IServiceBus _bus;

        protected override void Context()
        {
            _bus = Resolve<IServiceBus>();

            _bus.PublishDomainUpdate(new [] { new SequenceNumberAdded(_id, 1) },
                                     Metadata(1));

        
        }

        [Fact]
        public void should_throw_an_exception()
        {
            Assert.Throws<InvalidVersionException>(() =>
            {
                _bus.PublishDomainUpdate(new[] { new SequenceNumberAdded(_id, 3) },
                                         Metadata(3));
            });
                        
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
