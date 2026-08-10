using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.UpdateMeetingStatus;

public sealed class UpdateMeetingStatusCommandHandler
    : IRequestHandler<UpdateMeetingStatusCommand, bool>
{
    private readonly IMeetingRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateMeetingStatusCommandHandler(
        IMeetingRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(
        UpdateMeetingStatusCommand request,
        CancellationToken cancellationToken)
    {
        var meeting = await _repository.GetByIdAsync(
            request.Id,
            _currentUserService.UserId,
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