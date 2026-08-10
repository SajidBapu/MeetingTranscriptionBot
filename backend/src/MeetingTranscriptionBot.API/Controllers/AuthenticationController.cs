using MediatR;
using MeetingTranscriptionBot.Application.Common.Models;
using MeetingTranscriptionBot.Application.Features.Authentication.Commands.Login;
using MeetingTranscriptionBot.Application.Features.Authentication.Commands.Register;
using MeetingTranscriptionBot.Application.Features.Authentication.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingTranscriptionBot.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[Consumes("application/json")]
[AllowAnonymous]
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

    [HttpPost("login")]
    [ProducesResponseType(
        typeof(ApiResponse<LoginResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<LoginResponse>),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        if (!result.Succeeded ||
            result.UserId is null ||
            result.FirstName is null ||
            result.LastName is null ||
            result.Email is null ||
            result.AccessToken is null ||
            result.ExpiresAtUtc is null ||
            result.RefreshToken is null ||
            result.RefreshTokenExpiresAtUtc is null)
        {
            return Unauthorized(
                ApiResponse<LoginResponse>.Fail(
                    "Invalid email or password."));
        }

        var response = new LoginResponse(
            result.UserId.Value,
            result.FirstName,
            result.LastName,
            result.Email,
            result.AccessToken,
            result.ExpiresAtUtc.Value,
            result.RefreshToken,
            result.RefreshTokenExpiresAtUtc.Value);

        return Ok(
            ApiResponse<LoginResponse>.Ok(
                response,
                "Login completed successfully."));
    }
}