namespace MeetingTranscriptionBot.Application.Common.Models;

public sealed class ApiResponse<T>
{
    public bool Success { get; }

    public string? Message { get; }

    public T? Data { get; }


    private ApiResponse(
        bool success,
        T? data,
        string? message)
    {
        Success = success;
        Data = data;
        Message = message;
    }


    public static ApiResponse<T> Ok(
        T data,
        string? message = null)
    {
        return new ApiResponse<T>(
            true,
            data,
            message);
    }


    public static ApiResponse<T> Fail(
        string message)
    {
        return new ApiResponse<T>(
            false,
            default,
            message);
    }
}