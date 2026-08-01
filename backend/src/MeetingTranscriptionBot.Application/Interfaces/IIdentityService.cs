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
}