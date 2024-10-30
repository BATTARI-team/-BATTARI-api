using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BATTARI_api.Migrations
{
    /// <inheritdoc />
    public partial class addsouguuReasonStatustocalldb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SouguuReasonStatus",
                table: "Calls",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SouguuReasonStatus",
                table: "Calls");
        }
    }
}
