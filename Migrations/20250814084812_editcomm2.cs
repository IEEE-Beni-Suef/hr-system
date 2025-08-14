using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IEEE.Migrations
{
    /// <inheritdoc />
    public partial class editcomm2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Committees_AspNetUsers_HeadId",
                table: "Committees");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_AspNetUsers_HeadId",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Committees_HeadId",
                table: "Committees");

            migrationBuilder.DropColumn(
                name: "memberCount",
                table: "Committees");

            migrationBuilder.AlterColumn<int>(
                name: "HeadId",
                table: "Meetings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Committees",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_HeadId",
                table: "Committees",
                column: "HeadId",
                unique: true,
                filter: "[HeadId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Committees_Name",
                table: "Committees",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Committees_AspNetUsers_HeadId",
                table: "Committees",
                column: "HeadId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_AspNetUsers_HeadId",
                table: "Meetings",
                column: "HeadId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Committees_AspNetUsers_HeadId",
                table: "Committees");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_AspNetUsers_HeadId",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Committees_HeadId",
                table: "Committees");

            migrationBuilder.DropIndex(
                name: "IX_Committees_Name",
                table: "Committees");

            migrationBuilder.AlterColumn<int>(
                name: "HeadId",
                table: "Meetings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Committees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "memberCount",
                table: "Committees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Committees_HeadId",
                table: "Committees",
                column: "HeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Committees_AspNetUsers_HeadId",
                table: "Committees",
                column: "HeadId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_AspNetUsers_HeadId",
                table: "Meetings",
                column: "HeadId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
