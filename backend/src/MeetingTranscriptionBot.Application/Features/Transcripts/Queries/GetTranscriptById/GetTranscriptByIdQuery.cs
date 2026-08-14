using MediatR;
using MeetingTranscriptionBot.Application.Features.Transcripts.DTOs;

namespace MeetingTranscriptionBot.Application.Features.Transcripts.Queries.GetTranscriptById;

public sealed record GetTranscriptByIdQuery(
    Guid MeetingId,
    Guid TranscriptId)
    : IRequest<TranscriptDto?>;