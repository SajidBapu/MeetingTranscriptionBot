namespace MeetingTranscriptionBot.Domain.Entities;

public sealed class MeetingRecording
{
    private MeetingRecording()
    {
        // Required by EF Core.
    }

    public MeetingRecording(
        Guid meetingId,
        string fileName,
        string storagePath,
        string contentType,
        long fileSizeBytes)
    {
        if (meetingId == Guid.Empty)
        {
            throw new ArgumentException(
                "Meeting ID cannot be empty.",
                nameof(meetingId));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(storagePath))
        {
            throw new ArgumentException(
                "Storage path is required.",
                nameof(storagePath));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException(
                "Content type is required.",
                nameof(contentType));
        }

        if (fileSizeBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fileSizeBytes),
                "File size must be greater than zero.");
        }

        Id = Guid.NewGuid();
        MeetingId = meetingId;
        FileName = fileName.Trim();
        StoragePath = storagePath.Trim();
        ContentType = contentType.Trim();
        FileSizeBytes = fileSizeBytes;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid MeetingId { get; private set; }

    public string FileName { get; private set; } =
        string.Empty;

    public string StoragePath { get; private set; } =
        string.Empty;

    public string ContentType { get; private set; } =
        string.Empty;

    public long FileSizeBytes { get; private set; }

    public TimeSpan? Duration { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public Meeting? Meeting { get; private set; }

    public void SetDuration(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(duration));
        }

        Duration = duration;
    }
}