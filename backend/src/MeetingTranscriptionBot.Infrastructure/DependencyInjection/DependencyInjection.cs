using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Infrastructure.Identity;
using MeetingTranscriptionBot.Infrastructure.Persistence.Context;
using MeetingTranscriptionBot.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingTranscriptionBot.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "DefaultConnection")
            ?? throw new InvalidOperationException(
                "The DefaultConnection connection string is missing.");

        services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options.UseNpgsql(connectionString);
            });

        services.AddIdentityCore<ApplicationUser>(
                options =>
                {
                    options.Password.RequiredLength = 12;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredUniqueChars = 6;

                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan =
                        TimeSpan.FromMinutes(15);

                    options.User.RequireUniqueEmail = true;

                    // Enable after email verification is implemented.
                    options.SignIn.RequireConfirmedEmail = false;
                })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager();
        //.AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();

        services.AddScoped<
            IMeetingRepository,
            MeetingRepository>();

        return services;
    }
}