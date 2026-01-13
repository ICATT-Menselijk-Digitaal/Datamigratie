using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFKToResultaattypeMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResultaattypeMapping_DetZaaktypeId_DetResultaattypeId_Unique",
                table: "ResultaattypeMappings");

            migrationBuilder.DropColumn(
                name: "DetZaaktypeId",
                table: "ResultaattypeMappings");

            migrationBuilder.RenameColumn(
                name: "OzZaaktypeId",
                table: "ResultaattypeMappings",
                newName: "ZaaktypenMappingId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeId_Unique",
                table: "ResultaattypeMappings",
                columns: new[] { "ZaaktypenMappingId", "DetResultaattypeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultaattypeMappings_Mappings_ZaaktypenMappingId",
                table: "ResultaattypeMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultaattypeMappings_Mappings_ZaaktypenMappingId",
                table: "ResultaattypeMappings");

            migrationBuilder.DropIndex(
                name: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeId_Unique",
                table: "ResultaattypeMappings");

            migrationBuilder.RenameColumn(
                name: "ZaaktypenMappingId",
                table: "ResultaattypeMappings",
                newName: "OzZaaktypeId");

            migrationBuilder.AddColumn<string>(
                name: "DetZaaktypeId",
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
    }
}
