namespace MeetingTranscriptionBot.Application.Features.MeetingAnalysis.DTOs;

public sealed record MeetingAnalysisDto(
    Guid Id,
    Guid MeetingId,
    Guid TranscriptId,
    string Summary,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<string> KeyPoints,
    IReadOnlyCollection<string> Decisions,
    IReadOnlyCollection<MeetingActionItemDto> ActionItems);