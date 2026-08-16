using MeetingTranscriptionBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingTranscriptionBot.Infrastructure.Persistence.Configurations;

public sealed class MeetingAnalysisConfiguration
    : IEntityTypeConfiguration<MeetingAnalysis>
{
    public void Configure(
        EntityTypeBuilder<MeetingAnalysis> builder)
    {
        builder.ToTable("MeetingAnalyses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Summary)
            .HasColumnType("text")
            .IsRequired();

        builder.HasIndex(x => x.TranscriptId)
            .IsUnique();

        builder.HasOne(x => x.Meeting)
            .WithMany()
            .HasForeignKey(x => x.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Transcript)
            .WithMany()
            .HasForeignKey(x => x.TranscriptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}