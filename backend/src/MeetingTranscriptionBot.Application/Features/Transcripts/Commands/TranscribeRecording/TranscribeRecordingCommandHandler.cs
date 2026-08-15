using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Domain.Entities;
using MediatR;

namespace MeetingTranscriptionBot.Application.Features.Transcripts.Commands.TranscribeRecording;

public sealed class TranscribeRecordingCommandHandler
    : IRequestHandler<TranscribeRecordingCommand, Guid>
{
    private readonly IMeetingRecordingRepository _recordingRepository;
    private readonly ITranscriptRepository _transcriptRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITranscriptionBackgroundQueue _backgroundQueue;

    public TranscribeRecordingCommandHandler(
        IMeetingRecordingRepository recordingRepository,
        ITranscriptRepository transcriptRepository,
        ICurrentUserService currentUserService,
        ITranscriptionBackgroundQueue backgroundQueue)
    {
        _recordingRepository = recordingRepository;
        _transcriptRepository = transcriptRepository;
        _currentUserService = currentUserService;
        _backgroundQueue = backgroundQueue;
    }

    public async Task<Guid> Handle(
        TranscribeRecordingCommand request,
        CancellationToken cancellationToken)
    {
        var userId =
            _currentUserService.UserId;

        var recording =
            await _recordingRepository.GetByIdAsync(
                request.RecordingId,
                request.MeetingId,
                userId,
                cancellationToken);

        if (recording is null)
        {
            throw new KeyNotFoundException(
                "Recording was not found.");
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

        var transcript =
            new Transcript(
                request.MeetingId,
                request.RecordingId,
                "en");

        await _transcriptRepository.AddAsync(
            transcript,
            cancellationToken);

        await _transcriptRepository.SaveChangesAsync(
            cancellationToken);

        await _backgroundQueue.QueueAsync(
            request.MeetingId,
            request.RecordingId,
            transcript.Id,
            cancellationToken);

        return transcript.Id;
    }
}