namespace MeetingTranscriptionBot.Domain.Entities;

public sealed class TranscriptSegment
{
    private TranscriptSegment()
    {
        // Required by EF Core.
    }

    public TranscriptSegment(
        Guid transcriptId,
        string text,
        TimeSpan startTime,
        TimeSpan endTime,
        string? speakerLabel = null)
    {
        if (transcriptId == Guid.Empty)
        {
            throw new ArgumentException(
                "Transcript ID cannot be empty.",
                nameof(transcriptId));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Transcript segment text is required.",
                nameof(text));
        }

        if (startTime < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(startTime));
        }

        if (endTime <= startTime)
        {
            throw new ArgumentException(
                "End time must be greater than start time.",
                nameof(endTime));
        }

        Id = Guid.NewGuid();
        TranscriptId = transcriptId;
        Text = text.Trim();
        StartTime = startTime;
        EndTime = endTime;
        SpeakerLabel = speakerLabel?.Trim();
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid TranscriptId { get; private set; }

    public string Text { get; private set; } =
        string.Empty;

    public string? SpeakerLabel { get; private set; }

    public TimeSpan StartTime { get; private set; }

    public TimeSpan EndTime { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public Transcript? Transcript { get; private set; }
}