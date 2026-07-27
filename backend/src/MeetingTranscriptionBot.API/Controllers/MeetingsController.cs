using MediatR;
using MeetingTranscriptionBot.Application.Features.Meetings.Commands.DeleteMeeting;
using MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetingById;
using MeetingTranscriptionBot.Application.Features.Meetings.Commands.CreateMeeting;
using MeetingTranscriptionBot.Application.Features.Meetings.Commands.UpdateMeeting;
using MeetingTranscriptionBot.Application.Features.Meetings.Queries.GetMeetings;
using MeetingTranscriptionBot.Application.Features.Meetings.DTOs;
using MeetingTranscriptionBot.Application.Features.Meetings.Commands.UpdateMeetingStatus;
using MeetingTranscriptionBot.Application.Features.Meetings.Queries.SearchMeetings;
using MeetingTranscriptionBot.Application.Features.Meetings.Commands.RestoreMeeting;
using MeetingTranscriptionBot.Application.Common.Models;
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
        [FromBody] CreateMeetingCommand command,
        CancellationToken cancellationToken)
    {
        var meetingId = await _mediator.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
             nameof(GetById),
             new { id = meetingId },
             ApiResponse<Guid>.Ok(
             meetingId,
             "Meeting created successfully."));
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] GetMeetingsQuery query,
    CancellationToken cancellationToken)
    {
        var meetings = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(
            ApiResponse<PagedResult<MeetingDto>>.Ok(
              meetings));
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
            return NotFound(
               ApiResponse<MeetingDto>.Fail(
                   "Meeting not found."));
        }

        return Ok(
            ApiResponse<MeetingDto>.Ok(
                 meeting));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdateMeetingCommand command,
    CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(
                ApiResponse<bool>.Fail(
                  "Route id and request id do not match."));
        }


        var updated = await _mediator.Send(
            command,
            cancellationToken);


        if (!updated)
        {
            return NotFound(
               ApiResponse<bool>.Fail(
                   "Meeting not found."));
        }


        return Ok(
             ApiResponse<bool>.Ok(
                 true,
                  "Meeting updated successfully."));
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
                 ApiResponse<bool>.Fail(
                     "Meeting not found."));
        }


        return Ok(
             ApiResponse<bool>.Ok(
                 true,
                 "Meeting status updated successfully."));
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
                ApiResponse<bool>.Fail(
                    "Meeting not found."));
        }


        return Ok(
            ApiResponse<bool>.Ok(
               true,
               "Meeting deleted successfully."));
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
    [FromQuery] SearchMeetingsQuery query,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(
            ApiResponse<PagedResult<MeetingDto>>.Ok(
                result));
    }

    [HttpPut("{id:guid}/restore")]
    public async Task<IActionResult> Restore(
    Guid id,
    CancellationToken cancellationToken)
    {
        var restored = await _mediator.Send(
            new RestoreMeetingCommand(id),
            cancellationToken);


        if (!restored)
        {
            return NotFound(
               ApiResponse<bool>.Fail(
                   "Deleted meeting not found."));
        }


        return Ok(
            ApiResponse<bool>.Ok(
                true,
                "Meeting restored successfully."));
    }
}