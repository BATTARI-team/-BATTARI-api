using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BATTARI_api.Migrations
{
    /// <inheritdoc />
    public partial class addsouguudatetocalldb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SouguuDateTime",
                table: "Calls",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SouguuDateTime",
                table: "Calls");
        }
    }
}
