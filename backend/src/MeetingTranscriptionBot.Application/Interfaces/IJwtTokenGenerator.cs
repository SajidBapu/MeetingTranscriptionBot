using MeetingTranscriptionBot.Application.Common.Models;

namespace MeetingTranscriptionBot.Application.Interfaces;

public interface IJwtTokenGenerator
{
    AccessTokenResult GenerateAccessToken(
        Guid userId,
        string email,
        string firstName,
        string lastName,
        IReadOnlyCollection<string> roles);
}