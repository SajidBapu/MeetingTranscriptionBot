using MediatR;
using MeetingTranscriptionBot.Application.Common.Models;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetings;

public sealed class GetMeetingsQueryHandler
    : IRequestHandler<GetMeetingsQuery, PagedResult<MeetingDto>>
{
    private readonly IMeetingRepository _repository;

    public GetMeetingsQueryHandler(
        IMeetingRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<MeetingDto>> Handle(
        GetMeetingsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var meetingDtos = result.Items
            .Select(x => new MeetingDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Platform = x.Platform,
                ScheduledStartTime = x.ScheduledStartTime,
                ScheduledEndTime = x.ScheduledEndTime,
                Status = x.Status.ToString(),
                CreatedAt = x.CreatedAt
            })
            .ToList();

        return new PagedResult<MeetingDto>(
            meetingDtos,
            result.TotalCount,
            request.PageNumber,
            request.PageSize);
    }
}