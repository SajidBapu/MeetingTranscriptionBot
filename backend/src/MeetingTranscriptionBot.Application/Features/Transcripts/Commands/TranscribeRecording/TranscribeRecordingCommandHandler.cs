using MediatR;
using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Domain.Entities;

namespace MeetingTranscriptionBot.Application.Features.Transcripts.Commands.TranscribeRecording;

public sealed class TranscribeRecordingCommandHandler
    : IRequestHandler<TranscribeRecordingCommand, Guid>
{
    private readonly IMeetingRecordingRepository _recordingRepository;
    private readonly ITranscriptRepository _transcriptRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ITranscriptionService _transcriptionService;
    private readonly ICurrentUserService _currentUserService;

    public TranscribeRecordingCommandHandler(
        IMeetingRecordingRepository recordingRepository,
        ITranscriptRepository transcriptRepository,
        IFileStorageService fileStorageService,
        ITranscriptionService transcriptionService,
        ICurrentUserService currentUserService)
    {
        _recordingRepository = recordingRepository;
        _transcriptRepository = transcriptRepository;
        _fileStorageService = fileStorageService;
        _transcriptionService = transcriptionService;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(
        TranscribeRecordingCommand request,
        CancellationToken cancellationToken)
    {
        var ownerId =
            _currentUserService.UserId;

        var recording =
            await _recordingRepository.GetByIdAsync(
                request.RecordingId,
                request.MeetingId,
                ownerId,
                cancellationToken);

        if (recording is null)
        {
            throw new KeyNotFoundException(
                "Recording not found.");
        }

        var existingTranscript =
            await _transcriptRepository.GetByRecordingIdAsync(
                request.RecordingId,
                cancellationToken);

        if (existingTranscript is not null)
        {
            throw new InvalidOperationException(
                "This recording has already been transcribed.");
        }

        await using var audioStream =
            await _fileStorageService.OpenReadAsync(
                recording.StoragePath,
                cancellationToken);

        var result =
            await _transcriptionService.TranscribeAsync(
                audioStream,
                recording.FileName,
                recording.ContentType,
                cancellationToken);

        var transcript =
            new Transcript(
                request.MeetingId,
                request.RecordingId,
                result.Language);

        foreach (var segment in result.Segments)
        {
            transcript.AddSegment(
                segment.Text,
                segment.StartTime,
                segment.EndTime,
                segment.SpeakerLabel);
        }

        transcript.Complete(
            result.FullText);

        await _transcriptRepository.AddAsync(
            transcript,
            cancellationToken);

        await _transcriptRepository.SaveChangesAsync(
            cancellationToken);

        return transcript.Id;
    }
}