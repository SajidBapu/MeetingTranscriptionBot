using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Domain.Entities;

namespace MeetingTranscriptionBot.Application.Features.Recordings.Commands.UploadMeetingRecording;

public sealed class UploadMeetingRecordingCommandHandler
    : IRequestHandler<
        UploadMeetingRecordingCommand,
        Guid>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IMeetingRecordingRepository _recordingRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;

    public UploadMeetingRecordingCommandHandler(
        IMeetingRepository meetingRepository,
        IMeetingRecordingRepository recordingRepository,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService)
    {
        _meetingRepository = meetingRepository;
        _recordingRepository = recordingRepository;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(
        UploadMeetingRecordingCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var meeting =
            await _meetingRepository.GetByIdAsync(
                request.MeetingId,
                userId,
                cancellationToken);

        if (meeting is null)
        {
            throw new KeyNotFoundException(
                "Meeting not found.");
        }

        string? storagePath = null;

        try
        {
            storagePath =
                await _fileStorageService.SaveAsync(
                    request.FileStream,
                    request.FileName,
                    request.ContentType,
                    cancellationToken);

            var recording =
                new MeetingRecording(
                    request.MeetingId,
                    request.FileName,
                    storagePath,
                    request.ContentType,
                    request.FileSizeBytes);

            await _recordingRepository.AddAsync(
                recording,
                cancellationToken);

            await _recordingRepository.SaveChangesAsync(
                cancellationToken);

            return recording.Id;
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(storagePath))
            {
                await _fileStorageService.DeleteAsync(
                    storagePath,
                    CancellationToken.None);
            }

            throw;
        }
    }
}