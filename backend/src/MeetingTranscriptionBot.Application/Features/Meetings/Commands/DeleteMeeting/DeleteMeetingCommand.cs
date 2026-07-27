using MediatR;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.DeleteMeeting;

public sealed record DeleteMeetingCommand(
    Guid Id
) : IRequest<bool>;