namespace MeetingTranscriptionBot.Application.Features.Transcripts.DTOs;

public sealed record TranscriptSegmentDto(
    Guid Id,
    string Text,
    string? SpeakerLabel,
    TimeSpan StartTime,
    TimeSpan EndTime);