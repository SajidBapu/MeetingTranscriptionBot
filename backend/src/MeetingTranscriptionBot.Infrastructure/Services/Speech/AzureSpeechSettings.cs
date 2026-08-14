namespace MeetingTranscriptionBot.Infrastructure.Services.Speech;

public sealed class AzureSpeechSettings
{
    public const string SectionName = "AzureSpeech";

    public string Key { get; init; } = string.Empty;

    public string Region { get; init; } = string.Empty;

    public string Endpoint { get; init; } = string.Empty;
}