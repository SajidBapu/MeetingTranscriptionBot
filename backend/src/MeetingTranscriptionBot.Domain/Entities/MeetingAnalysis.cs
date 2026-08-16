namespace MeetingTranscriptionBot.Domain.Entities;

public sealed class MeetingAnalysis
{
    private MeetingAnalysis()
    {
        // Required by EF Core.
    }

    public MeetingAnalysis(
        Guid meetingId,
        Guid transcriptId,
        string summary)
    {
        if (meetingId == Guid.Empty)
        {
            throw new ArgumentException(
                "Meeting ID cannot be empty.",
                nameof(meetingId));
        }

        if (transcriptId == Guid.Empty)
        {
            throw new ArgumentException(
                "Transcript ID cannot be empty.",
                nameof(transcriptId));
        }

        if (string.IsNullOrWhiteSpace(summary))
        {
            throw new ArgumentException(
                "Summary is required.",
                nameof(summary));
        }

        Id = Guid.NewGuid();
        MeetingId = meetingId;
        TranscriptId = transcriptId;
        Summary = summary.Trim();
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid MeetingId { get; private set; }

    public Guid TranscriptId { get; private set; }

    public string Summary { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public Meeting? Meeting { get; private set; }

    public Transcript? Transcript { get; private set; }

    public ICollection<MeetingAnalysisKeyPoint> KeyPoints { get; private set; } =
        new List<MeetingAnalysisKeyPoint>();

    public ICollection<MeetingAnalysisDecision> Decisions { get; private set; } =
        new List<MeetingAnalysisDecision>();

    public ICollection<MeetingActionItem> ActionItems { get; private set; } =
        new List<MeetingActionItem>();

    public void AddKeyPoint(string text)
    {
        KeyPoints.Add(
            new MeetingAnalysisKeyPoint(
                Id,
                text));
    }

    public void AddDecision(string text)
    {
        Decisions.Add(
            new MeetingAnalysisDecision(
                Id,
                text));
    }

    public void AddActionItem(
        string description,
        string? assignee,
        DateTime? dueDateUtc)
    {
        ActionItems.Add(
            new MeetingActionItem(
                Id,
                description,
                assignee,
                dueDateUtc));
    }
}