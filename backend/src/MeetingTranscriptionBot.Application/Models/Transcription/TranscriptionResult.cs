namespace MeetingTranscriptionBot.Application.Models.Transcription;

public sealed record TranscriptionResult(
    string FullText,
    string Language,
    IReadOnlyCollection<TranscriptionSegmentResult> Segments);