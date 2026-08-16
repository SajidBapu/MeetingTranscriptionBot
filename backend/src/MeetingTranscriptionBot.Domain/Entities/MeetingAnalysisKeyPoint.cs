namespace MeetingTranscriptionBot.Domain.Entities;

public sealed class MeetingAnalysisKeyPoint
{
    private MeetingAnalysisKeyPoint()
    {
    }

    public MeetingAnalysisKeyPoint(
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
                "Key point text is required.",
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