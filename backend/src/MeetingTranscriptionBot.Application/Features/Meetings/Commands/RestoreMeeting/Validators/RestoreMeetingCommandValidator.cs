using FluentValidation;

namespace MeetingTranscriptionBot.Application.Features.Meetings
    .Commands.RestoreMeeting.Validators;

public sealed class RestoreMeetingCommandValidator
    : AbstractValidator<RestoreMeetingCommand>
{
    public RestoreMeetingCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Meeting id is required.");
    }
}