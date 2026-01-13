using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStatusMappingWithForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetZaaktypeId",
                table: "StatusMappings");

            migrationBuilder.AddColumn<Guid>(
                name: "ZaaktypenMappingId",
                table: "StatusMappings",
                type: "uuid",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_StatusMapping_ZaaktypenMappingId_DetStatusNaam_Unique",
                table: "StatusMappings",
                columns: new[] { "ZaaktypenMappingId", "DetStatusNaam" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StatusMappings_Mappings_ZaaktypenMappingId",
                table: "StatusMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatusMappings_Mappings_ZaaktypenMappingId",
                table: "StatusMappings");

            migrationBuilder.DropIndex(
                name: "IX_StatusMapping_ZaaktypenMappingId_DetStatusNaam_Unique",
                table: "StatusMappings");

            migrationBuilder.DropColumn(
                name: "ZaaktypenMappingId",
                table: "StatusMappings");

            migrationBuilder.AddColumn<string>(
                name: "DetZaaktypeId",
                table: "StatusMappings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
