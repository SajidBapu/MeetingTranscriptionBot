using MediatR;
using MeetingTranscriptionBot.Application.Common.Models;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Application.Features.Authentication
    .Commands.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, RegistrationResult>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<RegistrationResult> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        return _identityService.RegisterAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            cancellationToken);
    }
}