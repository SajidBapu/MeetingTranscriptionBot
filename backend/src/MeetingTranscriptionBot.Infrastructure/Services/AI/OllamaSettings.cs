namespace MeetingTranscriptionBot.Infrastructure.Services.AI;

public sealed class OllamaSettings
{
    public const string SectionName = "Ollama";

    public string BaseUrl { get; init; } = "http://localhost:11434";

    public string Model { get; init; } = "qwen3:4b";
}