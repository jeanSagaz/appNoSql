using appNoSql.Domain.Core.Configurations;
using MongoDB.Driver;

namespace appNoSql.Infra.Data.MongoDB.Context
{
    public abstract class MongoDbContext
    {
        protected readonly IMongoDatabase _database;

        protected MongoDbContext(MongoDbConfigurations configurations)
        {
            var client = new MongoClient(configurations.ConnectionString);
            _database = client.GetDatabase(configurations.DataBase);
        }

        protected IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            return _database.GetCollection<TEntity>(typeof(TEntity).Name.Trim());
        }
    }
}
