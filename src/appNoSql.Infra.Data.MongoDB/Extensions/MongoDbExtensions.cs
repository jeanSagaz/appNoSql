using appNoSql.Domain.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace appNoSql.Infra.Data.MongoDB.Extensions
{
    public static class MongoDbExtensions
    {
        public static void AddMongoDbExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoDBConfigurations = new MongoDbConfiguration();
            configuration.Bind("MongoDbSettings", mongoDBConfigurations);
            services.AddSingleton(mongoDBConfigurations);

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));            
        }
    }
}
