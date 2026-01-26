using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentPropertyMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeNaam_Unique",
                table: "ResultaattypeMappings",
                newName: "IX_ResultaattypeMappings_ZaaktypenMappingId_DetResultaattypeNa~");

            migrationBuilder.CreateTable(
                name: "DocumentPropertyMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    DetValue = table.Column<string>(type: "text", nullable: false),
                    OzValue = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPropertyMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentPropertyMappings_Mappings_ZaaktypenMappingId",
                        column: x => x.ZaaktypenMappingId,
                        principalTable: "Mappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPropertyMapping_ZaaktypenMappingId_PropertyName_DetValue_Unique",
                table: "DocumentPropertyMappings",
                columns: new[] { "ZaaktypenMappingId", "PropertyName", "DetValue" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentPropertyMappings");

            migrationBuilder.RenameIndex(
                name: "IX_ResultaattypeMappings_ZaaktypenMappingId_DetResultaattypeNa~",
                table: "ResultaattypeMappings",
                newName: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeNaam_Unique");
        }
    }
}
