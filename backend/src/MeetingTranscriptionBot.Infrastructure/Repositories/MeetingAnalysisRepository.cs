using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Domain.Entities;
using MeetingTranscriptionBot.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MeetingTranscriptionBot.Infrastructure.Repositories;

public sealed class MeetingAnalysisRepository
    : IMeetingAnalysisRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MeetingAnalysisRepository(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        MeetingAnalysis analysis,
        CancellationToken cancellationToken)
    {
        await _dbContext.MeetingAnalyses.AddAsync(
            analysis,
            cancellationToken);
    }

    public async Task<MeetingAnalysis?> GetByTranscriptIdAsync(
        Guid transcriptId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.MeetingAnalyses
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.TranscriptId == transcriptId,
                cancellationToken);
    }

    public async Task<MeetingAnalysis?> GetByIdAsync(
        Guid analysisId,
        Guid meetingId,
        Guid ownerId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.MeetingAnalyses
            .AsNoTracking()
            .Include(x => x.KeyPoints)
            .Include(x => x.Decisions)
            .Include(x => x.ActionItems)
            .Where(x =>
                x.Id == analysisId &&
                x.MeetingId == meetingId &&
                x.Meeting != null &&
                x.Meeting.OwnerId == ownerId)
            .SingleOrDefaultAsync(
                cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}