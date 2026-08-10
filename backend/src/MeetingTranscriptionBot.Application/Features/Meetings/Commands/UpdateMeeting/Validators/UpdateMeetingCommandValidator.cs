using FluentValidation;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.UpdateMeeting.Validators;

public sealed class UpdateMeetingCommandValidator
    : AbstractValidator<UpdateMeetingCommand>
{
    public UpdateMeetingCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Meeting id is required.");


        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Meeting title is required.")
            .MaximumLength(200)
            .WithMessage("Meeting title cannot exceed 200 characters.");


        RuleFor(x => x.Platform)
            .NotEmpty()
            .WithMessage("Meeting platform is required.");


        RuleFor(x => x.ScheduledStartTime)
            .NotEmpty()
            .WithMessage("Scheduled start time is required.");


        RuleFor(x => x.ScheduledEndTime)
            .NotEmpty()
            .WithMessage("Scheduled end time is required.");


        RuleFor(x => x)
            .Must(x => x.ScheduledEndTime > x.ScheduledStartTime)
            .WithMessage(
                "Scheduled end time must be after start time.");

        RuleFor(x => x.Description)
           .MaximumLength(1000)
           .WithMessage(
           "Description cannot exceed 1000 characters.");

        RuleFor(x => x.Platform)
            .NotEmpty()
            .WithMessage("Meeting platform is required.")
            .MaximumLength(50)
            .WithMessage(
                "Meeting platform cannot exceed 50 characters.");
    }
}