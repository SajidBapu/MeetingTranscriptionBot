using MediatR;
using MeetingTranscriptionBot.Application.Common.Models;

namespace MeetingTranscriptionBot.Application.Features.Authentication
    .Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password)
    : IRequest<LoginResult>;