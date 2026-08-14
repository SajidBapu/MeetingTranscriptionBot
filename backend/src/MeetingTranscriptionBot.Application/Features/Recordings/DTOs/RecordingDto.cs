namespace MeetingTranscriptionBot.Application.Features.Recordings.DTOs;

public sealed record RecordingDto(
    Guid Id,
    Guid MeetingId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    TimeSpan? Duration,
    DateTime CreatedAtUtc);