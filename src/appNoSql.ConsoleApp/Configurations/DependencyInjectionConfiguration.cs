using appNoSql.ConsoleApp.Model;
using appNoSql.Domain.Core.Configurations;
using appNoSql.Infra.Data.ElasticSearch.Extensions;
using appNoSql.Infra.Data.ElasticSearch.Interfaces;
using appNoSql.Infra.Data.ElasticSearch.Repository;
using appNoSql.Infra.Data.MongoDB.Extensions;
using appNoSql.Infra.Data.MongoDB.Interfaces;
using appNoSql.Infra.Data.MongoDB.Repository;
using appNoSql.Infra.Data.Redis.Extensions;
using appNoSql.Infra.Data.Redis.Interfaces;
using appNoSql.Infra.Data.Redis.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace appNoSql.ConsoleApp.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            var currentDirectory = string.Empty;
#if DEBUG
    currentDirectory = Directory.GetCurrentDirectory();
#else
    currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif

            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
              .SetBasePath(currentDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{environmentName}.json", optional: true);

            IConfiguration configuration = builder.Build();

            var here = configuration.GetSection("Test:Here").Value;
            services.Configure<AppConfiguration>(x =>
            {
                x.Here = here;
            });

            services.AddMongoDbExtensions(configuration);

            services.AddRedisExtensions(configuration);

            services.AddElasticsearchExtensions(configuration);

            services.AddSingleton<IConfiguration>(configuration);

            services.AddScoped(typeof(IElasticSearchRepository<>), typeof(ElasticSearchRepository<>));
            services.AddScoped(typeof(IRedisRepository<>), typeof(RedisRepository<>));
            services.AddScoped(typeof(IMongoDbRepository<>), typeof(MongoDbRepository<>));
            services.AddScoped<IPersonRepository, PersonRepository>();
        }
    }
}
