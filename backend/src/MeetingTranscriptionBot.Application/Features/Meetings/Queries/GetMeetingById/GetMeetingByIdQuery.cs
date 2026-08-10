using MediatR;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetingById;

public sealed record GetMeetingByIdQuery(
    Guid Id)
    : IRequest<MeetingDto?>;