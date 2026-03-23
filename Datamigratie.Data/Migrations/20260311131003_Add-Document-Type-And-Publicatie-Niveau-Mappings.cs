using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentTypeAndPublicatieNiveauMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentPropertyMappings");

            migrationBuilder.CreateTable(
                name: "DocumenttypeMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetDocumenttypeNaam = table.Column<string>(type: "text", nullable: false),
                    OzInformatieobjecttypeUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumenttypeMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumenttypeMappings_Mappings_ZaaktypenMappingId",
                        column: x => x.ZaaktypenMappingId,
                        principalTable: "Mappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublicatieNiveauMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetPublicatieNiveau = table.Column<string>(type: "text", nullable: false),
                    OzVertrouwelijkheidaanduiding = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicatieNiveauMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicatieNiveauMappings_Mappings_ZaaktypenMappingId",
                        column: x => x.ZaaktypenMappingId,
                        principalTable: "Mappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumenttypeMapping_ZaaktypenMappingId_DetDocumenttypeNaam_Unique",
                table: "DocumenttypeMappings",
                columns: new[] { "ZaaktypenMappingId", "DetDocumenttypeNaam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicatieNiveauMapping_ZaaktypenMappingId_DetPublicatieNiveau_Unique",
                table: "PublicatieNiveauMappings",
                columns: new[] { "ZaaktypenMappingId", "DetPublicatieNiveau" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumenttypeMappings");

            migrationBuilder.DropTable(
                name: "PublicatieNiveauMappings");

            migrationBuilder.CreateTable(
                name: "DocumentPropertyMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetPropertyName = table.Column<string>(type: "text", nullable: false),
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
                name: "IX_DocumentPropertyMapping_ZaaktypenMappingId_DetPropertyName_DetValue_Unique",
                table: "DocumentPropertyMappings",
                columns: new[] { "ZaaktypenMappingId", "DetPropertyName", "DetValue" },
                unique: true);
        }
    }
}
