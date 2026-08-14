using MediatR;
using MeetingTranscriptionBot.Application.Features.Recordings.DTOs;

namespace MeetingTranscriptionBot.Application.Features.Recordings.Queries.GetRecordingById;

public sealed record GetRecordingByIdQuery(
    Guid MeetingId,
    Guid RecordingId)
    : IRequest<RecordingDto?>;