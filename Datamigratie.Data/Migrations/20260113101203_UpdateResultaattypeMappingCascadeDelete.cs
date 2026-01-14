using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateResultaattypeMappingCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultaattypeMappings_Mappings_ZaaktypenMappingId",
                table: "ResultaattypeMappings");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultaattypeMappings_Mappings_ZaaktypenMappingId",
                table: "ResultaattypeMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultaattypeMappings_Mappings_ZaaktypenMappingId",
                table: "ResultaattypeMappings");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultaattypeMappings_Mappings_ZaaktypenMappingId",
                table: "ResultaattypeMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
