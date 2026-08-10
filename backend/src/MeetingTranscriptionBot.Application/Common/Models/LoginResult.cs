namespace MeetingTranscriptionBot.Application.Common.Models;

public sealed record LoginResult(
    bool Succeeded,
    Guid? UserId,
    string? FirstName,
    string? LastName,
    string? Email,
    string? AccessToken,
    DateTime? ExpiresAtUtc,
    string? RefreshToken,
    DateTime? RefreshTokenExpiresAtUtc,
    IReadOnlyCollection<string> Errors)
{
    public static LoginResult Success(
        Guid userId,
        string firstName,
        string lastName,
        string email,
        AccessTokenResult accessToken,
        string refreshToken,
        DateTime refreshTokenExpiresAtUtc)
    {
        return new LoginResult(
            true,
            userId,
            firstName,
            lastName,
            email,
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            refreshToken,
            refreshTokenExpiresAtUtc,
            Array.Empty<string>());
    }

    public static LoginResult Failure(
        string error)
    {
        return new LoginResult(
            false,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            [error]);
    }
}