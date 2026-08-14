using MeetingTranscriptionBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingTranscriptionBot.Infrastructure.Persistence.Configurations;

public sealed class MeetingRecordingConfiguration
    : IEntityTypeConfiguration<MeetingRecording>
{
    public void Configure(
        EntityTypeBuilder<MeetingRecording> builder)
    {
        builder.ToTable("MeetingRecordings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.StoragePath)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.FileSizeBytes)
            .IsRequired();

        builder.Property(x => x.Duration);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.MeetingId);

        builder.HasOne(x => x.Meeting)
            .WithMany()
            .HasForeignKey(x => x.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}