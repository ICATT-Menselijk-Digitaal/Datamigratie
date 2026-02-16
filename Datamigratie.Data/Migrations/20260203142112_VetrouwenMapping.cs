using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class VetrouwenMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_ResultaattypeMappings_ZaaktypenMappingId_DetResultaattypeNa~",
                table: "ResultaattypeMappings",
                newName: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeNaam_Unique");

            migrationBuilder.CreateTable(
                name: "VertrouwelijkheidMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetVertrouwelijkheid = table.Column<bool>(type: "boolean", nullable: false),
                    OzVertrouwelijkheidaanduiding = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VertrouwelijkheidMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VertrouwelijkheidMappings_Mappings_ZaaktypenMappingId",
                        column: x => x.ZaaktypenMappingId,
                        principalTable: "Mappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VertrouwelijkheidMapping_ZaaktypenMappingId_DetVertrouwelijkheid_Unique",
                table: "VertrouwelijkheidMappings",
                columns: new[] { "ZaaktypenMappingId", "DetVertrouwelijkheid" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VertrouwelijkheidMappings");

            migrationBuilder.RenameIndex(
                name: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeNaam_Unique",
                table: "ResultaattypeMappings",
                newName: "IX_ResultaattypeMappings_ZaaktypenMappingId_DetResultaattypeNa~");
        }
    }
}
