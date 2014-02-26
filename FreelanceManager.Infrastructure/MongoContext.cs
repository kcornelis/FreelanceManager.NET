using MongoDB.Driver;
using System.Configuration;
using System.Linq;

namespace FreelanceManager.Infrastructure
{
    public class MongoContext : IMongoContext
    {
        private MongoDatabase _database;

        public MongoContext(string url)
        {
            var database = url.Substring(url.LastIndexOf("/") + 1);

            var client = new MongoClient(url);

            _database = client.GetServer().GetDatabase(database);
        }

        public MongoDatabase GetDatabase()
        {
            return _database;
        }
    }
}