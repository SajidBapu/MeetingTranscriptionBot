using MediatR;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetingById;

public class GetMeetingByIdQueryHandler
    : IRequestHandler<GetMeetingByIdQuery, MeetingDto?>
{
    private readonly IMeetingRepository _repository;

    public GetMeetingByIdQueryHandler(
        IMeetingRepository repository)
    {
        _repository = repository;
    }

    public async Task<MeetingDto?> Handle(
        GetMeetingByIdQuery request,
        CancellationToken cancellationToken)
    {
        var meeting = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (meeting is null)
        {
            return null;
        }

        return new MeetingDto
        {
            Id = meeting.Id,
            Title = meeting.Title,
            Description = meeting.Description,
            Platform = meeting.Platform,
            ScheduledStartTime = meeting.ScheduledStartTime,
            ScheduledEndTime = meeting.ScheduledEndTime,
            Status = meeting.Status.ToString(),
            CreatedAt = meeting.CreatedAt
        };
    }
}