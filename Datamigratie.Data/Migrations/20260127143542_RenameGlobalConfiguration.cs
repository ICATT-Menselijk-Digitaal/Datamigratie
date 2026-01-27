using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameGlobalConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RsinConfigurations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DocumentstatusMappings");

            migrationBuilder.RenameIndex(
                name: "IX_ResultaattypeMappings_ZaaktypenMappingId_DetResultaattypeNa~",
                table: "ResultaattypeMappings",
                newName: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeNaam_Unique");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeNaam_Unique",
                table: "ResultaattypeMappings",
                newName: "IX_ResultaattypeMappings_ZaaktypenMappingId_DetResultaattypeNa~");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RsinConfigurations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DocumentstatusMappings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
