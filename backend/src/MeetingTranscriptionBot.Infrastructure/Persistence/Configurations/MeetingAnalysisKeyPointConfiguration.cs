using MeetingTranscriptionBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingTranscriptionBot.Infrastructure.Persistence.Configurations;

public sealed class MeetingAnalysisKeyPointConfiguration
    : IEntityTypeConfiguration<MeetingAnalysisKeyPoint>
{
    public void Configure(
        EntityTypeBuilder<MeetingAnalysisKeyPoint> builder)
    {
        builder.ToTable("MeetingAnalysisKeyPoints");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Text)
            .HasColumnType("text")
            .IsRequired();

        builder.HasOne(x => x.MeetingAnalysis)
            .WithMany(x => x.KeyPoints)
            .HasForeignKey(x => x.MeetingAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}