using MeetingTranscriptionBot.Domain.Entities;
using MeetingTranscriptionBot.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeetingTranscriptionBot.Infrastructure.Persistence.Context;

public sealed class ApplicationDbContext
    : IdentityDbContext<
        ApplicationUser,
        IdentityRole<Guid>,
        Guid>
{
    public DbSet<Meeting> Meetings => Set<Meeting>();

    public DbSet<MeetingStatusHistory> MeetingStatusHistories =>
        Set<MeetingStatusHistory>();

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(
    ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<Meeting>()
            .HasQueryFilter(
                meeting => !meeting.IsDeleted);

        modelBuilder.Entity<MeetingStatusHistory>()
            .HasQueryFilter(
                history => !history.Meeting!.IsDeleted);
    }
}