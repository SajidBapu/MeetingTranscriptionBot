using MeetingTranscriptionBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingTranscriptionBot.Infrastructure.Persistence.Configurations;

public sealed class MeetingActionItemConfiguration
    : IEntityTypeConfiguration<MeetingActionItem>
{
    public void Configure(
        EntityTypeBuilder<MeetingActionItem> builder)
    {
        builder.ToTable("MeetingActionItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Description)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.Assignee)
            .HasMaxLength(200);

        builder.HasOne(x => x.MeetingAnalysis)
            .WithMany(x => x.ActionItems)
            .HasForeignKey(x => x.MeetingAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}