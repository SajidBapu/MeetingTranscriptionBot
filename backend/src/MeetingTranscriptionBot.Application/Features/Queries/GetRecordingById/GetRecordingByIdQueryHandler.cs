using MediatR;
using MeetingTranscriptionBot.Application.Features.Recordings.DTOs;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Recordings.Queries.GetRecordingById;

public sealed class GetRecordingByIdQueryHandler
    : IRequestHandler<GetRecordingByIdQuery, RecordingDto?>
{
    private readonly IMeetingRecordingRepository _recordingRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetRecordingByIdQueryHandler(
        IMeetingRecordingRepository recordingRepository,
        ICurrentUserService currentUserService)
    {
        _recordingRepository = recordingRepository;
        _currentUserService = currentUserService;
    }

    public async Task<RecordingDto?> Handle(
        GetRecordingByIdQuery request,
        CancellationToken cancellationToken)
    {
        var ownerId = _currentUserService.UserId;

        var recording =
            await _recordingRepository.GetByIdAsync(
                request.RecordingId,
                request.MeetingId,
                ownerId,
                cancellationToken);

        if (recording is null)
        {
            return null;
        }

        return new RecordingDto(
            recording.Id,
            recording.MeetingId,
            recording.FileName,
            recording.ContentType,
            recording.FileSizeBytes,
            recording.Duration,
            recording.CreatedAtUtc);
    }
}