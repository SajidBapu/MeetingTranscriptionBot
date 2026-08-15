using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MeetingTranscriptionBot.Infrastructure.BackgroundJobs;

public sealed class TranscriptionBackgroundWorker
    : BackgroundService
{
    private readonly ITranscriptionBackgroundQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TranscriptionBackgroundWorker> _logger;

    public TranscriptionBackgroundWorker(
        ITranscriptionBackgroundQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<TranscriptionBackgroundWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Transcription background worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem =
                    await _queue.DequeueAsync(stoppingToken);

                await ProcessAsync(
                    workItem.TranscriptId,
                    workItem.RecordingId,
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Unexpected error in transcription background worker.");
            }
        }

        _logger.LogInformation(
            "Transcription background worker stopped.");
    }

    private async Task ProcessAsync(
        Guid transcriptId,
        Guid recordingId,
        CancellationToken cancellationToken)
    {
        await using var scope =
            _scopeFactory.CreateAsyncScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

        var fileStorageService =
            scope.ServiceProvider
                .GetRequiredService<IFileStorageService>();

        var transcriptionService =
            scope.ServiceProvider
                .GetRequiredService<ITranscriptionService>();

        var transcript =
            await dbContext.Transcripts
                .Include(x => x.Segments)
                .SingleOrDefaultAsync(
                    x => x.Id == transcriptId,
                    cancellationToken);

        if (transcript is null)
        {
            _logger.LogWarning(
                "Transcript {TranscriptId} was not found.",
                transcriptId);

            return;
        }

        var recording =
            await dbContext.MeetingRecordings
                .SingleOrDefaultAsync(
                    x => x.Id == recordingId,
                    cancellationToken);

        if (recording is null)
        {
            transcript.MarkFailed();

            await dbContext.SaveChangesAsync(
                cancellationToken);

            _logger.LogWarning(
                "Recording {RecordingId} was not found.",
                recordingId);

            return;
        }

        try
        {
            transcript.MarkProcessing();

            await dbContext.SaveChangesAsync(
                cancellationToken);

            await using var audioStream =
                await fileStorageService.OpenReadAsync(
                    recording.StoragePath,
                    cancellationToken);

            var result =
                await transcriptionService.TranscribeAsync(
                    audioStream,
                    recording.FileName,
                    recording.ContentType,
                    cancellationToken);

            foreach (var segment in result.Segments)
            {
                var transcriptSegment =
                    transcript.AddSegment(
                        segment.Text,
                        segment.StartTime,
                        segment.EndTime,
                        segment.SpeakerLabel);

                dbContext.TranscriptSegments.Add(
                    transcriptSegment);
            }

            transcript.Complete(
                result.FullText);

            await dbContext.SaveChangesAsync(
                cancellationToken);

            _logger.LogInformation(
                "Transcript {TranscriptId} completed successfully.",
                transcriptId);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            transcript.MarkFailed();

            await dbContext.SaveChangesAsync(
                CancellationToken.None);

            _logger.LogError(
                exception,
                "Transcript {TranscriptId} failed.",
                transcriptId);
        }
    }
}