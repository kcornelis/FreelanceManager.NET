using System;
using System.Collections.Generic;

namespace FreelanceManager
{
    public class DomainUpdateBusMessage
    {
        public DomainUpdateMetadate Metadata { get; set; }

        public string[] Events { get; set; }
    }

    public class DomainUpdateMetadate
    {
        public string AggregateType { get; set; }
        public string Tenant { get; set; }
        public Guid AggregateId { get; set; }
        public string ApplicationService { get; set; }
        public int LastVersion { get; set; }
    }
}
