using FluentValidation;
using MeetingTranscriptionBot.Domain.Enums;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.UpdateMeetingStatus.Validators;

public sealed class UpdateMeetingStatusCommandValidator
    : AbstractValidator<UpdateMeetingStatusCommand>
{
    public UpdateMeetingStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Meeting id is required.");


        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid meeting status.");
    }
}