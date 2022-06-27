using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace appNoSql.Infra.Data.Redis.Extensions
{
    public static class RedisExtensions
    {
        public static void AddRedisExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = configuration["Redis:InstanceName"];
            });
        }
    }
}
