using MediatR;

namespace MeetingTranscriptionBot.Application.Features.Recordings.Commands.UploadMeetingRecording;

public sealed record UploadMeetingRecordingCommand(
    Guid MeetingId,
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSizeBytes)
    : IRequest<Guid>;