using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Videos.Application.Interfaces;

namespace Videos.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection
            services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<VideoDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            services.AddScoped<IVideoDbContext>(provider => provider.GetRequiredService<VideoDbContext>());
            return services;
        }
    }
}
