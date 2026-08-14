using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Application.Models.Transcription;

namespace MeetingTranscriptionBot.Infrastructure.Services.Speech;

public sealed class DevelopmentTranscriptionService
    : ITranscriptionService
{
    public Task<TranscriptionResult> TranscribeAsync(
        Stream audioStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(audioStream);

        var segments =
            new List<TranscriptionSegmentResult>
            {
                new(
                    "Hello, this is a test meeting recording.",
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(4),
                    "Speaker 1"),

                new(
                    "The transcription pipeline is working successfully.",
                    TimeSpan.FromSeconds(4),
                    TimeSpan.FromSeconds(9),
                    "Speaker 2")
            };

        var result =
            new TranscriptionResult(
                "Hello, this is a test meeting recording. " +
                "The transcription pipeline is working successfully.",
                "en",
                segments);

        return Task.FromResult(result);
    }
}