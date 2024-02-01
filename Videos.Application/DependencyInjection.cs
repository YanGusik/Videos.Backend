using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Videos.Application.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Videos.Application.Jobs;

namespace Videos.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(Video420ConvertJob));
            services.AddScoped(typeof(Video720ConvertJob));
            services.AddScoped(typeof(Video1080ConvertJob));
            services.AddScoped(typeof(VideoService));
            services.AddHangfire(cfg => cfg
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options =>
        {
            options.UseNpgsqlConnection(configuration.GetConnectionString("HangfireConnection"));
        }));

            services.AddHangfireServer();

            return services;
        }
    }
}

