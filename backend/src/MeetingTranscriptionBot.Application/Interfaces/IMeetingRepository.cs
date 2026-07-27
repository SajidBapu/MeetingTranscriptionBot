using MeetingTranscriptionBot.Domain.Entities;

namespace MeetingTranscriptionBot.Application.Interfaces;

public interface IMeetingRepository
{
    Task<Meeting?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<(List<Meeting> Items, int TotalCount)> GetAllAsync(
         int pageNumber,
         int pageSize,
         CancellationToken cancellationToken);

    Task AddAsync(
        Meeting meeting,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Meeting meeting,
        CancellationToken cancellationToken);

    Task<(List<Meeting> Items, int TotalCount)> SearchAsync(
        string? title,
        string? platform,
        string? status,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<Meeting?> GetDeletedByIdAsync(
    Guid id,
    CancellationToken cancellationToken);
}