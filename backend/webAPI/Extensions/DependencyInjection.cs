using infrastructure.data;
using infrastructure.repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
        services.AddScoped<ICircleRepository, CircleRepository>();
        services.AddScoped<ICircleMemberRepository, CircleMemberRepository>();
        services.AddScoped<ICircleInvitationRepository, CircleInvitationRepository>();
        services.AddScoped<IContributionRepository, ContributionRepository>();
        services.AddScoped<IContributionCycleRepository, ContributionCycleRepository>();
        services.AddScoped<ISwapRequestRepository, SwapRequestRepository>();
        services.AddScoped<IReminderRepository, ReminderRepository>();
        services.AddScoped<ITransparencyLogRepository, TransparencyLogRepository>();
        services.AddScoped<IEmergencyRequestRepository, EmergencyRequestRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICircleService, CircleService>();
        services.AddScoped<ICircleMemberService, CircleMemberService>();
        services.AddScoped<IInvitationService, InvitationService>();
        services.AddScoped<IContributionService, ContributionService>();
        services.AddScoped<IContributionCycleService, ContributionCycleService>();
        services.AddScoped<ISwapService, SwapService>();
        services.AddScoped<IReminderService, ReminderService>();
        services.AddScoped<ITransparencyLogService, TransparencyLogService>();
        services.AddScoped<IEmergencyRequestService, EmergencyRequestService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuthService, AuthService>();

      

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                    ?? configuration["JwtSettings:Issuer"],
                ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                    ?? configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                    Environment.GetEnvironmentVariable("JWT_SECRET")
                    ?? configuration["JwtSettings:SecretKey"]
                    ?? string.Empty))
            };



        });

        services.AddAuthorization();

        return services;
    }
}
