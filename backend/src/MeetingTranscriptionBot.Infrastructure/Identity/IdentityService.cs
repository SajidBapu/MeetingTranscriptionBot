using MeetingTranscriptionBot.Application.Common.Models;
using MeetingTranscriptionBot.Application.Interfaces;
using MeetingTranscriptionBot.Domain.Entities;
using MeetingTranscriptionBot.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MeetingTranscriptionBot.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private const string InvalidCredentialsMessage =
        "Invalid email or password.";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenService refreshTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtSettings> jwtOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<RegistrationResult> RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedEmail = email
            .Trim()
            .ToLowerInvariant();

        var existingUser =
            await _userManager.FindByEmailAsync(
                normalizedEmail);

        if (existingUser is not null)
        {
            return RegistrationResult.Failure(
                ["Registration could not be completed."]);
        }

        var user = new ApplicationUser(
            firstName.Trim(),
            lastName.Trim(),
            normalizedEmail);

        var result = await _userManager.CreateAsync(
            user,
            password);

        if (!result.Succeeded)
        {
            return RegistrationResult.Failure(
                result.Errors.Select(
                    error => error.Description));
        }

        return RegistrationResult.Success(user.Id);
    }

    public async Task<LoginResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedEmail = email
            .Trim()
            .ToLowerInvariant();

        var user = await _userManager.FindByEmailAsync(
            normalizedEmail);

        if (user is null)
        {
            return LoginResult.Failure(
                InvalidCredentialsMessage);
        }

        var signInResult =
            await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            return LoginResult.Failure(
                InvalidCredentialsMessage);
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        var accessToken =
            _jwtTokenGenerator.GenerateAccessToken(
                user.Id,
                user.Email ?? normalizedEmail,
                user.FirstName,
                user.LastName,
                roles.ToArray());

        // Generate the raw refresh token.
        var refreshToken =
            _refreshTokenService.GenerateToken();

        // Store only the SHA-256 hash in the database.
        var refreshTokenHash =
            _refreshTokenService.HashToken(
                refreshToken);

        var refreshTokenExpiresAtUtc =
            DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenExpirationDays);

        var refreshTokenEntity =
            new RefreshToken(
                user.Id,
                refreshTokenHash,
                refreshTokenExpiresAtUtc);

        await _refreshTokenRepository.AddAsync(
            refreshTokenEntity,
            cancellationToken);

        await _refreshTokenRepository.SaveChangesAsync(
            cancellationToken);

        return LoginResult.Success(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email ?? normalizedEmail,
            accessToken,
            refreshToken,
            refreshTokenExpiresAtUtc);
    }

    public async Task<LoginResult> RefreshTokenAsync(
    string refreshToken,
    CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return LoginResult.Failure(
                "Invalid refresh token.");
        }

        var refreshTokenHash =
            _refreshTokenService.HashToken(
                refreshToken);

        var storedRefreshToken =
            await _refreshTokenRepository.GetByTokenHashAsync(
                refreshTokenHash,
                cancellationToken);

        if (storedRefreshToken is null ||
            !storedRefreshToken.IsActive)
        {
            return LoginResult.Failure(
                "Invalid or expired refresh token.");
        }

        var user =
            await _userManager.FindByIdAsync(
                storedRefreshToken.UserId.ToString());

        if (user is null)
        {
            return LoginResult.Failure(
                "Invalid refresh token.");
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        var accessToken =
            _jwtTokenGenerator.GenerateAccessToken(
                user.Id,
                user.Email ?? string.Empty,
                user.FirstName,
                user.LastName,
                roles.ToArray());

        var newRefreshToken =
            _refreshTokenService.GenerateToken();

        var newRefreshTokenHash =
            _refreshTokenService.HashToken(
                newRefreshToken);

        var newRefreshTokenExpiresAtUtc =
            DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenExpirationDays);

        storedRefreshToken.Revoke(
            newRefreshTokenHash);

        var newRefreshTokenEntity =
            new RefreshToken(
                user.Id,
                newRefreshTokenHash,
                newRefreshTokenExpiresAtUtc);

        await _refreshTokenRepository.AddAsync(
            newRefreshTokenEntity,
            cancellationToken);

        await _refreshTokenRepository.SaveChangesAsync(
            cancellationToken);

        return LoginResult.Success(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email ?? string.Empty,
            accessToken,
            newRefreshToken,
            newRefreshTokenExpiresAtUtc);
    }

    public async Task<bool> LogoutAsync(
    string refreshToken,
    CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();

    if (string.IsNullOrWhiteSpace(refreshToken))
    {
        return false;
    }

    var refreshTokenHash =
        _refreshTokenService.HashToken(
            refreshToken);

    var storedRefreshToken =
        await _refreshTokenRepository.GetByTokenHashAsync(
            refreshTokenHash,
            cancellationToken);

    if (storedRefreshToken is null)
    {
        return false;
    }

    if (!storedRefreshToken.IsActive)
    {
        return false;
    }

    storedRefreshToken.Revoke();

    await _refreshTokenRepository.SaveChangesAsync(
        cancellationToken);

    return true;
}
}