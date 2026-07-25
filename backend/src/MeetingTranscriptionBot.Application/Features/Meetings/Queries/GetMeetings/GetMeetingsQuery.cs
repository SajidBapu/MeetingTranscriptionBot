using MediatR;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetings;

public record GetMeetingsQuery(
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<List<MeetingDto>>;