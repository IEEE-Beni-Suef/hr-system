using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IEEE.Migrations
{
    /// <inheritdoc />
    public partial class AddVice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "memberCount",
                table: "Committees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViceCommitteeId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ViceCommitteeId",
                table: "AspNetUsers",
                column: "ViceCommitteeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Committees_ViceCommitteeId",
                table: "AspNetUsers",
                column: "ViceCommitteeId",
                principalTable: "Committees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Committees_ViceCommitteeId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ViceCommitteeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "memberCount",
                table: "Committees");

            migrationBuilder.DropColumn(
                name: "ViceCommitteeId",
                table: "AspNetUsers");
        }
    }
}
