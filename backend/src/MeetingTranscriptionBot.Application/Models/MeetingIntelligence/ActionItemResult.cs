namespace MeetingTranscriptionBot.Application.Models.MeetingIntelligence;

public sealed record ActionItemResult(
    string Description,
    string? Assignee,
    DateTime? DueDateUtc);