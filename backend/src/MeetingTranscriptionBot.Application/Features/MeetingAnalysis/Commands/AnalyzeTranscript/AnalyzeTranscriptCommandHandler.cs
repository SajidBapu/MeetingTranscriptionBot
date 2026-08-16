using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.MeetingAnalysis.Commands.AnalyzeTranscript;

public sealed class AnalyzeTranscriptCommandHandler
    : IRequestHandler<AnalyzeTranscriptCommand, Guid>
{
    private readonly ITranscriptRepository _transcriptRepository;
    private readonly IMeetingAnalysisRepository _meetingAnalysisRepository;
    private readonly IMeetingIntelligenceService _meetingIntelligenceService;
    private readonly ICurrentUserService _currentUserService;

    public AnalyzeTranscriptCommandHandler(
        ITranscriptRepository transcriptRepository,
        IMeetingAnalysisRepository meetingAnalysisRepository,
        IMeetingIntelligenceService meetingIntelligenceService,
        ICurrentUserService currentUserService)
    {
        _transcriptRepository = transcriptRepository;
        _meetingAnalysisRepository = meetingAnalysisRepository;
        _meetingIntelligenceService = meetingIntelligenceService;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(
        AnalyzeTranscriptCommand request,
        CancellationToken cancellationToken)
    {
        var ownerId = _currentUserService.UserId;

        var transcript =
            await _transcriptRepository.GetByIdAsync(
                request.TranscriptId,
                request.MeetingId,
                ownerId,
                cancellationToken);

        if (transcript is null)
        {
            throw new KeyNotFoundException(
                "Transcript was not found.");
        }

        if (!string.Equals(
                transcript.Status,
                "Completed",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The transcript must be completed before analysis can begin.");
        }

        if (string.IsNullOrWhiteSpace(transcript.FullText))
        {
            throw new InvalidOperationException(
                "The completed transcript does not contain any text.");
        }

        var existingAnalysis =
            await _meetingAnalysisRepository.GetByTranscriptIdAsync(
                request.TranscriptId,
                cancellationToken);

        if (existingAnalysis is not null)
        {
            throw new InvalidOperationException(
                "This transcript has already been analyzed.");
        }

        var intelligence =
            await _meetingIntelligenceService.AnalyzeAsync(
                transcript.FullText,
                cancellationToken);

        var analysis =
            new MeetingTranscriptionBot.Domain.Entities.MeetingAnalysis(
                request.MeetingId,
                request.TranscriptId,
                intelligence.Summary);

        foreach (var keyPoint in intelligence.KeyPoints)
        {
            analysis.AddKeyPoint(keyPoint);
        }

        foreach (var decision in intelligence.Decisions)
        {
            analysis.AddDecision(decision);
        }

        foreach (var actionItem in intelligence.ActionItems)
        {
            analysis.AddActionItem(
                actionItem.Description,
                actionItem.Assignee,
                actionItem.DueDateUtc);
        }

        await _meetingAnalysisRepository.AddAsync(
            analysis,
            cancellationToken);

        await _meetingAnalysisRepository.SaveChangesAsync(
            cancellationToken);

        return analysis.Id;
    }
}