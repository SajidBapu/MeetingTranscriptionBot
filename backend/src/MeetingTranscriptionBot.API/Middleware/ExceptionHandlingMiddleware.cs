using System.Net;
using System.Text.Json;
using FluentValidation;

namespace MeetingTranscriptionBot.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;


    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }


    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            await HandleValidationExceptionAsync(
                context,
                exception);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred");

            await HandleExceptionAsync(
                context);
        }
    }


    private static async Task HandleValidationExceptionAsync(
        HttpContext context,
        ValidationException exception)
    {
        context.Response.ContentType =
            "application/json";


        context.Response.StatusCode =
            (int)HttpStatusCode.BadRequest;


        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .ToArray());


        var response = new
        {
            success = false,
            message = "Validation failed.",
            errors,
            traceId = context.TraceIdentifier
        };


        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }



    private static async Task HandleExceptionAsync(
        HttpContext context)
    {
        context.Response.ContentType =
            "application/json";


        context.Response.StatusCode =
            (int)HttpStatusCode.InternalServerError;


        var response = new
        {
            success = false,
            message = "An unexpected error occurred.",
            traceId = context.TraceIdentifier
        };


        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}