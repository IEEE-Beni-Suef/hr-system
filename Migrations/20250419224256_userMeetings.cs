using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IEEE.Migrations
{
    /// <inheritdoc />
    public partial class userMeetings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
     
            migrationBuilder.CreateTable(
                name: "Users_Meetings",
                columns: table => new
                {
                    MeetingsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_Meetings", x => new { x.MeetingsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_Users_Meetings_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Meetings_meetings_MeetingsId",
                        column: x => x.MeetingsId,
                        principalTable: "meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

       

            migrationBuilder.CreateIndex(
                name: "IX_Users_Meetings_UsersId",
                table: "Users_Meetings",
                column: "UsersId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meetings_Users_CreatorId",
                table: "meetings");

            migrationBuilder.DropTable(
                name: "Users_Meetings");

            migrationBuilder.DropIndex(
                name: "IX_meetings_CreatorId",
                table: "meetings");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "meetings");
        }
    }
}
