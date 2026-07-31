using MeetingTranscriptionBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingTranscriptionBot.Infrastructure.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public DbSet<Meeting> Meetings { get; set; }

    public DbSet<MeetingStatusHistory> MeetingStatusHistories { get; set; }


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
                x => !x.IsDeleted);
    }
}