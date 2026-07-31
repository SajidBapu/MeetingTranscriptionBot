using FluentValidation;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetingStatusHistory.Validators;

public sealed class GetMeetingStatusHistoryQueryValidator
    : AbstractValidator<GetMeetingStatusHistoryQuery>
{
    public GetMeetingStatusHistoryQueryValidator()
    {
        RuleFor(x => x.MeetingId)
            .NotEmpty()
            .WithMessage("Meeting id is required.");
    }
}