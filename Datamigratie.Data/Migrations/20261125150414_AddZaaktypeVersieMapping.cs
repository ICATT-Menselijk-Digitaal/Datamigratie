using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddZaaktypeVersieMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BesluittypeMappings_Mappings_ZaaktypenMappingId",
                table: "BesluittypeMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumenttypeMappings_Mappings_ZaaktypenMappingId",
                table: "DocumenttypeMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_PdfInformatieobjecttypeMappings_Mappings_ZaaktypenMappingId",
                table: "PdfInformatieobjecttypeMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicatieNiveauMappings_Mappings_ZaaktypenMappingId",
                table: "PublicatieNiveauMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultaattypeMappings_Mappings_ZaaktypenMappingId",
                table: "ResultaattypeMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_RoltypeMappings_Mappings_ZaaktypenMappingId",
                table: "RoltypeMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_StatusMappings_Mappings_ZaaktypenMappingId",
                table: "StatusMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_VertrouwelijkheidMappings_Mappings_ZaaktypenMappingId",
                table: "VertrouwelijkheidMappings");

            migrationBuilder.RenameColumn(
                name: "ZaaktypenMappingId",
                table: "VertrouwelijkheidMappings",
                newName: "ZaaktypeVersieMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_VertrouwelijkheidMapping_ZaaktypenMappingId_DetVertrouwelijkheid_Unique",
                table: "VertrouwelijkheidMappings",
                newName: "IX_VertrouwelijkheidMapping_ZaaktypeVersieMappingId_DetVertrouwelijkheid_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypenMappingId",
                table: "StatusMappings",
                newName: "ZaaktypeVersieMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_StatusMapping_ZaaktypenMappingId_DetStatusNaam_Unique",
                table: "StatusMappings",
                newName: "IX_StatusMapping_ZaaktypeVersieMappingId_DetStatusNaam_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypenMappingId",
                table: "RoltypeMappings",
                newName: "ZaaktypeVersieMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_RoltypeMapping_ZaaktypenMappingId_DetRol_Unique",
                table: "RoltypeMappings",
                newName: "IX_RoltypeMapping_ZaaktypeVersieMappingId_DetRol_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypenMappingId",
                table: "ResultaattypeMappings",
                newName: "ZaaktypeVersieMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeNaam_Unique",
                table: "ResultaattypeMappings",
                newName: "IX_ResultaattypeMapping_ZaaktypeVersieMappingId_DetResultaattypeNaam_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypenMappingId",
                table: "PublicatieNiveauMappings",
                newName: "ZaaktypeVersieMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_PublicatieNiveauMapping_ZaaktypenMappingId_DetPublicatieNiveau_Unique",
                table: "PublicatieNiveauMappings",
                newName: "IX_PublicatieNiveauMapping_ZaaktypeVersieMappingId_DetPublicatieNiveau_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypenMappingId",
                table: "PdfInformatieobjecttypeMappings",
                newName: "ZaaktypeVersieMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_PdfInformatieobjecttypeMapping_ZaaktypenMappingId_Unique",
                table: "PdfInformatieobjecttypeMappings",
                newName: "IX_PdfInformatieobjecttypeMapping_ZaaktypeVersieMappingId_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypenMappingId",
                table: "DocumenttypeMappings",
                newName: "ZaaktypeVersieMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumenttypeMapping_ZaaktypenMappingId_DetDocumenttypeNaam_Unique",
                table: "DocumenttypeMappings",
                newName: "IX_DocumenttypeMapping_ZaaktypeVersieMappingId_DetDocumenttypeNaam_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypenMappingId",
                table: "BesluittypeMappings",
                newName: "ZaaktypeVersieMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_BesluittypeMapping_ZaaktypenMappingId_DetBesluittypeNaam_Unique",
                table: "BesluittypeMappings",
                newName: "IX_BesluittypeMapping_ZaaktypeVersieMappingId_DetBesluittypeNaam_Unique");

            migrationBuilder.AddColumn<Guid>(
                name: "OzCatalogusId",
                table: "Mappings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OzZaaktypeIdentificatie",
                table: "Mappings",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ZaaktypeVersieMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: false),
                    SelectielijstJaar = table.Column<int>(type: "integer", nullable: true),
                    OzZaaktypeVersieId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZaaktypeVersieMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZaaktypeVersieMappings_Mappings_ZaaktypenMappingId",
                        column: x => x.ZaaktypenMappingId,
                        principalTable: "Mappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ZaaktypeVersieMapping_ZaaktypenMappingId_SelectielijstJaar_Unique",
                table: "ZaaktypeVersieMappings",
                columns: new[] { "ZaaktypenMappingId", "SelectielijstJaar" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BesluittypeMappings_ZaaktypeVersieMappings_ZaaktypeVersieMa~",
                table: "BesluittypeMappings",
                column: "ZaaktypeVersieMappingId",
                principalTable: "ZaaktypeVersieMappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumenttypeMappings_ZaaktypeVersieMappings_ZaaktypeVersieM~",
                table: "DocumenttypeMappings",
                column: "ZaaktypeVersieMappingId",
                principalTable: "ZaaktypeVersieMappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PdfInformatieobjecttypeMappings_ZaaktypeVersieMappings_Zaak~",
                table: "PdfInformatieobjecttypeMappings",
                column: "ZaaktypeVersieMappingId",
                principalTable: "ZaaktypeVersieMappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicatieNiveauMappings_ZaaktypeVersieMappings_ZaaktypeVer~",
                table: "PublicatieNiveauMappings",
                column: "ZaaktypeVersieMappingId",
                principalTable: "ZaaktypeVersieMappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultaattypeMappings_ZaaktypeVersieMappings_ZaaktypeVersie~",
                table: "ResultaattypeMappings",
                column: "ZaaktypeVersieMappingId",
                principalTable: "ZaaktypeVersieMappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoltypeMappings_ZaaktypeVersieMappings_ZaaktypeVersieMappin~",
                table: "RoltypeMappings",
                column: "ZaaktypeVersieMappingId",
                principalTable: "ZaaktypeVersieMappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatusMappings_ZaaktypeVersieMappings_ZaaktypeVersieMapping~",
                table: "StatusMappings",
                column: "ZaaktypeVersieMappingId",
                principalTable: "ZaaktypeVersieMappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VertrouwelijkheidMappings_ZaaktypeVersieMappings_ZaaktypeVe~",
                table: "VertrouwelijkheidMappings",
                column: "ZaaktypeVersieMappingId",
                principalTable: "ZaaktypeVersieMappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BesluittypeMappings_ZaaktypeVersieMappings_ZaaktypeVersieMa~",
                table: "BesluittypeMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumenttypeMappings_ZaaktypeVersieMappings_ZaaktypeVersieM~",
                table: "DocumenttypeMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_PdfInformatieobjecttypeMappings_ZaaktypeVersieMappings_Zaak~",
                table: "PdfInformatieobjecttypeMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicatieNiveauMappings_ZaaktypeVersieMappings_ZaaktypeVer~",
                table: "PublicatieNiveauMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_ResultaattypeMappings_ZaaktypeVersieMappings_ZaaktypeVersie~",
                table: "ResultaattypeMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_RoltypeMappings_ZaaktypeVersieMappings_ZaaktypeVersieMappin~",
                table: "RoltypeMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_StatusMappings_ZaaktypeVersieMappings_ZaaktypeVersieMapping~",
                table: "StatusMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_VertrouwelijkheidMappings_ZaaktypeVersieMappings_ZaaktypeVe~",
                table: "VertrouwelijkheidMappings");

            migrationBuilder.DropTable(
                name: "ZaaktypeVersieMappings");

            migrationBuilder.DropColumn(
                name: "OzCatalogusId",
                table: "Mappings");

            migrationBuilder.DropColumn(
                name: "OzZaaktypeIdentificatie",
                table: "Mappings");

            migrationBuilder.RenameColumn(
                name: "ZaaktypeVersieMappingId",
                table: "VertrouwelijkheidMappings",
                newName: "ZaaktypenMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_VertrouwelijkheidMapping_ZaaktypeVersieMappingId_DetVertrouwelijkheid_Unique",
                table: "VertrouwelijkheidMappings",
                newName: "IX_VertrouwelijkheidMapping_ZaaktypenMappingId_DetVertrouwelijkheid_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypeVersieMappingId",
                table: "StatusMappings",
                newName: "ZaaktypenMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_StatusMapping_ZaaktypeVersieMappingId_DetStatusNaam_Unique",
                table: "StatusMappings",
                newName: "IX_StatusMapping_ZaaktypenMappingId_DetStatusNaam_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypeVersieMappingId",
                table: "RoltypeMappings",
                newName: "ZaaktypenMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_RoltypeMapping_ZaaktypeVersieMappingId_DetRol_Unique",
                table: "RoltypeMappings",
                newName: "IX_RoltypeMapping_ZaaktypenMappingId_DetRol_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypeVersieMappingId",
                table: "ResultaattypeMappings",
                newName: "ZaaktypenMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_ResultaattypeMapping_ZaaktypeVersieMappingId_DetResultaattypeNaam_Unique",
                table: "ResultaattypeMappings",
                newName: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeNaam_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypeVersieMappingId",
                table: "PublicatieNiveauMappings",
                newName: "ZaaktypenMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_PublicatieNiveauMapping_ZaaktypeVersieMappingId_DetPublicatieNiveau_Unique",
                table: "PublicatieNiveauMappings",
                newName: "IX_PublicatieNiveauMapping_ZaaktypenMappingId_DetPublicatieNiveau_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypeVersieMappingId",
                table: "PdfInformatieobjecttypeMappings",
                newName: "ZaaktypenMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_PdfInformatieobjecttypeMapping_ZaaktypeVersieMappingId_Unique",
                table: "PdfInformatieobjecttypeMappings",
                newName: "IX_PdfInformatieobjecttypeMapping_ZaaktypenMappingId_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypeVersieMappingId",
                table: "DocumenttypeMappings",
                newName: "ZaaktypenMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumenttypeMapping_ZaaktypeVersieMappingId_DetDocumenttypeNaam_Unique",
                table: "DocumenttypeMappings",
                newName: "IX_DocumenttypeMapping_ZaaktypenMappingId_DetDocumenttypeNaam_Unique");

            migrationBuilder.RenameColumn(
                name: "ZaaktypeVersieMappingId",
                table: "BesluittypeMappings",
                newName: "ZaaktypenMappingId");

            migrationBuilder.RenameIndex(
                name: "IX_BesluittypeMapping_ZaaktypeVersieMappingId_DetBesluittypeNaam_Unique",
                table: "BesluittypeMappings",
                newName: "IX_BesluittypeMapping_ZaaktypenMappingId_DetBesluittypeNaam_Unique");

            migrationBuilder.AddForeignKey(
                name: "FK_BesluittypeMappings_Mappings_ZaaktypenMappingId",
                table: "BesluittypeMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumenttypeMappings_Mappings_ZaaktypenMappingId",
                table: "DocumenttypeMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PdfInformatieobjecttypeMappings_Mappings_ZaaktypenMappingId",
                table: "PdfInformatieobjecttypeMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicatieNiveauMappings_Mappings_ZaaktypenMappingId",
                table: "PublicatieNiveauMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultaattypeMappings_Mappings_ZaaktypenMappingId",
                table: "ResultaattypeMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoltypeMappings_Mappings_ZaaktypenMappingId",
                table: "RoltypeMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatusMappings_Mappings_ZaaktypenMappingId",
                table: "StatusMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VertrouwelijkheidMappings_Mappings_ZaaktypenMappingId",
                table: "VertrouwelijkheidMappings",
                column: "ZaaktypenMappingId",
                principalTable: "Mappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
