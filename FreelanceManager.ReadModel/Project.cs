using System;
using System.Collections.Generic;

namespace FreelanceManager.ReadModel
{
    public class Project : Model
    {
        public string Name { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public bool Hidden { get; set; }
        public DateTime CreatedOn { get; set; }
        public Task[] Tasks { get; set; }
    }
}