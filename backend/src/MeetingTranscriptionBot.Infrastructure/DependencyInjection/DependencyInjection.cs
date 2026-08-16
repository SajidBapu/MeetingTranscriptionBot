using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Infrastructure.Authentication;
using MeetingTranscriptionBot.Infrastructure.Identity;
using MeetingTranscriptionBot.Infrastructure.Services.AI;
using MeetingTranscriptionBot.Infrastructure.Persistence.Context;
using MeetingTranscriptionBot.Infrastructure.Repositories;
using MeetingTranscriptionBot.Infrastructure.Services.Security;
using MeetingTranscriptionBot.Infrastructure.Services.Speech;
using MeetingTranscriptionBot.Infrastructure.Services.Storage;
using MeetingTranscriptionBot.Infrastructure.BackgroundJobs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingTranscriptionBot.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AzureSpeechSettings>()
            .Bind(
                configuration.GetSection(
                    AzureSpeechSettings.SectionName))
            .Validate(
                settings =>
                    !string.IsNullOrWhiteSpace(settings.Key),
                "Azure Speech key is required.")
            .Validate(
                settings =>
                    !string.IsNullOrWhiteSpace(settings.Region),
                "Azure Speech region is required.")
            .Validate(
                settings =>
                    !string.IsNullOrWhiteSpace(settings.Endpoint),
                "Azure Speech endpoint is required.")
            .ValidateOnStart();

        var connectionString =
            configuration.GetConnectionString(
                "DefaultConnection")
            ?? throw new InvalidOperationException(
                "The DefaultConnection connection string is missing.");

        services
            .AddOptions<JwtSettings>()
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

        services.AddSingleton<
             ITranscriptionBackgroundQueue,
             TranscriptionBackgroundQueue>();

        services.AddHostedService<
           TranscriptionBackgroundWorker>();

        services
    .AddOptions<OllamaSettings>()
    .Bind(configuration.GetSection(
        OllamaSettings.SectionName))
    .Validate(
        x => Uri.TryCreate(
            x.BaseUrl,
            UriKind.Absolute,
            out _),
        "Ollama BaseUrl must be a valid URL.")
    .Validate(
        x => !string.IsNullOrWhiteSpace(x.Model),
        "Ollama model is required.")
    .ValidateOnStart();

        services.AddHttpClient<
    IMeetingIntelligenceService,
    OllamaMeetingIntelligenceService>(
        (serviceProvider, client) =>
        {
            var settings =
                serviceProvider
                    .GetRequiredService<
                        Microsoft.Extensions.Options.IOptions<OllamaSettings>>()
                    .Value;

            client.BaseAddress =
                new Uri(settings.BaseUrl);

            client.Timeout =
                TimeSpan.FromMinutes(5);
        });

        services.AddScoped<
            IJwtTokenGenerator,
            JwtTokenGenerator>();

        services.AddScoped<
            IRefreshTokenService,
            RefreshTokenService>();

        services.AddScoped<
             IMeetingAnalysisRepository,
             MeetingAnalysisRepository>();

        services.AddScoped<
            IRefreshTokenRepository,
            RefreshTokenRepository>();

        services.AddScoped<
            IMeetingRecordingRepository,
            MeetingRecordingRepository>();

        services.AddScoped<
            ITranscriptRepository,
            TranscriptRepository>();

        services.AddScoped<
            IFileStorageService,
            LocalFileStorageService>();

        services.AddScoped<
             ITranscriptionService,
             AzureDiarizedTranscriptionService>();

        services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options.UseNpgsql(connectionString);
            });

        services
            .AddIdentityCore<ApplicationUser>(
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

        services.AddScoped<
            IIdentityService,
            IdentityService>();

        services.AddScoped<
            IMeetingRepository,
            MeetingRepository>();

        return services;
    }
}