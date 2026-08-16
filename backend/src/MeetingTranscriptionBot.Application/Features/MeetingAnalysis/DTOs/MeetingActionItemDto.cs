namespace MeetingTranscriptionBot.Application.Features.MeetingAnalysis.DTOs;

public sealed record MeetingActionItemDto(
    Guid Id,
    string Description,
    string? Assignee,
    DateTime? DueDateUtc);