using AutoMapper;
using MediatR;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetingStatusHistory;

public sealed class GetMeetingStatusHistoryQueryHandler
    : IRequestHandler<
        GetMeetingStatusHistoryQuery,
        List<MeetingStatusHistoryDto>?>
{
    private readonly IMeetingRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetMeetingStatusHistoryQueryHandler(
        IMeetingRepository repository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<List<MeetingStatusHistoryDto>?> Handle(
    GetMeetingStatusHistoryQuery request,
    CancellationToken cancellationToken)
    {
        var meeting = await _repository.GetByIdAsync(
            request.MeetingId,
            _currentUserService.UserId,
            cancellationToken);

        if (meeting is null)
        {
            return null;
        }

        var history = await _repository.GetStatusHistoryAsync(
            request.MeetingId,
            _currentUserService.UserId,
            cancellationToken);

        return _mapper.Map<List<MeetingStatusHistoryDto>>(
            history);
    }
}