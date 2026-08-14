namespace MeetingTranscriptionBot.Application.Models.Transcription;

public sealed record TranscriptionSegmentResult(
    string Text,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string? SpeakerLabel);