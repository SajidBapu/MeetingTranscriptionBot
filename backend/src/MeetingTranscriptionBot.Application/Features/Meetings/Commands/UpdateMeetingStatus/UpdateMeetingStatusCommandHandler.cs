using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings
    .Commands.UpdateMeetingStatus;

public sealed class UpdateMeetingStatusCommandHandler
    : IRequestHandler<UpdateMeetingStatusCommand, bool>
{
    private readonly IMeetingRepository _repository;

    public UpdateMeetingStatusCommandHandler(
        IMeetingRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        UpdateMeetingStatusCommand request,
        CancellationToken cancellationToken)
    {
        var meeting = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (meeting is null)
        {
            return false;
        }

        meeting.UpdateStatus(request.Status);

        await _repository.UpdateAsync(
            meeting,
            cancellationToken);

        return true;
    }
}