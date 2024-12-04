using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace appNoSql.Infra.Data.ElasticSearch.Extensions
{
    public static class ElasticSearchExtensions
    {
        public static void AddElasticsearchExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new ConnectionSettings(new Uri(configuration["ElasticsearchSettings:uri"]));

            var defaultIndex = configuration["ElasticsearchSettings:defaultIndex"];

            if (!string.IsNullOrEmpty(defaultIndex))
                settings = settings.DefaultIndex(defaultIndex);

            var basicAuthUser = configuration["ElasticsearchSettings:username"];
            var basicAuthPassword = configuration["ElasticsearchSettings:password"];

            if (!string.IsNullOrEmpty(basicAuthUser) && !string.IsNullOrEmpty(basicAuthPassword))
                settings = settings.BasicAuthentication(basicAuthUser, basicAuthPassword);

            settings.EnableApiVersioningHeader();
            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);
        }
    }
}
