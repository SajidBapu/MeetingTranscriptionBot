using FluentValidation;

namespace MeetingTranscriptionBot.Application.Features.Transcripts.Commands.TranscribeRecording;

public sealed class TranscribeRecordingCommandValidator
    : AbstractValidator<TranscribeRecordingCommand>
{
    public TranscribeRecordingCommandValidator()
    {
        RuleFor(x => x.MeetingId)
            .NotEmpty();

        RuleFor(x => x.RecordingId)
            .NotEmpty();
    }
}