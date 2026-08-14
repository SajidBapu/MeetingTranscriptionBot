using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Application.Models.Transcription;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Options;

namespace MeetingTranscriptionBot.Infrastructure.Services.Speech;

public sealed class AzureSpeechTranscriptionService
    : ITranscriptionService
{
    private readonly AzureSpeechSettings _settings;

    public AzureSpeechTranscriptionService(
        IOptions<AzureSpeechSettings> options)
    {
        _settings = options.Value;
    }

    public async Task<TranscriptionResult> TranscribeAsync(
        Stream audioStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(audioStream);

        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));
        }

        var extension =
            Path.GetExtension(fileName);

        if (!string.Equals(
                extension,
                ".wav",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException(
                "The initial Azure Speech implementation supports WAV files only.");
        }

        var temporaryFilePath =
            Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}.wav");

        try
        {
            await using (
                var outputStream =
                    new FileStream(
                        temporaryFilePath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None,
                        bufferSize: 81920,
                        useAsync: true))
            {
                await audioStream.CopyToAsync(
                    outputStream,
                    cancellationToken);
            }

            var speechConfig =
                SpeechConfig.FromSubscription(
                    _settings.Key,
                    _settings.Region);

            speechConfig.SpeechRecognitionLanguage =
                "en-US";

            using var audioConfig =
                AudioConfig.FromWavFileInput(
                    temporaryFilePath);

            using var recognizer =
                new SpeechRecognizer(
                    speechConfig,
                    audioConfig);

            var segments =
                new List<TranscriptionSegmentResult>();

            var fullTextParts =
                new List<string>();

            var recognitionCompleted =
                new TaskCompletionSource<bool>(
                    TaskCreationOptions
                        .RunContinuationsAsynchronously);

            recognizer.Recognized += (_, eventArgs) =>
            {
                if (eventArgs.Result.Reason !=
                    ResultReason.RecognizedSpeech)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(
                    eventArgs.Result.Text))
                {
                    return;
                }

                var startTime =
                    TimeSpan.FromTicks(
                        eventArgs.Result.OffsetInTicks);

                var endTime =
                    startTime +
                    eventArgs.Result.Duration;

                segments.Add(
                    new TranscriptionSegmentResult(
                        eventArgs.Result.Text,
                        startTime,
                        endTime,
                        null));

                fullTextParts.Add(
                    eventArgs.Result.Text);
            };

            recognizer.Canceled += (_, eventArgs) =>
            {
                if (eventArgs.Reason ==
                    CancellationReason.EndOfStream)
                {
                    recognitionCompleted.TrySetResult(true);
                    return;
                }

                recognitionCompleted.TrySetException(
                    new InvalidOperationException(
                        $"Azure Speech recognition failed. " +
                        $"Reason: {eventArgs.Reason}. " +
                        $"ErrorCode: {eventArgs.ErrorCode}. " +
                        $"Details: {eventArgs.ErrorDetails}"));
            };

            recognizer.SessionStopped += (_, _) =>
            {
                recognitionCompleted.TrySetResult(true);
            };

            await recognizer
                .StartContinuousRecognitionAsync();

            using var registration =
                cancellationToken.Register(
                    () =>
                        recognitionCompleted
                            .TrySetCanceled(
                                cancellationToken));

            await recognitionCompleted.Task;

            await recognizer
                .StopContinuousRecognitionAsync();

            if (segments.Count == 0)
            {
                throw new InvalidOperationException(
                    "Azure Speech did not recognize any speech.");
            }

            return new TranscriptionResult(
                string.Join(" ", fullTextParts),
                "en",
                segments);
        }
        finally
        {
            if (File.Exists(temporaryFilePath))
            {
                File.Delete(temporaryFilePath);
            }
        }
    }
}