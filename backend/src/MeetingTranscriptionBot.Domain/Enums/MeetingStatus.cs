using MeetingTranscriptionBot.Domain.Enums;
namespace MeetingTranscriptionBot.Domain.Enums;

public enum MeetingStatus
{
    Scheduled = 1,

    Starting = 2,

    Recording = 3,

    Processing = 4,

    Completed = 5,

    Failed = 6,

    Cancelled = 7
}