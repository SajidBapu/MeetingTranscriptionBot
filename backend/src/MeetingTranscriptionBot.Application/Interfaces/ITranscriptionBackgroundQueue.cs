namespace MeetingTranscriptionBot.Application.Interfaces;

using MeetingTranscriptionBot.Application.Models.Transcription;

public interface ITranscriptionBackgroundQueue
{
    ValueTask QueueAsync(
        Guid meetingId,
        Guid recordingId,
        Guid transcriptId,
        CancellationToken cancellationToken);

    ValueTask<TranscriptionBackgroundWorkItem> DequeueAsync(
        CancellationToken cancellationToken);
}