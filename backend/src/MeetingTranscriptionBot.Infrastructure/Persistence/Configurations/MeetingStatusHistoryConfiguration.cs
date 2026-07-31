using MeetingTranscriptionBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingTranscriptionBot.Infrastructure.Persistence.Configurations;

public class MeetingStatusHistoryConfiguration
    : IEntityTypeConfiguration<MeetingStatusHistory>
{
    public void Configure(
        EntityTypeBuilder<MeetingStatusHistory> builder)
    {
        builder.ToTable("meeting_status_histories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasOne(x => x.Meeting)
            .WithMany(x => x.StatusHistory)
            .HasForeignKey(x => x.MeetingId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.PreviousStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.NewStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ChangedAt)
            .IsRequired();
    }
}