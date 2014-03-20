using System;

namespace FreelanceManager.ReadModel
{
    public abstract class Model
    {
        public Guid Id { get; set; }
        public string Tenant { get; set; }
        public int Version { get; set; }

        public void VerifyAndUpdateVersion(int version)
        {
            if (version != (Version + 1))
                throw new ModelInvalidVersionException(GetType().Name, Id, Version, version);

            Version = version;
        }
    }
}
