using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStatusMappingForeignKeyToCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatusMappings_Mappings_ZaaktypenMappingId",
                table: "StatusMappings");

            migrationBuilder.AddForeignKey(
                name: "FK_StatusMappings_Mappings_ZaaktypenMappingId",
                table: "StatusMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatusMappings_Mappings_ZaaktypenMappingId",
                table: "StatusMappings");

            migrationBuilder.AddForeignKey(
                name: "FK_StatusMappings_Mappings_ZaaktypenMappingId",
                table: "StatusMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
