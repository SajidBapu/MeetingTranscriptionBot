using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingTranscriptionBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetingAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeetingAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MeetingId = table.Column<Guid>(type: "uuid", nullable: false),
                    TranscriptId = table.Column<Guid>(type: "uuid", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingAnalyses_Transcripts_TranscriptId",
                        column: x => x.TranscriptId,
                        principalTable: "Transcripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingAnalyses_meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeetingActionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MeetingAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Assignee = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DueDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingActionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingActionItems_MeetingAnalyses_MeetingAnalysisId",
                        column: x => x.MeetingAnalysisId,
                        principalTable: "MeetingAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeetingAnalysisDecisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MeetingAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingAnalysisDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingAnalysisDecisions_MeetingAnalyses_MeetingAnalysisId",
                        column: x => x.MeetingAnalysisId,
                        principalTable: "MeetingAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeetingAnalysisKeyPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MeetingAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingAnalysisKeyPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingAnalysisKeyPoints_MeetingAnalyses_MeetingAnalysisId",
                        column: x => x.MeetingAnalysisId,
                        principalTable: "MeetingAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingActionItems_MeetingAnalysisId",
                table: "MeetingActionItems",
                column: "MeetingAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAnalyses_MeetingId",
                table: "MeetingAnalyses",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAnalyses_TranscriptId",
                table: "MeetingAnalyses",
                column: "TranscriptId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAnalysisDecisions_MeetingAnalysisId",
                table: "MeetingAnalysisDecisions",
                column: "MeetingAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAnalysisKeyPoints_MeetingAnalysisId",
                table: "MeetingAnalysisKeyPoints",
                column: "MeetingAnalysisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeetingActionItems");

            migrationBuilder.DropTable(
                name: "MeetingAnalysisDecisions");

            migrationBuilder.DropTable(
                name: "MeetingAnalysisKeyPoints");

            migrationBuilder.DropTable(
                name: "MeetingAnalyses");
        }
    }
}
