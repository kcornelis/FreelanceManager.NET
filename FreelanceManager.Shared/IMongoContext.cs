using MongoDB.Driver;

namespace FreelanceManager
{
    public interface IMongoContext
    {
        MongoDatabase GetDatabase();
    }
}
