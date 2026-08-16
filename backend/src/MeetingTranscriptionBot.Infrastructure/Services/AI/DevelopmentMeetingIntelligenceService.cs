using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Application.Models.MeetingIntelligence;

namespace MeetingTranscriptionBot.Infrastructure.Services.AI;

public sealed class DevelopmentMeetingIntelligenceService
    : IMeetingIntelligenceService
{
    public Task<MeetingIntelligenceResult> AnalyzeAsync(
        string transcript,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(transcript))
        {
            throw new ArgumentException(
                "Transcript cannot be empty.",
                nameof(transcript));
        }

        var result = new MeetingIntelligenceResult(
            Summary:
                "The meeting transcript was analyzed successfully. " +
                "The participants discussed the current project status, " +
                "speaker diarization, transcription, and upcoming development work.",

            KeyPoints:
            [
                "The transcription pipeline is functioning.",
                "Speaker diarization is detecting multiple speakers.",
                "The project is moving toward AI-powered meeting analysis."
            ],

            ActionItems:
            [
                new ActionItemResult(
                    "Continue implementation of meeting intelligence features.",
                    null,
                    null),

                new ActionItemResult(
                    "Verify the generated meeting analysis through the API.",
                    null,
                    null)
            ],

            Decisions:
            [
                "Continue using the existing transcription pipeline.",
                "Persist meeting intelligence results in PostgreSQL."
            ]);

        return Task.FromResult(result);
    }
}