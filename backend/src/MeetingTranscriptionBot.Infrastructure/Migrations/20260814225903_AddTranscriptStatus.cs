using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingTranscriptionBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTranscriptStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Transcripts",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Transcripts");
        }
    }
}
