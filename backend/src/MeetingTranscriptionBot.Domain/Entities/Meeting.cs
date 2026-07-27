using MeetingTranscriptionBot.Domain.Enums;
using MeetingTranscriptionBot.Domain.Common.Exceptions;

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

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }


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

    public void UpdateDetails(
    string title,
    string? description,
    string platform,
    DateTime scheduledStartTime,
    DateTime scheduledEndTime)
    {
        Title = title;
        Description = description;
        Platform = platform;

        ScheduledStartTime =
            DateTime.SpecifyKind(
                scheduledStartTime,
                DateTimeKind.Utc);

        ScheduledEndTime =
            DateTime.SpecifyKind(
                scheduledEndTime,
                DateTimeKind.Utc);

        UpdatedAt = DateTime.UtcNow;
    }

    private bool CanTransitionTo(MeetingStatus newStatus)
    {
        return Status switch
        {
            MeetingStatus.Scheduled =>
                newStatus == MeetingStatus.Starting ||
                newStatus == MeetingStatus.Cancelled,

            MeetingStatus.Starting =>
                newStatus == MeetingStatus.Recording ||
                newStatus == MeetingStatus.Failed,

            MeetingStatus.Recording =>
                newStatus == MeetingStatus.Processing ||
                newStatus == MeetingStatus.Failed,

            MeetingStatus.Processing =>
                newStatus == MeetingStatus.Completed ||
                newStatus == MeetingStatus.Failed,

            MeetingStatus.Completed => false,

            MeetingStatus.Cancelled => false,

            MeetingStatus.Failed => false,

            _ => false
        };
    }

    public void UpdateStatus(MeetingStatus status)
    {
        if (!CanTransitionTo(status))
        {
            throw new BusinessRuleException(
                 $"Cannot change meeting status from {Status} to {status}");
        }

        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        if (IsDeleted)
        {
            throw new BusinessRuleException(
                 "Meeting has already been deleted.");
        }

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (!IsDeleted)
        {
            throw new BusinessRuleException(
                "Meeting is not deleted.");
        }

        IsDeleted = false;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

}