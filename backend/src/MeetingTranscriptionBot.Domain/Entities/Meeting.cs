using MeetingTranscriptionBot.Domain.Enums;

namespace MeetingTranscriptionBot.Domain.Entities;

public class Meeting
{
    public Guid Id { get; private set; }

    public string Title { get; private set; }

    public string? Description { get; private set; }

    public string Platform { get; private set; }

    public DateTime ScheduledStartTime { get; private set; }

    public DateTime ScheduledEndTime { get; private set; }

    public MeetingStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }


    private Meeting()
    {
        Title = null!;
        Platform = null!;
    }


    public Meeting(
        string title,
        string? description,
        string platform,
        DateTime scheduledStartTime,
        DateTime scheduledEndTime)
    {
        Id = Guid.NewGuid();

        Title = title;

        Description = description;

        Platform = platform;

        ScheduledStartTime = scheduledStartTime;

        ScheduledEndTime = scheduledEndTime;

        Status = MeetingStatus.Scheduled;

        CreatedAt = DateTime.UtcNow;
    }
}