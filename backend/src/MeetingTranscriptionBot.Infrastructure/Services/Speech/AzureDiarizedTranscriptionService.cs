using System.ClientModel;
using Azure.AI.Speech.Transcription;
using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Application.Models.Transcription;
using Microsoft.Extensions.Options;

using AppTranscriptionResult =
    MeetingTranscriptionBot.Application.Models.Transcription.TranscriptionResult;

namespace MeetingTranscriptionBot.Infrastructure.Services.Speech;

public sealed class AzureDiarizedTranscriptionService
    : ITranscriptionService
{
    private readonly AzureSpeechSettings _settings;

    public AzureDiarizedTranscriptionService(
        IOptions<AzureSpeechSettings> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _settings = options.Value;
    }

    public async Task<AppTranscriptionResult> TranscribeAsync(
        Stream audioStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(audioStream);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var endpoint =
            new Uri(_settings.Endpoint);

        var credential =
            new ApiKeyCredential(_settings.Key);

        var clientOptions =
            new TranscriptionClientOptions(
            TranscriptionClientOptions.ServiceVersion.V20251015);

        var client =
            new TranscriptionClient(
                endpoint,
                credential,
                clientOptions);

        var options =
            new TranscriptionOptions(audioStream)
            {
                DiarizationOptions =
                    new TranscriptionDiarizationOptions
                    {
                        MaxSpeakers = 2
                    }
            };

        options.Locales.Add("en-US");

        var response =
            await client.TranscribeAsync(
                options,
                cancellationToken);

        var result =
            response.Value;

        var segments =
            new List<TranscriptionSegmentResult>();

        foreach (var phrase in result.Phrases)
        {
            if (string.IsNullOrWhiteSpace(
                    phrase.Text))
            {
                continue;
            }

            var startTime =
                phrase.Offset;

            var endTime =
                phrase.Offset +
                phrase.Duration;

            var speakerLabel =
                phrase.Speaker.HasValue
                    ? $"Speaker {phrase.Speaker.Value}"
                    : null;

            segments.Add(
                new TranscriptionSegmentResult(
                    phrase.Text,
                    startTime,
                    endTime,
                    speakerLabel));
        }

        segments =
            segments
                .OrderBy(segment =>
                    segment.StartTime)
                .ToList();

        if (segments.Count == 0)
        {
            throw new InvalidOperationException(
                "Azure Speech did not return any transcription segments.");
        }

        var fullText =
            string.Join(
                " ",
                segments.Select(
                    segment => segment.Text));

        return new AppTranscriptionResult(
            fullText,
            "en",
            segments);
    }
}