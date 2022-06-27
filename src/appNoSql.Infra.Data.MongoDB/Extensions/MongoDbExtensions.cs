using appNoSql.Domain.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace appNoSql.Infra.Data.MongoDB.Extensions
{
    public static class MongoDbExtensions
    {
        public static void AddMongoDBExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoDBConfigurations = new MongoDbConfigurations();
            configuration.Bind("MongoDbSettings", mongoDBConfigurations);
            services.AddSingleton(mongoDBConfigurations);
        }
    }
}
