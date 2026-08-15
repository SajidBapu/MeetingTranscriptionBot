using System.Threading.Channels;
using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Application.Models.Transcription;

namespace MeetingTranscriptionBot.Infrastructure.BackgroundJobs;

public sealed class TranscriptionBackgroundQueue
    : ITranscriptionBackgroundQueue
{
    private readonly Channel<TranscriptionBackgroundWorkItem> _queue;

    public TranscriptionBackgroundQueue()
    {
        var options =
            new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

        _queue =
            Channel.CreateBounded<TranscriptionBackgroundWorkItem>(
                options);
    }

    public async ValueTask QueueAsync(
        Guid meetingId,
        Guid recordingId,
        Guid transcriptId,
        CancellationToken cancellationToken)
    {
        var workItem =
            new TranscriptionBackgroundWorkItem(
                meetingId,
                recordingId,
                transcriptId);

        await _queue.Writer.WriteAsync(
            workItem,
            cancellationToken);
    }

    public async ValueTask<TranscriptionBackgroundWorkItem>
        DequeueAsync(
            CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(
            cancellationToken);
    }
}