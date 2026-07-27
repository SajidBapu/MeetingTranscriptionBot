using FluentValidation;

namespace MeetingTranscriptionBot.Application.Features.Meetings.Queries.SearchMeetings.Validators;

public sealed class SearchMeetingsQueryValidator
    : AbstractValidator<SearchMeetingsQuery>
{
    public SearchMeetingsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");


        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
    }
}