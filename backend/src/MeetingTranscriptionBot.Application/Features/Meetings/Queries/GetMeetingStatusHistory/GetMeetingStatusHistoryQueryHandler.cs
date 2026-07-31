using MediatR;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetingStatusHistory;

public sealed class GetMeetingStatusHistoryQueryHandler
    : IRequestHandler<GetMeetingStatusHistoryQuery, List<MeetingStatusHistoryDto>>
{
    private readonly IMeetingRepository _repository;


    public GetMeetingStatusHistoryQueryHandler(
        IMeetingRepository repository)
    {
        _repository = repository;
    }


    public async Task<List<MeetingStatusHistoryDto>> Handle(
        GetMeetingStatusHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var history = await _repository.GetStatusHistoryAsync(
            request.MeetingId,
            cancellationToken);


        return history
            .Select(x => new MeetingStatusHistoryDto(
                x.PreviousStatus.ToString(),
                x.NewStatus.ToString(),
                x.ChangedAt))
            .ToList();
    }
}