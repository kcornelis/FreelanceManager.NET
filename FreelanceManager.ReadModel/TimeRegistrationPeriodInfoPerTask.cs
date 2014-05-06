using System;
using System.Collections.Generic;
using System.Linq;

namespace FreelanceManager.ReadModel
{
    public class TimeRegistrationPeriodInfoPerTask
    {
        public Guid ClientId { get; set; }
        public string Client { get; set; }
        public Guid ProjectId { get; set; }
        public string Project { get; set; }
        public string Task { get; set; }

        public int Count { get; set; }
        public decimal Income { get; set; }
        public int UnbillableMinutes { get; set; }
        public int BillableMinutes { get; set; }
    }
}