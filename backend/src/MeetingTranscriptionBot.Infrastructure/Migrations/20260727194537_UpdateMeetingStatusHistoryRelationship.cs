using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingTranscriptionBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMeetingStatusHistoryRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meeting_status_histories_meetings_MeetingId",
                table: "meeting_status_histories");

            migrationBuilder.AddForeignKey(
                name: "FK_meeting_status_histories_meetings_MeetingId",
                table: "meeting_status_histories",
                column: "MeetingId",
                principalTable: "meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meeting_status_histories_meetings_MeetingId",
                table: "meeting_status_histories");

            migrationBuilder.AddForeignKey(
                name: "FK_meeting_status_histories_meetings_MeetingId",
                table: "meeting_status_histories",
                column: "MeetingId",
                principalTable: "meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
