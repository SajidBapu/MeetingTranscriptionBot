using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.UpdateMeeting;

public sealed class UpdateMeetingCommandHandler
    : IRequestHandler<UpdateMeetingCommand, bool>
{
    private readonly IMeetingRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateMeetingCommandHandler(
        IMeetingRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(
        UpdateMeetingCommand request,
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