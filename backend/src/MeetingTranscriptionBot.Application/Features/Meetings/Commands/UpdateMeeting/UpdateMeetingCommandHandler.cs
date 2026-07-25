using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.UpdateMeeting;

public sealed class UpdateMeetingCommandHandler
    : IRequestHandler<UpdateMeetingCommand, bool>
{
    private readonly IMeetingRepository _repository;


    public UpdateMeetingCommandHandler(
        IMeetingRepository repository)
    {
        _repository = repository;
    }


    public async Task<bool> Handle(
        UpdateMeetingCommand request,
        CancellationToken cancellationToken)
    {
        var meeting = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);


        if (meeting is null)
        {
            return false;
        }


        meeting.UpdateDetails(
            request.Title,
            request.Description,
            request.Platform,
            request.ScheduledStartTime,
            request.ScheduledEndTime);


        await _repository.UpdateAsync(
            meeting,
            cancellationToken);


        return true;
    }
}