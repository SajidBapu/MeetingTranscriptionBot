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

    public DbSet<MeetingRecording> MeetingRecordings =>
    Set<MeetingRecording>();

    public DbSet<Transcript> Transcripts =>
        Set<Transcript>();

    public DbSet<TranscriptSegment> TranscriptSegments =>
        Set<TranscriptSegment>();

    public DbSet<RefreshToken> RefreshTokens =>
    Set<RefreshToken>();

    public DbSet<MeetingStatusHistory> MeetingStatusHistories =>
        Set<MeetingStatusHistory>();

    public DbSet<MeetingAnalysis> MeetingAnalyses => Set<MeetingAnalysis>();

    public DbSet<MeetingAnalysisKeyPoint> MeetingAnalysisKeyPoints =>
        Set<MeetingAnalysisKeyPoint>();

    public DbSet<MeetingAnalysisDecision> MeetingAnalysisDecisions =>
        Set<MeetingAnalysisDecision>();

    public DbSet<MeetingActionItem> MeetingActionItems =>
        Set<MeetingActionItem>();

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

        modelBuilder.Entity<MeetingRecording>()
           .HasQueryFilter(
        recording => !recording.Meeting!.IsDeleted);

        modelBuilder.Entity<Transcript>()
            .HasQueryFilter(
                transcript => !transcript.Meeting!.IsDeleted);

        modelBuilder.Entity<TranscriptSegment>()
            .HasQueryFilter(
                segment => !segment.Transcript!.Meeting!.IsDeleted);

        modelBuilder.Entity<MeetingStatusHistory>()
            .HasQueryFilter(
                history => !history.Meeting!.IsDeleted);
    }
}