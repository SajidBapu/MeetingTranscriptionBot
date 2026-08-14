using MeetingTranscriptionBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingTranscriptionBot.Infrastructure.Persistence.Configurations;

public sealed class TranscriptSegmentConfiguration
    : IEntityTypeConfiguration<TranscriptSegment>
{
    public void Configure(
        EntityTypeBuilder<TranscriptSegment> builder)
    {
        builder.ToTable("TranscriptSegments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.SpeakerLabel)
            .HasMaxLength(200);

        builder.Property(x => x.StartTime)
            .IsRequired();

        builder.Property(x => x.EndTime)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.TranscriptId);

        builder.HasOne(x => x.Transcript)
            .WithMany(x => x.Segments)
            .HasForeignKey(x => x.TranscriptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}