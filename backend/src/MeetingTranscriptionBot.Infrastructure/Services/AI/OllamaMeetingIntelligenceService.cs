using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Application.Models.MeetingIntelligence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MeetingTranscriptionBot.Infrastructure.Services.AI;

public sealed class OllamaMeetingIntelligenceService
    : IMeetingIntelligenceService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaSettings _settings;
    private readonly ILogger<OllamaMeetingIntelligenceService> _logger;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

    public OllamaMeetingIntelligenceService(
        HttpClient httpClient,
        IOptions<OllamaSettings> options,
        ILogger<OllamaMeetingIntelligenceService> logger)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<MeetingIntelligenceResult> AnalyzeAsync(
        string transcript,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(transcript))
        {
            throw new ArgumentException(
                "Transcript cannot be empty.",
                nameof(transcript));
        }

        var request = new OllamaChatRequest(
            _settings.Model,
            CreateMessages(transcript),
            CreateResponseSchema(),
            false,
            false,
            "10m");

        _logger.LogInformation(
            "Sending transcript to local Ollama model {Model}.",
            _settings.Model);

        using var timeoutCts =
    CancellationTokenSource.CreateLinkedTokenSource(
        cancellationToken);

        timeoutCts.CancelAfter(TimeSpan.FromMinutes(3));

        using var response =
            await _httpClient.PostAsJsonAsync(
                "/api/chat",
                request,
                JsonOptions,
                timeoutCts.Token);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            _logger.LogError(
                "Ollama request failed with status {StatusCode}. Response: {Response}",
                response.StatusCode,
                error);

            throw new InvalidOperationException(
                "The local AI service failed to analyze the transcript.");
        }

        var ollamaResponse =
            await response.Content.ReadFromJsonAsync<OllamaChatResponse>(
                JsonOptions,
                cancellationToken);

        if (ollamaResponse?.Message is null ||
            string.IsNullOrWhiteSpace(ollamaResponse.Message.Content))
        {
            throw new InvalidOperationException(
                "Ollama returned an empty analysis.");
        }

        MeetingIntelligenceResponse? analysis;

        try
        {
            analysis =
                JsonSerializer.Deserialize<MeetingIntelligenceResponse>(
                    ollamaResponse.Message.Content,
                    JsonOptions);
        }
        catch (JsonException exception)
        {
            _logger.LogError(
                exception,
                "Ollama returned invalid structured JSON.");

            throw new InvalidOperationException(
                "The local AI service returned an invalid analysis format.",
                exception);
        }

        if (analysis is null ||
            string.IsNullOrWhiteSpace(analysis.Summary))
        {
            throw new InvalidOperationException(
                "The local AI service returned an incomplete analysis.");
        }

        var actionItems =
            analysis.ActionItems
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.Description))
                .Select(x => new ActionItemResult(
                    x.Description.Trim(),
                    string.IsNullOrWhiteSpace(x.Assignee)
                        ? null
                        : x.Assignee.Trim(),
                    ParseDueDate(x.DueDateUtc)))
                .ToList();

        return new MeetingIntelligenceResult(
            analysis.Summary.Trim(),
            analysis.KeyPoints
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToList(),
            actionItems,
            analysis.Decisions
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToList());
    }

    private static List<OllamaMessage> CreateMessages(
        string transcript)
    {
        return
        [
            new OllamaMessage(
                "system",
                """
                You are a meeting analysis assistant.

                Analyze meeting transcripts accurately.

                Produce:
                - a concise professional summary
                - important key points
                - decisions that were explicitly made
                - actionable tasks

                For action items:
                - use the speaker name or speaker label as the assignee
                  only when the transcript clearly identifies who owns the task
                - otherwise set assignee to null
                - only provide a due date when one is clearly stated
                - otherwise set dueDateUtc to null

                Do not invent decisions, tasks, people, or deadlines.

                Return only data that conforms to the provided JSON schema.
                """),

            new OllamaMessage(
                "user",
                $"""
                Analyze the following meeting transcript.

                TRANSCRIPT:
                {transcript}
                """)
        ];
    }

    private static object CreateResponseSchema()
    {
        return new
        {
            type = "object",

            properties = new
            {
                summary = new
                {
                    type = "string"
                },

                keyPoints = new
                {
                    type = "array",
                    items = new
                    {
                        type = "string"
                    }
                },

                decisions = new
                {
                    type = "array",
                    items = new
                    {
                        type = "string"
                    }
                },

                actionItems = new
                {
                    type = "array",

                    items = new
                    {
                        type = "object",

                        properties = new
                        {
                            description = new
                            {
                                type = "string"
                            },

                            assignee = new
                            {
                                type = new[]
                                {
                                    "string",
                                    "null"
                                }
                            },

                            dueDateUtc = new
                            {
                                type = new[]
                                {
                                    "string",
                                    "null"
                                }
                            }
                        },

                        required = new[]
                        {
                            "description",
                            "assignee",
                            "dueDateUtc"
                        },

                        additionalProperties = false
                    }
                }
            },

            required = new[]
            {
                "summary",
                "keyPoints",
                "decisions",
                "actionItems"
            },

            additionalProperties = false
        };
    }

    private static DateTime? ParseDueDate(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!DateTime.TryParse(
                value,
                out var parsedDate))
        {
            return null;
        }

        return parsedDate.Kind switch
        {
            DateTimeKind.Utc => parsedDate,

            DateTimeKind.Local =>
                parsedDate.ToUniversalTime(),

            _ =>
                DateTime.SpecifyKind(
                    parsedDate,
                    DateTimeKind.Utc)
        };
    }

    private sealed record OllamaChatRequest(
        [property: JsonPropertyName("model")]
        string Model,

        [property: JsonPropertyName("messages")]
        IReadOnlyCollection<OllamaMessage> Messages,

        [property: JsonPropertyName("format")]
        object Format,

        [property: JsonPropertyName("stream")]
        bool Stream,

        [property: JsonPropertyName("think")]
        bool Think,

        [property: JsonPropertyName("keep_alive")]
        string KeepAlive);

    private sealed record OllamaMessage(
        [property: JsonPropertyName("role")]
        string Role,

        [property: JsonPropertyName("content")]
        string Content);

    private sealed record OllamaChatResponse(
        [property: JsonPropertyName("message")]
        OllamaResponseMessage? Message);

    private sealed record OllamaResponseMessage(
        [property: JsonPropertyName("content")]
        string Content);

    private sealed record MeetingIntelligenceResponse(
        string Summary,
        List<string>? KeyPoints,
        List<string>? Decisions,
        List<ActionItemResponse>? ActionItems);

    private sealed record ActionItemResponse(
        string Description,
        string? Assignee,
        string? DueDateUtc);
}