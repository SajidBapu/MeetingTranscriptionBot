using MediatR;
using MeetingTranscriptionBot.Application.Features.MeetingAnalysis.DTOs;

namespace MeetingTranscriptionBot.Application.Features.MeetingAnalysis.Queries.GetMeetingAnalysis;

public sealed record GetMeetingAnalysisQuery(
    Guid MeetingId,
    Guid AnalysisId)
    : IRequest<MeetingAnalysisDto>;