using MeetingTranscriptionBot.Application.Common.Models;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;
using MediatR;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.SearchMeetings;

public sealed record SearchMeetingsQuery(
    string? Title,
    string? Platform,
    string? Status,
    DateTime? StartDate,
    DateTime? EndDate,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PagedResult<MeetingDto>>;