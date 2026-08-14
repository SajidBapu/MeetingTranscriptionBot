using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MeetingTranscriptionBot.Infrastructure.Persistence.Context;

namespace MeetingTranscriptionBot.Infrastructure.Repositories;

public sealed class TranscriptRepository
    : ITranscriptRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TranscriptRepository(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Transcript transcript,
        CancellationToken cancellationToken)
    {
        await _dbContext.Transcripts.AddAsync(
            transcript,
            cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public Task<bool> ExistsForRecordingAsync(
        Guid recordingId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Transcripts
            .AnyAsync(
                transcript =>
                    transcript.RecordingId == recordingId,
                cancellationToken);
    }

    public Task<Transcript?> GetByRecordingIdAsync(
    Guid recordingId,
    CancellationToken cancellationToken)
    {
        return _dbContext.Transcripts
            .AsNoTracking()
            .SingleOrDefaultAsync(
                transcript =>
                    transcript.RecordingId == recordingId,
                cancellationToken);
    }

    public Task<Transcript?> GetByIdAsync(
    Guid transcriptId,
    Guid meetingId,
    Guid ownerId,
    CancellationToken cancellationToken)
    {
        return _dbContext.Transcripts
            .AsNoTracking()
            .Include(x => x.Segments)
            .Where(x =>
                x.Id == transcriptId &&
                x.MeetingId == meetingId &&
                x.Meeting != null &&
                x.Meeting.OwnerId == ownerId)
            .SingleOrDefaultAsync(cancellationToken);
    }
}