using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingTranscriptionBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecordingAndTranscriptEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeetingRecordings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingRecordings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingRecordings_meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transcripts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FullText = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transcripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transcripts_MeetingRecordings_RecordingId",
                        column: x => x.RecordingId,
                        principalTable: "MeetingRecordings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transcripts_meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranscriptSegments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TranscriptId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    SpeakerLabel = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranscriptSegments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranscriptSegments_Transcripts_TranscriptId",
                        column: x => x.TranscriptId,
                        principalTable: "Transcripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingRecordings_MeetingId",
                table: "MeetingRecordings",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_Transcripts_MeetingId",
                table: "Transcripts",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_Transcripts_RecordingId",
                table: "Transcripts",
                column: "RecordingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TranscriptSegments_TranscriptId",
                table: "TranscriptSegments",
                column: "TranscriptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TranscriptSegments");

            migrationBuilder.DropTable(
                name: "Transcripts");

            migrationBuilder.DropTable(
                name: "MeetingRecordings");
        }
    }
}
