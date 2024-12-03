using appNoSql.Domain.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace appNoSql.Infra.Data.MongoDB.Extensions
{
    public static class MongoDbExtensions
    {
        public static void AddMongoDbExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoDBConfigurations = new MongoDbConfiguration();
            configuration.Bind("MongoDbSettings", mongoDBConfigurations);
            services.AddSingleton(mongoDBConfigurations);
        }
    }
}
