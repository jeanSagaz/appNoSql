using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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

            //services.AddSingleton<IConnectionMultiplexer>((sp) =>
            //{
            //    //return ConnectionMultiplexer.Connect(configuration.GetSection("DevWeek:Redis:ConnectionString").Get<string>(), null);
            //    return ConnectionMultiplexer.Connect(configuration.GetSection("DevWeek:Redis:ConnectionString").Value, null);
            //});

            //services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(new ConfigurationOptions
            //{
            //    EndPoints = { $"{configuration.GetSection("RedisCache:Host").Value}:{configuration.GetSection("RedisCache:Port").Value}" },
            //    Ssl = true,
            //    AbortOnConnectFail = false,
            //}));
        }
    }
}
