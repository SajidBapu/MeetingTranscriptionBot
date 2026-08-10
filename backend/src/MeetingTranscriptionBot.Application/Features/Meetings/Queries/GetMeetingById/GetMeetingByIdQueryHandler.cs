using AutoMapper;
using MediatR;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetingById;

public sealed class GetMeetingByIdQueryHandler
    : IRequestHandler<GetMeetingByIdQuery, MeetingDto?>
{
    private readonly IMeetingRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetMeetingByIdQueryHandler(
        IMeetingRepository repository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<MeetingDto?> Handle(
        GetMeetingByIdQuery request,
        CancellationToken cancellationToken)
    {
        var meeting = await _repository.GetByIdAsync(
            request.Id,
            _currentUserService.UserId,
            cancellationToken);

        if (meeting is null)
        {
            return null;
        }

        return _mapper.Map<MeetingDto>(meeting);
    }
}