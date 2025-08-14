using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IEEE.Migrations
{
    /// <inheritdoc />
    public partial class removeViceidfromusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Committees_ViceCommitteeId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ViceCommitteeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ViceCommitteeId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CommitteeId",
                table: "AspNetUsers",
                column: "CommitteeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Committees_CommitteeId",
                table: "AspNetUsers",
                column: "CommitteeId",
                principalTable: "Committees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Committees_CommitteeId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CommitteeId",
                table: "AspNetUsers");

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
    }
}
