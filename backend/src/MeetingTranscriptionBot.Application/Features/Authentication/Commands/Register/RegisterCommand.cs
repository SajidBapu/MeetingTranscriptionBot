using MediatR;
using MeetingTranscriptionBot.Application.Common.Models;

namespace MeetingTranscriptionBot.Application.Features.Authentication
    .Commands.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password)
    : IRequest<RegistrationResult>;