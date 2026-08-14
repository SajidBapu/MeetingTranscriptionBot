using MeetingTranscriptionBot.Domain.Entities;

namespace MeetingTranscriptionBot.Application.Interfaces;

public interface ITranscriptRepository
{
    Task AddAsync(
        Transcript transcript,
        CancellationToken cancellationToken);

    Task<Transcript?> GetByRecordingIdAsync(
        Guid recordingId,
        CancellationToken cancellationToken);

    Task<Transcript?> GetByIdAsync(
        Guid transcriptId,
        Guid meetingId,
        Guid ownerId,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}