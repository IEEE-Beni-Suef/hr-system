using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IEEE.Migrations
{
    /// <inheritdoc />
    public partial class addUserMeetings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users_Meetings",
                table: "Users_Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Users_Meetings_UserId",
                table: "Users_Meetings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users_Meetings",
                table: "Users_Meetings",
                columns: new[] { "UserId", "MeetingId" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Meetings_MeetingId",
                table: "Users_Meetings",
                column: "MeetingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users_Meetings",
                table: "Users_Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Users_Meetings_MeetingId",
                table: "Users_Meetings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users_Meetings",
                table: "Users_Meetings",
                columns: new[] { "MeetingId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Meetings_UserId",
                table: "Users_Meetings",
                column: "UserId");
        }
    }
}
