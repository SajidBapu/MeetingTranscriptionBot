using MediatR;
using MeetingTranscriptionBot.Application.Common.Models;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.SearchMeetings;

public sealed class SearchMeetingsQueryHandler
    : IRequestHandler<SearchMeetingsQuery, PagedResult<MeetingDto>>
{
    private readonly IMeetingRepository _repository;


    public SearchMeetingsQueryHandler(
        IMeetingRepository repository)
    {
        _repository = repository;
    }


    public async Task<PagedResult<MeetingDto>> Handle(
        SearchMeetingsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.SearchAsync(
            request.Title,
            request.Platform,
            request.Status,
            request.StartDate,
            request.EndDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken);


        var items = result.Items
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
               items,
               result.TotalCount,
               request.PageNumber,
               request.PageSize);
    }
    
}