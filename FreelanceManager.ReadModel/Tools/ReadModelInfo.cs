using System;

namespace FreelanceManager.ReadModel.Tools
{
    public class ReadModelInfo
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Tenant { get; set; }
        public int Version { get; set; }
        public int Errors { get; set; }
        public DateTime? Locked { get; set; }
    }
}
