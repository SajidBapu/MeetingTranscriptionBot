namespace MeetingTranscriptionBot.Application.Features.Authentication.DTOs;

public sealed record RefreshTokenResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);