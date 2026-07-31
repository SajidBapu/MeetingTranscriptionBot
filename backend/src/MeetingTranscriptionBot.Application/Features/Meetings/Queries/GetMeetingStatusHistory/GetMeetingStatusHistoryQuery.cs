using MediatR;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetingStatusHistory;

public sealed record GetMeetingStatusHistoryQuery(
    Guid MeetingId
) : IRequest<List<MeetingStatusHistoryDto>>;