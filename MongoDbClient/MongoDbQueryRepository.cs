using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;

namespace MongoDbClient
{
    public interface IMongoDbQueryRepository
    {
        IQueryable<T> Query<T>(string collectionName) where T : class, new();
        IMongoQueryable<T> Query<T>() where T : class, new();
    }

    public class MongoDbQueryRepository : IMongoDbQueryRepository
    {
        public IMongoDatabase _db;

        public MongoDbQueryRepository(IDatabaseContext databaseContext)
        {
            _db = databaseContext.Database;
        }


        public IQueryable<T> Query<T>(string collectionName) where T : class, new()
        {
            return _db.GetCollection<T>(collectionName).AsQueryable();
        }

        public IMongoQueryable<T> Query<T>() where T : class, new()
        {
            return _db.GetCollection<T>(typeof(T).Name).AsQueryable();
        }
    }
}
