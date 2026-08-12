namespace MeetingTranscriptionBot.Application.Features.Authentication.DTOs;

public sealed record LogoutRequest(
    string RefreshToken);