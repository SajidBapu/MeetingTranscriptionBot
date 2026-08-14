using MediatR;

namespace MeetingTranscriptionBot.Application.Features.Transcripts.Commands.TranscribeRecording;

public sealed record TranscribeRecordingCommand(
    Guid MeetingId,
    Guid RecordingId)
    : IRequest<Guid>;