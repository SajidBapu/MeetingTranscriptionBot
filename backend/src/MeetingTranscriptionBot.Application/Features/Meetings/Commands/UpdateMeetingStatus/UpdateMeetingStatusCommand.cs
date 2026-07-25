using MediatR;
using MeetingTranscriptionBot.Domain.Enums;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.UpdateMeetingStatus;

public sealed record UpdateMeetingStatusCommand(
    Guid Id,
    MeetingStatus Status
) : IRequest<bool>;