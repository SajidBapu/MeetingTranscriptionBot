using MediatR;
using MeetingTranscriptionBot.Application.Common.Models;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Authentication
    .Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<LoginResult> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        return _identityService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);
    }
}