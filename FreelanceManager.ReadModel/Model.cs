using System;

namespace FreelanceManager.ReadModel
{
    public abstract class Model
    {
        public Guid Id { get; set; }
        public string Tenant { get; set; }
        public int Version { get; set; }
    }
}
