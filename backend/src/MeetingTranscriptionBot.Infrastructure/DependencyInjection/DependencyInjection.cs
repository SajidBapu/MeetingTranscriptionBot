using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Infrastructure.Identity;
using MeetingTranscriptionBot.Infrastructure.Services.Security;
using MeetingTranscriptionBot.Infrastructure.Persistence.Context;
using MeetingTranscriptionBot.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using MeetingTranscriptionBot.Infrastructure.Authentication;
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

        services.AddOptions<JwtSettings>()
    .Bind(
        configuration.GetSection(
            JwtSettings.SectionName))
    .Validate(
        settings =>
            !string.IsNullOrWhiteSpace(settings.Issuer),
        "JWT issuer is required.")
    .Validate(
        settings =>
            !string.IsNullOrWhiteSpace(settings.Audience),
        "JWT audience is required.")
    .Validate(
        settings =>
            !string.IsNullOrWhiteSpace(settings.Key),
        "JWT signing key is required.")
    .Validate(
        settings =>
            settings.AccessTokenExpirationMinutes
                is >= 5 and <= 60,
        "JWT expiration must be between 5 and 60 minutes.")
    
    .Validate(
    settings =>
        settings.RefreshTokenExpirationDays
            is >= 1 and <= 30,
      "Refresh token expiration must be between 1 and 30 days.")
    .ValidateOnStart();

        services.AddSingleton(TimeProvider.System);

        services.AddScoped<
            IJwtTokenGenerator,
            JwtTokenGenerator>();

        services.AddScoped<
            IRefreshTokenService,
            RefreshTokenService>();

        services.AddScoped<
           IRefreshTokenRepository,
           RefreshTokenRepository>();

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