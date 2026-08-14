using MeetingTranscriptionBot.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Infrastructure.Services.Speech;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace MeetingTranscriptionBot.API.IntegrationTests.Infrastructure;

public sealed class MeetingTranscriptionBotWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private const string TestJwtKey =
        "VGhpcyBpcyBhIHNlY3VyZSBpbnRlZ3JhdGlvbiB0ZXN0IEpXVCBrZXkgZm9yIHRlc3Rpbmcu";

    private readonly string _databaseName =
        $"MeetingTranscriptionBotTests-{Guid.NewGuid()}";

    public MeetingTranscriptionBotWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable(
            "Jwt__Issuer",
            "MeetingTranscriptionBot.API");

        Environment.SetEnvironmentVariable(
            "Jwt__Audience",
            "MeetingTranscriptionBot.Client");

        Environment.SetEnvironmentVariable(
            "Jwt__Key",
            TestJwtKey);

        Environment.SetEnvironmentVariable(
           "Jwt__RefreshTokenExpirationDays",
           "7");

        Environment.SetEnvironmentVariable(
            "Jwt__AccessTokenExpirationMinutes",
            "15");

        Environment.SetEnvironmentVariable(
    "AzureSpeech__Key",
    "integration-test-azure-speech-key");

        Environment.SetEnvironmentVariable(
            "AzureSpeech__Region",
            "canadacentral");

        Environment.SetEnvironmentVariable(
            "AzureSpeech__Endpoint",
            "https://integration-test-speech.invalid");

        Environment.SetEnvironmentVariable(
            "ConnectionStrings__DefaultConnection",
            "Host=localhost;Database=unused;Username=test;Password=test");
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ApplicationDbContext>();

            services.RemoveAll<
                DbContextOptions<ApplicationDbContext>>();

            services.RemoveAll<
                IDbContextOptionsConfiguration<ApplicationDbContext>>();

            services.AddDbContext<ApplicationDbContext>(
                options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ITranscriptionService>();

            services.AddScoped<
                ITranscriptionService,
                DevelopmentTranscriptionService>();
        });
    }

    protected override IHost CreateHost(
        IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope =
            host.Services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

        dbContext.Database.EnsureCreated();

        return host;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        Environment.SetEnvironmentVariable(
            "Jwt__Issuer",
            null);

        Environment.SetEnvironmentVariable(
            "Jwt__Audience",
            null);

        Environment.SetEnvironmentVariable(
            "Jwt__Key",
            null);

        Environment.SetEnvironmentVariable(
            "Jwt__AccessTokenExpirationMinutes",
            null);

        Environment.SetEnvironmentVariable(
            "ConnectionStrings__DefaultConnection",
            null);

        Environment.SetEnvironmentVariable(
             "Jwt__RefreshTokenExpirationDays",
             null);

        Environment.SetEnvironmentVariable(
             "AzureSpeech__Key",
             null);

        Environment.SetEnvironmentVariable(
            "AzureSpeech__Region",
            null);

        Environment.SetEnvironmentVariable(
            "AzureSpeech__Endpoint",
            null);
    }
}