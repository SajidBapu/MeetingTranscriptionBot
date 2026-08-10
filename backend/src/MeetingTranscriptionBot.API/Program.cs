using MeetingTranscriptionBot.Application.DependencyInjection;
using MeetingTranscriptionBot.Infrastructure.DependencyInjection;
using MeetingTranscriptionBot.API.Services;
using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.API.Extensions;
using MeetingTranscriptionBot.API.Middleware;
using Microsoft.OpenApi;

namespace MeetingTranscriptionBot.API
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddApplication();

            builder.Services.AddJwtAuthentication(
                             builder.Configuration);

            builder.Services.AddInfrastructure(builder.Configuration);
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();

            //builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(options =>
            {
                const string bearerScheme = "Bearer";

                options.AddSecurityDefinition(
                    bearerScheme,
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description =
                            "Enter the JWT access token only. " +
                            "Do not include the word Bearer."
                    });

                options.AddSecurityRequirement(
                    document => new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecuritySchemeReference(
                                bearerScheme,
                                document)
                        ] = []
                    });
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<
                ICurrentUserService,
                CurrentUserService>();


            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
