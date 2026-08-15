namespace MeetingTranscriptionBot.Application.Models.Transcription;

public sealed record TranscriptionBackgroundWorkItem(
    Guid MeetingId,
    Guid RecordingId,
    Guid TranscriptId);