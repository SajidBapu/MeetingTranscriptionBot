namespace MeetingTranscriptionBot.Application.Features.Authentication.DTOs;

public sealed record LoginResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string AccessToken,
    DateTime ExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);