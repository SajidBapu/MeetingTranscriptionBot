using System.Security.Cryptography;
using System.Text;
using MeetingTranscriptionBot.Application.Interfaces;

namespace MeetingTranscriptionBot.Infrastructure.Services.Security;

public sealed class RefreshTokenService
    : IRefreshTokenService
{
    public string GenerateToken()
    {
        var randomBytes =
            RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

    public string HashToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException(
                "Refresh token is required.",
                nameof(token));
        }

        var tokenBytes =
            Encoding.UTF8.GetBytes(token);

        var hashBytes =
            SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}