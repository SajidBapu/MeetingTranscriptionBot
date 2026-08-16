using FluentValidation;

namespace MeetingTranscriptionBot.Application.Features.MeetingAnalysis.Commands.AnalyzeTranscript;

public sealed class AnalyzeTranscriptCommandValidator
    : AbstractValidator<AnalyzeTranscriptCommand>
{
    public AnalyzeTranscriptCommandValidator()
    {
        RuleFor(x => x.MeetingId)
            .NotEmpty();

        RuleFor(x => x.TranscriptId)
            .NotEmpty();
    }
}