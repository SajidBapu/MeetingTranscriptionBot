namespace MeetingTranscriptionBot.Domain.Entities;

public sealed class Transcript
{
    private Transcript()
    {
        // Required by EF Core.
    }

    public Transcript(
        Guid meetingId,
        Guid recordingId,
        string language)
    {
        if (meetingId == Guid.Empty)
        {
            throw new ArgumentException(
                "Meeting ID cannot be empty.",
                nameof(meetingId));
        }

        if (recordingId == Guid.Empty)
        {
            throw new ArgumentException(
                "Recording ID cannot be empty.",
                nameof(recordingId));
        }

        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentException(
                "Language is required.",
                nameof(language));
        }

        Id = Guid.NewGuid();
        MeetingId = meetingId;
        RecordingId = recordingId;
        Language = language.Trim();
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid MeetingId { get; private set; }

    public Guid RecordingId { get; private set; }

    public string Language { get; private set; } =
        string.Empty;

    public string? FullText { get; private set; }

    public string Status { get; private set; } = "Pending";

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    public Meeting? Meeting { get; private set; }

    public MeetingRecording? Recording { get; private set; }

    public ICollection<TranscriptSegment> Segments { get; private set; } =
        new List<TranscriptSegment>();

    public void Complete(
    string fullText)
    {
        if (string.IsNullOrWhiteSpace(fullText))
        {
            throw new ArgumentException(
                "Transcript text is required.",
                nameof(fullText));
        }

        FullText = fullText.Trim();
        CompletedAtUtc = DateTime.UtcNow;
        Status = "Completed";
    }

    public TranscriptSegment AddSegment(
    string text,
    TimeSpan startTime,
    TimeSpan endTime,
    string? speakerLabel = null)
    {
        var segment =
            new TranscriptSegment(
                Id,
                text,
                startTime,
                endTime,
                speakerLabel);

        Segments.Add(segment);

        return segment;
    }

    public void MarkProcessing()
    {
        Status = "Processing";
    }

    public void MarkCompleted()
    {
        Status = "Completed";
    }

    public void MarkFailed()
    {
        Status = "Failed";
    }
}