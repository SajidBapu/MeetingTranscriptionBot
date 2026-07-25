using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Domain.Entities;
using MeetingTranscriptionBot.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MeetingTranscriptionBot.Infrastructure.Repositories;

public class MeetingRepository : IMeetingRepository
{
    private readonly ApplicationDbContext _context;

    public MeetingRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Meeting?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Meetings
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }


    public async Task<(List<Meeting> Items, int TotalCount)> GetAllAsync(
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken)
    {
        var totalCount = await _context.Meetings
            .CountAsync(cancellationToken);

        var meetings = await _context.Meetings
            .OrderBy(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (meetings, totalCount);
    }


    public async Task AddAsync(
        Meeting meeting,
        CancellationToken cancellationToken)
    {
        await _context.Meetings.AddAsync(
            meeting,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }


    public async Task UpdateAsync(
        Meeting meeting,
        CancellationToken cancellationToken)
    {
        _context.Meetings.Update(meeting);

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}