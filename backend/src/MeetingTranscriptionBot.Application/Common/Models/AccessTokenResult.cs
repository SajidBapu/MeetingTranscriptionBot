namespace MeetingTranscriptionBot.Application.Common.Models;

public sealed record AccessTokenResult(
    string Token,
    DateTime ExpiresAtUtc);