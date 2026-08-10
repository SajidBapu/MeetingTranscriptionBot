using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.RestoreMeeting;

public sealed class RestoreMeetingCommandHandler
    : IRequestHandler<RestoreMeetingCommand, bool>
{
    private readonly IMeetingRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public RestoreMeetingCommandHandler(
        IMeetingRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(
        RestoreMeetingCommand request,
        CancellationToken cancellationToken)
    {
        var meeting = await _repository.GetDeletedByIdAsync(
            request.Id,
            _currentUserService.UserId,
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