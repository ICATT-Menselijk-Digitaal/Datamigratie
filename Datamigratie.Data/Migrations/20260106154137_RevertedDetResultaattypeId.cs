using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class RevertedDetResultaattypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResultaattypeMapping_DetZaaktypeId_Unique",
                table: "ResultaattypeMappings");

            migrationBuilder.AddColumn<string>(
                name: "DetResultaattypeId",
                table: "ResultaattypeMappings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ResultaattypeMapping_DetZaaktypeId_DetResultaattypeId_Unique",
                table: "ResultaattypeMappings",
                columns: new[] { "DetZaaktypeId", "DetResultaattypeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResultaattypeMapping_DetZaaktypeId_DetResultaattypeId_Unique",
                table: "ResultaattypeMappings");

            migrationBuilder.DropColumn(
                name: "DetResultaattypeId",
                table: "ResultaattypeMappings");

            migrationBuilder.CreateIndex(
                name: "IX_ResultaattypeMapping_DetZaaktypeId_Unique",
                table: "ResultaattypeMappings",
                column: "DetZaaktypeId",
                unique: true);
        }
    }
}
