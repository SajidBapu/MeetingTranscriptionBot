namespace MeetingTranscriptionBot.Application.Features.Meetings.DTOs;

public sealed record MeetingStatusHistoryDto(
    string PreviousStatus,
    string NewStatus,
    DateTime ChangedAt
);