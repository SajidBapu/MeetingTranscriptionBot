using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Domain.Entities;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.CreateMeeting;

public class CreateMeetingCommandHandler
    : IRequestHandler<CreateMeetingCommand, Guid>
{
    private readonly IMeetingRepository _repository;

    public CreateMeetingCommandHandler(
        IMeetingRepository repository)
    {
        _repository = repository;
    }


    public async Task<Guid> Handle(
        CreateMeetingCommand request,
        CancellationToken cancellationToken)
    {
        var meeting = new Meeting(
           request.Title,
           request.Description,
           request.Platform,
        DateTime.SpecifyKind(
           request.ScheduledStartTime,
           DateTimeKind.Utc),
        DateTime.SpecifyKind(
           request.ScheduledEndTime,
           DateTimeKind.Utc));


        await _repository.AddAsync(
            meeting,
            cancellationToken);


        return meeting.Id;
    }
}