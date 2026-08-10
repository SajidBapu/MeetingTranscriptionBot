using MediatR;
using AutoMapper;
using MeetingTranscriptionBot.Application.Common.Models;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.SearchMeetings;

public sealed class SearchMeetingsQueryHandler
    : IRequestHandler<SearchMeetingsQuery, PagedResult<MeetingDto>>
{
    private readonly IMeetingRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;


    public SearchMeetingsQueryHandler(
    IMeetingRepository repository,
    ICurrentUserService currentUserService,
    IMapper mapper)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }


    public async Task<PagedResult<MeetingDto>> Handle(
        SearchMeetingsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.SearchAsync(
              _currentUserService.UserId,
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