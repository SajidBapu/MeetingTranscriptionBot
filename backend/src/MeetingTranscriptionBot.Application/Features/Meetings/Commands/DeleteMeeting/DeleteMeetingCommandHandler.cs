using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.DeleteMeeting;

public sealed class DeleteMeetingCommandHandler
    : IRequestHandler<DeleteMeetingCommand, bool>
{
    private readonly IMeetingRepository _repository;


    public DeleteMeetingCommandHandler(
        IMeetingRepository repository)
    {
        _repository = repository;
    }


    public async Task<bool> Handle(
        DeleteMeetingCommand request,
        CancellationToken cancellationToken)
    {
        var meeting = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);


        if (meeting is null)
        {
            return false;
        }


        meeting.Delete();


        await _repository.UpdateAsync(
            meeting,
            cancellationToken);


        return true;
    }
}