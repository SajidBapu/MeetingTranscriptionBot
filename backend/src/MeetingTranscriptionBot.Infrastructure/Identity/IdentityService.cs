using MeetingTranscriptionBot.Application.Common.Models;
using MeetingTranscriptionBot.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MeetingTranscriptionBot.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
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
            // Generic message reduces account-enumeration leakage.
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
                result.Errors
                    .Select(error => error.Description));
        }

        return RegistrationResult.Success(user.Id);
    }
}