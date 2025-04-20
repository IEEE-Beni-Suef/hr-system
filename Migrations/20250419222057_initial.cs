using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IEEE.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "committees",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_committees", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Roles",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Roles", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        City = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CommitteeId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "meetings",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Recap = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatorId = table.Column<int>(type: "int", nullable: false),
            //        CommitteeId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_meetings", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_meetings_Users_CreatorId",
            //            column: x => x.CreatorId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_meetings_committees_CommitteeId",
            //            column: x => x.CommitteeId,
            //            principalTable: "committees",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Tasks",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Month = table.Column<DateOnly>(type: "date", nullable: false),
            //        HeadId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Tasks", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Tasks_Users_HeadId",
            //            column: x => x.HeadId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users_Meetings",
            //    columns: table => new
            //    {
            //        MeetingsId = table.Column<int>(type: "int", nullable: false),
            //        UsersId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users_Meetings", x => new { x.MeetingsId, x.UsersId });
            //        table.ForeignKey(
            //            name: "FK_Users_Meetings_Users_UsersId",
            //            column: x => x.UsersId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Users_Meetings_meetings_MeetingsId",
            //            column: x => x.MeetingsId,
            //            principalTable: "meetings",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users_Tasks",
            //    columns: table => new
            //    {
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        TaskId = table.Column<int>(type: "int", nullable: false),
            //        Score = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users_Tasks", x => new { x.UserId, x.TaskId });
            //        table.ForeignKey(
            //            name: "FK_Users_Tasks_Tasks_TaskId",
            //            column: x => x.TaskId,
            //            principalTable: "Tasks",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Users_Tasks_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_meetings_CommitteeId",
            //    table: "meetings",
            //    column: "CommitteeId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_meetings_CreatorId",
            //    table: "meetings",
            //    column: "CreatorId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Tasks_HeadId",
            //    table: "Tasks",
            //    column: "HeadId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_Meetings_UsersId",
            //    table: "Users_Meetings",
            //    column: "UsersId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_Tasks_TaskId",
            //    table: "Users_Tasks",
            //    column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Roles");

            //migrationBuilder.DropTable(
            //    name: "Users_Meetings");

            //migrationBuilder.DropTable(
            //    name: "Users_Tasks");

            //migrationBuilder.DropTable(
            //    name: "meetings");

            //migrationBuilder.DropTable(
            //    name: "Tasks");

            //migrationBuilder.DropTable(
            //    name: "committees");

            //migrationBuilder.DropTable(
            //    name: "Users");
        }
    }
}
