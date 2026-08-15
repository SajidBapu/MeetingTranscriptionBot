using MeetingTranscriptionBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingTranscriptionBot.Infrastructure.Persistence.Configurations;

public sealed class TranscriptConfiguration
    : IEntityTypeConfiguration<Transcript>
{
    public void Configure(
        EntityTypeBuilder<Transcript> builder)
    {
        builder.ToTable("Transcripts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Language)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.FullText)
            .HasColumnType("text");

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.CompletedAtUtc);

        builder.HasIndex(x => x.MeetingId);

        builder.HasIndex(x => x.RecordingId)
            .IsUnique();

        builder.HasOne(x => x.Meeting)
            .WithMany()
            .HasForeignKey(x => x.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Recording)
            .WithOne()
            .HasForeignKey<Transcript>(
                x => x.RecordingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Status)
             .HasMaxLength(30)
             .IsRequired();
    }
}