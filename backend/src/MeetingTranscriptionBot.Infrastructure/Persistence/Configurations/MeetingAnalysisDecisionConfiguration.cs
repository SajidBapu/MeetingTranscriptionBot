using MeetingTranscriptionBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingTranscriptionBot.Infrastructure.Persistence.Configurations;

public sealed class MeetingAnalysisDecisionConfiguration
    : IEntityTypeConfiguration<MeetingAnalysisDecision>
{
    public void Configure(
        EntityTypeBuilder<MeetingAnalysisDecision> builder)
    {
        builder.ToTable("MeetingAnalysisDecisions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Text)
            .HasColumnType("text")
            .IsRequired();

        builder.HasOne(x => x.MeetingAnalysis)
            .WithMany(x => x.Decisions)
            .HasForeignKey(x => x.MeetingAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}