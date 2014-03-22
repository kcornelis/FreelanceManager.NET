using System;
using System.Collections.Generic;

namespace FreelanceManager
{
    public class DomainUpdateBusMessage
    {
        public DomainUpdateMetadate Metadata { get; set; }

        public object Event { get; set; }
    }

    public class DomainUpdateMetadate
    {
        public string AggregateType { get; set; }
        public string Tenant { get; set; }
        public string AggregateId { get; set; }
        public string ApplicationService { get; set; }
        public int Version { get; set; }
    }
}
