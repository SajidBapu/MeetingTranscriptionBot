using MediatR;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.RestoreMeeting;

public sealed record RestoreMeetingCommand(
    Guid Id
) : IRequest<bool>;