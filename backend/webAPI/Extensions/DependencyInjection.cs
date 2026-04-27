using infrastructure.data;
using infrastructure.repositories;
using Microsoft.EntityFrameworkCore;
using service.interfaces.repositories;
using service.interfaces.services;
using service.services;

namespace webAPI.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Configuration
        var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION") 
            ?? "Host=localhost;Database=salamcircle;Username=postgres;Password=postgres";

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
