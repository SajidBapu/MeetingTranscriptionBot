using MeetingTranscriptionBot.Application.Models.MeetingIntelligence;

namespace MeetingTranscriptionBot.Application.Interfaces;

public interface IMeetingIntelligenceService
{
    Task<MeetingIntelligenceResult> AnalyzeAsync(
        string transcript,
        CancellationToken cancellationToken);
}