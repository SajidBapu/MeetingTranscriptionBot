namespace MeetingTranscriptionBot.Domain.Entities;

public sealed class MeetingAnalysisDecision
{
    private MeetingAnalysisDecision()
    {
    }

    public MeetingAnalysisDecision(
        Guid meetingAnalysisId,
        string text)
    {
        if (meetingAnalysisId == Guid.Empty)
        {
            throw new ArgumentException(
                "Meeting analysis ID cannot be empty.",
                nameof(meetingAnalysisId));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Decision text is required.",
                nameof(text));
        }

        Id = Guid.NewGuid();
        MeetingAnalysisId = meetingAnalysisId;
        Text = text.Trim();
    }

    public Guid Id { get; private set; }

    public Guid MeetingAnalysisId { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public MeetingAnalysis? MeetingAnalysis { get; private set; }
}