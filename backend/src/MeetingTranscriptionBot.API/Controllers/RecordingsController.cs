using MediatR;
using MeetingTranscriptionBot.Application.Features.Recordings.Commands.UploadMeetingRecording;
using MeetingTranscriptionBot.Application.Features.Recordings.DTOs;
using MeetingTranscriptionBot.Application.Features.Transcripts.Commands.TranscribeRecording;
using MeetingTranscriptionBot.Application.Features.Recordings.Queries.GetRecordingById;
using MeetingTranscriptionBot.Application.Features.Transcripts.DTOs;
using MeetingTranscriptionBot.Application.Features.Transcripts.Queries.GetTranscriptById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingTranscriptionBot.API.Controllers;

[ApiController]
[Route("api/meetings/{meetingId:guid}/recordings")]
[Authorize]
public sealed class RecordingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecordingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Upload(
        Guid meetingId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("A recording file is required.");
        }

        await using var stream = file.OpenReadStream();

        var command =
            new UploadMeetingRecordingCommand(
                meetingId,
                stream,
                file.FileName,
                file.ContentType,
                file.Length);

        try
        {
            var recordingId =
                await _mediator.Send(
                    command,
                    cancellationToken);

            return Created(
                $"/api/meetings/{meetingId}/recordings/{recordingId}",
                new
                {
                    id = recordingId
                });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{recordingId:guid}")]
    [ProducesResponseType(
    typeof(RecordingDto),
    StatusCodes.Status200OK)]
    [ProducesResponseType(
    StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
    StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
    Guid meetingId,
    Guid recordingId,
    CancellationToken cancellationToken)
    {
        var recording =
            await _mediator.Send(
                new GetRecordingByIdQuery(
                    meetingId,
                    recordingId),
                cancellationToken);

        if (recording is null)
        {
            return NotFound();
        }

        return Ok(recording);
    }

    [HttpPost("{recordingId:guid}/transcribe")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Transcribe(
    Guid meetingId,
    Guid recordingId,
    CancellationToken cancellationToken)
    {
        try
        {
            var transcriptId =
                await _mediator.Send(
                    new TranscribeRecordingCommand(
                        meetingId,
                        recordingId),
                    cancellationToken);

            return Created(
                $"/api/meetings/{meetingId}/transcripts/{transcriptId}",
                new
                {
                    id = transcriptId
                });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(
                new
                {
                    message = exception.Message
                });
        }
    }

    [HttpGet("~/api/meetings/{meetingId:guid}/transcripts/{transcriptId:guid}")]
    [ProducesResponseType(
    typeof(TranscriptDto),
    StatusCodes.Status200OK)]
    [ProducesResponseType(
    StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
    StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTranscript(
    Guid meetingId,
    Guid transcriptId,
    CancellationToken cancellationToken)
    {
        var transcript =
            await _mediator.Send(
                new GetTranscriptByIdQuery(
                    meetingId,
                    transcriptId),
                cancellationToken);

        if (transcript is null)
        {
            return NotFound();
        }

        return Ok(transcript);
    }
}