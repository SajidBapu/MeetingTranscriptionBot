using MediatR;

namespace MeetingTranscriptionBot.Application.Features.MeetingAnalysis.Commands.AnalyzeTranscript;

public sealed record AnalyzeTranscriptCommand(
    Guid MeetingId,
    Guid TranscriptId)
    : IRequest<Guid>;