using MediatR;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.CreateMeeting;

public record CreateMeetingCommand(
    string Title,
    string? Description,
    string Platform,
    DateTime ScheduledStartTime,
    DateTime ScheduledEndTime
) : IRequest<Guid>;