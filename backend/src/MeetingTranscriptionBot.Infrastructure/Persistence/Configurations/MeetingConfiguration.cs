using MeetingTranscriptionBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingTranscriptionBot.Infrastructure.Persistence.Configurations;

public class MeetingConfiguration
    : IEntityTypeConfiguration<Meeting>
{
    public void Configure(
        EntityTypeBuilder<Meeting> builder)
    {
        builder.ToTable("meetings");


        builder.HasKey(x => x.Id);


        builder.Property(x => x.Id)
            .ValueGeneratedNever();


        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();


        builder.Property(x => x.Description)
            .HasMaxLength(1000);


        builder.Property(x => x.Platform)
            .HasMaxLength(50)
            .IsRequired();


        builder.Property(x => x.Status)
            .HasConversion<int>();


        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}