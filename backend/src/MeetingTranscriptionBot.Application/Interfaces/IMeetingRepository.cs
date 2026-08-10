using MeetingTranscriptionBot.Domain.Entities;

namespace MeetingTranscriptionBot.Application.Interfaces;

public interface IMeetingRepository
{
    Task<Meeting?> GetByIdAsync(
        Guid id,
        Guid ownerId,
        CancellationToken cancellationToken);

    Task<(List<Meeting> Items, int TotalCount)> GetAllAsync(
        Guid ownerId,
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
        Guid ownerId,
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
        Guid ownerId,
        CancellationToken cancellationToken);

    Task<List<MeetingStatusHistory>> GetStatusHistoryAsync(
    Guid meetingId,
    Guid ownerId,
    CancellationToken cancellationToken);

}