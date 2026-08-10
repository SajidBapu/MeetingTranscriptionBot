using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.DeleteMeeting;

public sealed class DeleteMeetingCommandHandler
    : IRequestHandler<DeleteMeetingCommand, bool>
{
    private readonly IMeetingRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteMeetingCommandHandler(
        IMeetingRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(
        DeleteMeetingCommand request,
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

        meeting.Delete();

        await _repository.UpdateAsync(
            meeting,
            cancellationToken);

        return true;
    }
}