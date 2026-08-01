using MediatR;
using MeetingTranscriptionBot.Application.Common.Models;
using MeetingTranscriptionBot.Application.Features.Authentication
    .Commands.Register;
using Microsoft.AspNetCore.Mvc;

namespace MeetingTranscriptionBot.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[Consumes("application/json")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [ProducesResponseType(
        typeof(ApiResponse<Guid?>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ApiResponse<Guid?>),
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid?>>> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        if (!result.Succeeded ||
            result.UserId is null)
        {
            return BadRequest(
                ApiResponse<Guid?>.Fail(
                    string.Join(
                        " ",
                        result.Errors)));
        }

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<Guid?>.Ok(
                result.UserId.Value,
                "Registration completed successfully."));
    }
}