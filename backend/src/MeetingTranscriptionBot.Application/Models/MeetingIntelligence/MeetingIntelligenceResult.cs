namespace MeetingTranscriptionBot.Application.Models.MeetingIntelligence;

public sealed record MeetingIntelligenceResult(
    string Summary,
    IReadOnlyCollection<string> KeyPoints,
    IReadOnlyCollection<ActionItemResult> ActionItems,
    IReadOnlyCollection<string> Decisions);