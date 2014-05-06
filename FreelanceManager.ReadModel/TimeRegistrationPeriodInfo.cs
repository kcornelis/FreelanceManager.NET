using System;
using System.Collections.Generic;
using System.Linq;

namespace FreelanceManager.ReadModel
{
    public class TimeRegistrationPeriodInfo
    {
        public int Count { get; set; }
        public decimal Income { get; set; }
        public int UnbillableMinutes { get; set; }
        public int BillableMinutes { get; set; }
    }
}