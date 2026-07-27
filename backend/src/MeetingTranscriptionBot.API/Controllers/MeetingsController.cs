using MediatR;
using MeetingTranscriptionBot.Application.Features.Meetings.Commands.DeleteMeeting;
using MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetingById;
using MeetingTranscriptionBot.Application.Features.Meetings.Commands.CreateMeeting;
using MeetingTranscriptionBot.Application.Features.Meetings.Commands.UpdateMeeting;
using MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetings;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;
using MeetingTranscriptionBot.Application.Features.Meetings.Commands.UpdateMeetingStatus;
using Microsoft.AspNetCore.Mvc;

namespace MeetingTranscriptionBot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeetingsController : ControllerBase
{
    private readonly IMediator _mediator;


    public MeetingsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateMeetingCommand command,
        CancellationToken cancellationToken)
    {
        var meetingId = await _mediator.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = meetingId },
            meetingId);
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] GetMeetingsQuery query,
    CancellationToken cancellationToken)
    {
        var meetings = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(meetings);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var meeting = await _mediator.Send(
            new GetMeetingByIdQuery(id),
            cancellationToken);

        if (meeting is null)
        {
            return NotFound();
        }

        return Ok(meeting);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
    Guid id,
    UpdateMeetingCommand command,
    CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(
                "Route id and request id do not match.");
        }


        var updated = await _mediator.Send(
            command,
            cancellationToken);


        if (!updated)
        {
            return NotFound(
                new
                {
                    message = "Meeting not found."
                });
        }


        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
    Guid id,
    UpdateMeetingStatusDto request,
    CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(
            new UpdateMeetingStatusCommand(
                id,
                request.Status),
            cancellationToken);


        if (!updated)
        {
            return NotFound(
                new
                {
                    message = "Meeting not found."
                });
        }


        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
    Guid id,
    CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(
            new DeleteMeetingCommand(id),
            cancellationToken);


        if (!deleted)
        {
            return NotFound(
                new
                {
                    message = "Meeting not found."
                });
        }


        return NoContent();
    }
}