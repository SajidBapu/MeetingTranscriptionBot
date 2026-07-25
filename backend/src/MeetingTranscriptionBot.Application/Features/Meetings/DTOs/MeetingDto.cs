namespace MeetingTranscriptionBot.Application.Features.Meetings.DTOs;

public class MeetingDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Platform { get; set; } = null!;

    public DateTime ScheduledStartTime { get; set; }

    public DateTime ScheduledEndTime { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}