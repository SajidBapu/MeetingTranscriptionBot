using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingTranscriptionBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetingOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "meetings",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE meetings
                SET "OwnerId" = 'f30ca8d8-7b4e-48ac-a946-e37c3943bc12'
                WHERE "OwnerId" IS NULL;
                """);

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM meetings
                        WHERE "OwnerId" IS NULL
                    ) THEN
                        RAISE EXCEPTION
                            'Cannot assign meeting ownership because no valid user was provided.';
                    END IF;
                END $$;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "meetings",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_meetings_OwnerId",
                table: "meetings",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_meetings_AspNetUsers_OwnerId",
                table: "meetings",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meetings_AspNetUsers_OwnerId",
                table: "meetings");

            migrationBuilder.DropIndex(
                name: "IX_meetings_OwnerId",
                table: "meetings");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "meetings");
        }
    }
}