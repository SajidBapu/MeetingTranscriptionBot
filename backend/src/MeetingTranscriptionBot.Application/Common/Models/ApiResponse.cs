namespace MeetingTranscriptionBot.Application.Common.Models;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public string? Message { get; init; }

    public T? Data { get; init; }

    private ApiResponse(
        bool success,
        string? message,
        T? data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public static ApiResponse<T> Ok(
        T data,
        string? message = null)
    {
        return new ApiResponse<T>(
            true,
            message,
            data);
    }

    public static ApiResponse<T> Fail(
        string message)
    {
        return new ApiResponse<T>(
            false,
            message,
            default);
    }
}