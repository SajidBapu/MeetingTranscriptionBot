using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MeetingTranscriptionBot.Infrastructure.Persistence.Context;

namespace MeetingTranscriptionBot.Infrastructure.Repositories;

public sealed class MeetingRecordingRepository
    : IMeetingRecordingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MeetingRecordingRepository(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        MeetingRecording recording,
        CancellationToken cancellationToken)
    {
        await _dbContext.MeetingRecordings.AddAsync(
            recording,
            cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public Task<MeetingRecording?> GetByIdAsync(
    Guid recordingId,
    Guid meetingId,
    Guid ownerId,
    CancellationToken cancellationToken)
{
    return _dbContext.MeetingRecordings
        .AsNoTracking()
        .Where(recording =>
            recording.Id == recordingId &&
            recording.MeetingId == meetingId &&
            recording.Meeting != null &&
            recording.Meeting.OwnerId == ownerId)
        .SingleOrDefaultAsync(cancellationToken);
}
}