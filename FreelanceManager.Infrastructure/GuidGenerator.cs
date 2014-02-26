using System;
using MongoDB.Bson;

namespace FreelanceManager.Infrastructure
{
    public class GuidGenerator : IIdGenerator
    {
        public Guid NextGuid()
        {
            return Guid.NewGuid();
        }

        public string Next()
        {
            return new ObjectId().ToString();
        }
    }
}