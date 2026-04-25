using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IEEE.Migrations
{
    /// <inheritdoc />
    public partial class AddSponsors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subsection_Articles_ArticleId",
                table: "Subsection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subsection",
                table: "Subsection");

            migrationBuilder.RenameTable(
                name: "Subsection",
                newName: "Subsections");

            migrationBuilder.RenameIndex(
                name: "IX_Subsection_ArticleId",
                table: "Subsections",
                newName: "IX_Subsections_ArticleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subsections",
                table: "Subsections",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Sponsors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponsors", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Subsections_Articles_ArticleId",
                table: "Subsections",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subsections_Articles_ArticleId",
                table: "Subsections");

            migrationBuilder.DropTable(
                name: "Sponsors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subsections",
                table: "Subsections");

            migrationBuilder.RenameTable(
                name: "Subsections",
                newName: "Subsection");

            migrationBuilder.RenameIndex(
                name: "IX_Subsections_ArticleId",
                table: "Subsection",
                newName: "IX_Subsection_ArticleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subsection",
                table: "Subsection",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subsection_Articles_ArticleId",
                table: "Subsection",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
