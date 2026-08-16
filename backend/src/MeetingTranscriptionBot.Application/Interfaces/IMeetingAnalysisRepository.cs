using MeetingTranscriptionBot.Domain.Entities;

namespace MeetingTranscriptionBot.Application.Interfaces;

public interface IMeetingAnalysisRepository
{
    Task AddAsync(
        MeetingAnalysis analysis,
        CancellationToken cancellationToken);

    Task<MeetingAnalysis?> GetByTranscriptIdAsync(
        Guid transcriptId,
        CancellationToken cancellationToken);

    Task<MeetingAnalysis?> GetByIdAsync(
        Guid analysisId,
        Guid meetingId,
        Guid ownerId,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}