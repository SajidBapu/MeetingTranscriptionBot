using MeetingTranscriptionBot.Domain.Entities;

namespace MeetingTranscriptionBot.Application.Interfaces;

public interface IMeetingRecordingRepository
{
    Task AddAsync(
        MeetingRecording recording,
        CancellationToken cancellationToken);

    Task<MeetingRecording?> GetByIdAsync(
        Guid recordingId,
        Guid meetingId,
        Guid ownerId,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}