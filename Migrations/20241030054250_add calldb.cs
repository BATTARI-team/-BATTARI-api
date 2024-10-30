using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BATTARI_api.Migrations
{
    /// <inheritdoc />
    public partial class addcalldb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calls",
                columns: table => new
                {
                    CallId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CallStartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SouguuReason = table.Column<string>(type: "TEXT", nullable: false),
                    CallTime = table.Column<int>(type: "INTEGER", nullable: false),
                    User1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    User2Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calls", x => x.CallId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calls");
        }
    }
}
