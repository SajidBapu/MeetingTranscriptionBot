using FluentValidation;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Commands.CreateMeeting.Validators;

public class CreateMeetingCommandValidator
    : AbstractValidator<CreateMeetingCommand>
{
    public CreateMeetingCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Platform)
            .NotEmpty()
            .WithMessage("Platform is required.")
            .MaximumLength(50)
            .WithMessage("Platform cannot exceed 100 characters.");

        RuleFor(x => x.ScheduledStartTime)
            .NotEmpty()
            .WithMessage("Scheduled start time is required.");

        RuleFor(x => x.ScheduledEndTime)
            .NotEmpty()
            .WithMessage("Scheduled end time is required.");

        RuleFor(x => x)
            .Must(x => x.ScheduledEndTime > x.ScheduledStartTime)
            .WithMessage("Scheduled end time must be after the scheduled start time.");
    }
}