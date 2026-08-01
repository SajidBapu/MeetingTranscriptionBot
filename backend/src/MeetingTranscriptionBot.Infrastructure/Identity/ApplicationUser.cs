using Microsoft.AspNetCore.Identity;

namespace MeetingTranscriptionBot.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    private ApplicationUser()
    {
    }

    public ApplicationUser(
        string firstName,
        string lastName,
        string email)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        UserName = email;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(
        string firstName,
        string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}