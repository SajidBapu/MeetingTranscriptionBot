using MeetingTranscriptionBot.Application.Models.Transcription;

namespace MeetingTranscriptionBot.Application.Interfaces;

public interface ITranscriptionService
{
    Task<TranscriptionResult> TranscribeAsync(
        Stream audioStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);
}