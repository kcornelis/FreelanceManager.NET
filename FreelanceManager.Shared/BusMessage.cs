using System;
using System.Collections.Generic;

namespace FreelanceManager
{
    public class BusMessage
    {
        public Dictionary<string, string> Headers { get; set; }
        public object[] Messages { get; set; }
    }
}
