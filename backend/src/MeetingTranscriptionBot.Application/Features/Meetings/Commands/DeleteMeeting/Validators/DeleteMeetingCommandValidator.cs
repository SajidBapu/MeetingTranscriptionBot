using FluentValidation;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.DeleteMeeting.Validators;

public sealed class DeleteMeetingCommandValidator
    : AbstractValidator<DeleteMeetingCommand>
{
    public DeleteMeetingCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Meeting id is required.");
    }
}