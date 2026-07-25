using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Infrastructure.Persistence.Context;
using MeetingTranscriptionBot.Infrastructure.Repositories;
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
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        services.AddScoped<IMeetingRepository, MeetingRepository>();

        return services;
    }
}