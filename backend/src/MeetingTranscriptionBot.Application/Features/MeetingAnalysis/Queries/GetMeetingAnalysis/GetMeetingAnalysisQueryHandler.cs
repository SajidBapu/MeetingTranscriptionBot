using MediatR;
using MeetingTranscriptionBot.Application.Features.MeetingAnalysis.DTOs;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.MeetingAnalysis.Queries.GetMeetingAnalysis;

public sealed class GetMeetingAnalysisQueryHandler
    : IRequestHandler<GetMeetingAnalysisQuery, MeetingAnalysisDto>
{
    private readonly IMeetingAnalysisRepository _meetingAnalysisRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMeetingAnalysisQueryHandler(
        IMeetingAnalysisRepository meetingAnalysisRepository,
        ICurrentUserService currentUserService)
    {
        _meetingAnalysisRepository = meetingAnalysisRepository;
        _currentUserService = currentUserService;
    }

    public async Task<MeetingAnalysisDto> Handle(
        GetMeetingAnalysisQuery request,
        CancellationToken cancellationToken)
    {
        var ownerId = _currentUserService.UserId;

        var analysis =
            await _meetingAnalysisRepository.GetByIdAsync(
                request.AnalysisId,
                request.MeetingId,
                ownerId,
                cancellationToken);

        if (analysis is null)
        {
            throw new KeyNotFoundException(
                "Meeting analysis was not found.");
        }

        return new MeetingAnalysisDto(
            analysis.Id,
            analysis.MeetingId,
            analysis.TranscriptId,
            analysis.Summary,
            analysis.CreatedAtUtc,
            analysis.KeyPoints
                .Select(x => x.Text)
                .ToList(),
            analysis.Decisions
                .Select(x => x.Text)
                .ToList(),
            analysis.ActionItems
                .Select(x => new MeetingActionItemDto(
                    x.Id,
                    x.Description,
                    x.Assignee,
                    x.DueDateUtc))
                .ToList());
    }
}