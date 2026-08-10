using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MeetingTranscriptionBot.Application.Common.Models;
using MeetingTranscriptionBot.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MeetingTranscriptionBot.Infrastructure.Authentication;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;
    private readonly TimeProvider _timeProvider;

    public JwtTokenGenerator(
        IOptions<JwtSettings> settings,
        TimeProvider timeProvider)
    {
        _settings = settings.Value;
        _timeProvider = timeProvider;
    }

    public AccessTokenResult GenerateAccessToken(
        Guid userId,
        string email,
        string firstName,
        string lastName,
        IReadOnlyCollection<string> roles)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var expiresAtUtc = now.AddMinutes(
            _settings.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                userId.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                email),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),

            new(
                ClaimTypes.NameIdentifier,
                userId.ToString()),

            new(
                ClaimTypes.Name,
                $"{firstName} {lastName}")
        };

        claims.AddRange(
            roles.Select(role =>
                new Claim(ClaimTypes.Role, role)));

        /*var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Key));*/

        byte[] keyBytes;

        try
        {
            keyBytes = Convert.FromBase64String(
                _settings.Key);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException(
                "The JWT signing key is not valid Base64.",
                exception);
        }

        var signingKey =
            new SymmetricSecurityKey(keyBytes);

        var signingCredentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials);

        var encodedToken =
            new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult(
            encodedToken,
            expiresAtUtc);
    }
}