using MeetingTranscriptionBot.Domain.Enums;

namespace MeetingTranscriptionBot.Domain.Entities;

public class MeetingStatusHistory
{
    public Guid Id { get; private set; }

    public Guid MeetingId { get; private set; }

    public Meeting? Meeting { get; private set; }

    public MeetingStatus PreviousStatus { get; private set; }

    public MeetingStatus NewStatus { get; private set; }


    public DateTime ChangedAt { get; private set; }


    private MeetingStatusHistory()
    {
    }


    public MeetingStatusHistory(
        Guid meetingId,
        MeetingStatus previousStatus,
        MeetingStatus newStatus)
    {
        Id = Guid.NewGuid();

        MeetingId = meetingId;

        PreviousStatus = previousStatus;

        NewStatus = newStatus;

        ChangedAt = DateTime.UtcNow;
    }
}