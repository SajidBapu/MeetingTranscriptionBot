using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Domain.Entities;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.CreateMeeting;

public sealed class CreateMeetingCommandHandler
    : IRequestHandler<CreateMeetingCommand, Guid>
{
    private readonly IMeetingRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public CreateMeetingCommandHandler(
        IMeetingRepository repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(
        CreateMeetingCommand request,
        CancellationToken cancellationToken)
    {
        var meeting = new Meeting(
            _currentUserService.UserId,
            request.Title,
            request.Description,
            request.Platform,
            request.ScheduledStartTime,
            request.ScheduledEndTime);

        await _repository.AddAsync(
            meeting,
            cancellationToken);

        return meeting.Id;
    }
}