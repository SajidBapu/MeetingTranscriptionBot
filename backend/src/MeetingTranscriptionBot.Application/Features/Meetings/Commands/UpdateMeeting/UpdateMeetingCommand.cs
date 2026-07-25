using MediatR;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.UpdateMeeting;

public sealed record UpdateMeetingCommand(
    Guid Id,
    string Title,
    string? Description,
    string Platform,
    DateTime ScheduledStartTime,
    DateTime ScheduledEndTime
) : IRequest<bool>;