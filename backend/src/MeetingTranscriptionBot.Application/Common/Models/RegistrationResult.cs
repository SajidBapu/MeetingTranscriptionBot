namespace MeetingTranscriptionBot.Application.Common.Models;

public sealed record RegistrationResult(
    bool Succeeded,
    Guid? UserId,
    IReadOnlyCollection<string> Errors)
{
    public static RegistrationResult Success(Guid userId)
    {
        return new RegistrationResult(
            true,
            userId,
            Array.Empty<string>());
    }

    public static RegistrationResult Failure(
        IEnumerable<string> errors)
    {
        return new RegistrationResult(
            false,
            null,
            errors.ToArray());
    }
}