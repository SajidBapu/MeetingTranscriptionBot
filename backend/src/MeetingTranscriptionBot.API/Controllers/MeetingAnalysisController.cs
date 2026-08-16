using MediatR;
using MeetingTranscriptionBot.Application.Features.MeetingAnalysis.Commands.AnalyzeTranscript;
using MeetingTranscriptionBot.Application.Features.MeetingAnalysis.DTOs;
using MeetingTranscriptionBot.Application.Features.MeetingAnalysis.Queries.GetMeetingAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingTranscriptionBot.API.Controllers;

[ApiController]
[Authorize]
[Route("api/meetings/{meetingId:guid}")]
public sealed class MeetingAnalysisController : ControllerBase
{
    private readonly ISender _sender;

    public MeetingAnalysisController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("transcripts/{transcriptId:guid}/analysis")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AnalyzeTranscript(
        Guid meetingId,
        Guid transcriptId,
        CancellationToken cancellationToken)
    {
        try
        {
            var analysisId =
                await _sender.Send(
                    new AnalyzeTranscriptCommand(
                        meetingId,
                        transcriptId),
                    cancellationToken);

            return CreatedAtAction(
                nameof(GetAnalysis),
                new
                {
                    meetingId,
                    analysisId
                },
                new
                {
                    id = analysisId,
                    status = "Completed"
                });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    [HttpGet("analysis/{analysisId:guid}")]
    [ProducesResponseType(
        typeof(MeetingAnalysisDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAnalysis(
        Guid meetingId,
        Guid analysisId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result =
                await _sender.Send(
                    new GetMeetingAnalysisQuery(
                        meetingId,
                        analysisId),
                    cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }
}