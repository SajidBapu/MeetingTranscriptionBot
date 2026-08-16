namespace MeetingTranscriptionBot.Domain.Entities;

public sealed class MeetingActionItem
{
    private MeetingActionItem()
    {
    }

    public MeetingActionItem(
        Guid meetingAnalysisId,
        string description,
        string? assignee,
        DateTime? dueDateUtc)
    {
        if (meetingAnalysisId == Guid.Empty)
        {
            throw new ArgumentException(
                "Meeting analysis ID cannot be empty.",
                nameof(meetingAnalysisId));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Action item description is required.",
                nameof(description));
        }

        Id = Guid.NewGuid();
        MeetingAnalysisId = meetingAnalysisId;
        Description = description.Trim();
        Assignee = string.IsNullOrWhiteSpace(assignee)
            ? null
            : assignee.Trim();
        DueDateUtc = dueDateUtc;
    }

    public Guid Id { get; private set; }

    public Guid MeetingAnalysisId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public string? Assignee { get; private set; }

    public DateTime? DueDateUtc { get; private set; }

    public MeetingAnalysis? MeetingAnalysis { get; private set; }
}