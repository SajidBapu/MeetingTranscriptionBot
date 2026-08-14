namespace MeetingTranscriptionBot.Application.Features.Transcripts.DTOs;

public sealed record TranscriptDto(
    Guid Id,
    Guid MeetingId,
    Guid RecordingId,
    string Language,
    string? FullText,
    DateTime CreatedAtUtc,
    DateTime? CompletedAtUtc,
    IReadOnlyCollection<TranscriptSegmentDto> Segments);