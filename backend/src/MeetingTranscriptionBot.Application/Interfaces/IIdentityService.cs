using MeetingTranscriptionBot.Application.Common.Models;

namespace MeetingTranscriptionBot.Application.Interfaces;

public interface IIdentityService
{
    Task<RegistrationResult> RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken);

    Task<LoginResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken);
    Task<LoginResult> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken);
    Task<bool> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken);
}