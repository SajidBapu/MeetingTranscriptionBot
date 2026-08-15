using MediatR;
using MeetingTranscriptionBot.Application.Features.Transcripts.DTOs;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Transcripts.Queries.GetTranscriptById;

public sealed class GetTranscriptByIdQueryHandler
    : IRequestHandler<GetTranscriptByIdQuery, TranscriptDto?>
{
    private readonly ITranscriptRepository _transcriptRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTranscriptByIdQueryHandler(
        ITranscriptRepository transcriptRepository,
        ICurrentUserService currentUserService)
    {
        _transcriptRepository = transcriptRepository;
        _currentUserService = currentUserService;
    }

    public async Task<TranscriptDto?> Handle(
        GetTranscriptByIdQuery request,
        CancellationToken cancellationToken)
    {
        var ownerId = _currentUserService.UserId;

        var transcript =
            await _transcriptRepository.GetByIdAsync(
                request.TranscriptId,
                request.MeetingId,
                ownerId,
                cancellationToken);

        if (transcript is null)
        {
            return null;
        }

        var segments =
            transcript.Segments
                .OrderBy(x => x.StartTime)
                .Select(x =>
                    new TranscriptSegmentDto(
                        x.Id,
                        x.Text,
                        x.SpeakerLabel,
                        x.StartTime,
                        x.EndTime))
                .ToList();

        return new TranscriptDto(
            transcript.Id,
            transcript.MeetingId,
            transcript.RecordingId,
            transcript.Language,
            transcript.Status,
            transcript.FullText,
            transcript.CreatedAtUtc,
            transcript.CompletedAtUtc,
            segments);
    }
}