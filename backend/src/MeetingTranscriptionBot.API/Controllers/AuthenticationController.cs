using MediatR;
using MeetingTranscriptionBot.Application.Common.Models;
using MeetingTranscriptionBot.Application.Features.Authentication.Commands.Login;
using MeetingTranscriptionBot.Application.Features.Authentication.Commands.Register;
using MeetingTranscriptionBot.Application.Features.Authentication.DTOs;
using MeetingTranscriptionBot.Application.Interfaces;
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
    private readonly IIdentityService _identityService;

    public AuthenticationController(
    IMediator mediator,
    IIdentityService identityService)
    {
        _mediator = mediator;
        _identityService = identityService;
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

    [HttpPost("refresh")]
    [ProducesResponseType(
    typeof(ApiResponse<RefreshTokenResponse>),
    StatusCodes.Status200OK)]
    [ProducesResponseType(
    typeof(ApiResponse<RefreshTokenResponse>),
    StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
    StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<RefreshTokenResponse>>> Refresh(
    [FromBody] RefreshTokenRequest request,
    CancellationToken cancellationToken)
    {
        var result =
            await _identityService.RefreshTokenAsync(
                request.RefreshToken,
                cancellationToken);

        if (!result.Succeeded ||
            result.AccessToken is null ||
            result.ExpiresAtUtc is null ||
            result.RefreshToken is null ||
            result.RefreshTokenExpiresAtUtc is null)
        {
            return Unauthorized(
                ApiResponse<RefreshTokenResponse>.Fail(
                    "Invalid or expired refresh token."));
        }

        var response =
            new RefreshTokenResponse(
                result.AccessToken,
                result.ExpiresAtUtc.Value,
                result.RefreshToken,
                result.RefreshTokenExpiresAtUtc.Value);

        return Ok(
            ApiResponse<RefreshTokenResponse>.Ok(
                response,
                "Token refreshed successfully."));
    }

    [HttpPost("logout")]
    [ProducesResponseType(
    typeof(ApiResponse<object>),
    StatusCodes.Status200OK)]
    [ProducesResponseType(
    typeof(ApiResponse<object>),
    StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
    [FromBody] LogoutRequest request,
    CancellationToken cancellationToken)
    {
        var succeeded =
            await _identityService.LogoutAsync(
                request.RefreshToken,
                cancellationToken);

        if (!succeeded)
        {
            return BadRequest(
                ApiResponse<object>.Fail(
                    "Logout could not be completed."));
        }

        return Ok(
            ApiResponse<object>.Ok(
                null,
                "Logout completed successfully."));
    }
}