using MeetingTranscriptionBot.Domain.Enums;

namespace MeetingTranscriptionBot.Application.Features.Meetings.DTOs;

public sealed record UpdateMeetingStatusDto(
    MeetingStatus Status
);