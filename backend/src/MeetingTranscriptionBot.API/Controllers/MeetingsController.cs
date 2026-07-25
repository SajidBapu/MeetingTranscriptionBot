using MediatR;
using MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetingById;
using MeetingTranscriptionBot.Application.Features.Meetings.Commands.CreateMeeting;
using MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetings;
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
}