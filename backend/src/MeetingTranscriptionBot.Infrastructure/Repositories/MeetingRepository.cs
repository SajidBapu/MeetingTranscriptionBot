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
                x => x.Id == id && !x.IsDeleted,
                cancellationToken);
    }


    public async Task<(List<Meeting> Items, int TotalCount)> GetAllAsync(
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken)
    {
        var totalCount = await _context.Meetings
            .Where(x => !x.IsDeleted)
            .CountAsync(cancellationToken);

        var meetings = await _context.Meetings
            .Where(x => !x.IsDeleted)
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

    public async Task<(List<Meeting> Items, int TotalCount)> SearchAsync(
    string? title,
    string? platform,
    string? status,
    DateTime? startDate,
    DateTime? endDate,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken)
    {
        var query = _context.Meetings
            .AsQueryable();


        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(x =>
                x.Title.Contains(title));
        }


        if (!string.IsNullOrWhiteSpace(platform))
        {
            query = query.Where(x =>
                x.Platform == platform);
        }


        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x =>
                x.Status.ToString() == status);
        }


        if (startDate.HasValue)
        {
            query = query.Where(x =>
                x.ScheduledStartTime >= startDate.Value);
        }


        if (endDate.HasValue)
        {
            query = query.Where(x =>
                x.ScheduledEndTime <= endDate.Value);
        }


        var totalCount = await query.CountAsync(
            cancellationToken);


        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);


        return (items, totalCount);
    }
}