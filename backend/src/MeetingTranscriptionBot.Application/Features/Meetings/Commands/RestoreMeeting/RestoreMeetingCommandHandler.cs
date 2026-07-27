using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.RestoreMeeting;

public sealed class RestoreMeetingCommandHandler
    : IRequestHandler<RestoreMeetingCommand, bool>
{
    private readonly IMeetingRepository _repository;


    public RestoreMeetingCommandHandler(
        IMeetingRepository repository)
    {
        _repository = repository;
    }


    public async Task<bool> Handle(
        RestoreMeetingCommand request,
        CancellationToken cancellationToken)
    {
        var meeting = await _repository.GetDeletedByIdAsync(
            request.Id,
            cancellationToken);


        if (meeting is null)
        {
            return false;
        }


        meeting.Restore();


        await _repository.UpdateAsync(
            meeting,
            cancellationToken);


        return true;
    }
}