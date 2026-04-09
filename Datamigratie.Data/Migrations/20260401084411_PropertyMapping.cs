using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class PropertyMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BesluittypeMappings");

            migrationBuilder.DropTable(
                name: "DocumentstatusMappings");

            migrationBuilder.DropTable(
                name: "DocumenttypeMappings");

            migrationBuilder.DropTable(
                name: "PdfInformatieobjecttypeMappings");

            migrationBuilder.DropTable(
                name: "PublicatieNiveauMappings");

            migrationBuilder.DropTable(
                name: "ResultaattypeMappings");

            migrationBuilder.DropTable(
                name: "RoltypeMappings");

            migrationBuilder.DropTable(
                name: "RsinConfigurations");

            migrationBuilder.DropTable(
                name: "StatusMappings");

            migrationBuilder.DropTable(
                name: "VertrouwelijkheidMappings");

            migrationBuilder.CreateTable(
                name: "PropertyMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: true),
                    Property = table.Column<string>(type: "text", nullable: false),
                    SourceId = table.Column<string>(type: "text", nullable: true),
                    TargetId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyMappings_Mappings_ZaaktypenMappingId",
                        column: x => x.ZaaktypenMappingId,
                        principalTable: "Mappings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyMappings_ZaaktypenMappingId_Property",
                table: "PropertyMappings",
                columns: new[] { "ZaaktypenMappingId", "Property" });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyMappings_ZaaktypenMappingId_Property_SourceId",
                table: "PropertyMappings",
                columns: new[] { "ZaaktypenMappingId", "Property", "SourceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyMappings");

            migrationBuilder.CreateTable(
                name: "BesluittypeMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetBesluittypeNaam = table.Column<string>(type: "text", nullable: false),
                    OzBesluittypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BesluittypeMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BesluittypeMappings_Mappings_ZaaktypenMappingId",
                        column: x => x.ZaaktypenMappingId,
                        principalTable: "Mappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentstatusMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DetDocumentstatus = table.Column<string>(type: "text", nullable: false),
                    OzDocumentstatus = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentstatusMappings", x => x.Id);
                });

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
                name: "PdfInformatieobjecttypeMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: false),
                    OzInformatieobjecttypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdfInformatieobjecttypeMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PdfInformatieobjecttypeMappings_Mappings_ZaaktypenMappingId",
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

            migrationBuilder.CreateTable(
                name: "ResultaattypeMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetResultaattypeNaam = table.Column<string>(type: "text", nullable: false),
                    OzResultaattypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultaattypeMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultaattypeMappings_Mappings_ZaaktypenMappingId",
                        column: x => x.ZaaktypenMappingId,
                        principalTable: "Mappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoltypeMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: false),
                    AlleenPdf = table.Column<bool>(type: "boolean", nullable: false),
                    DetRol = table.Column<string>(type: "text", nullable: false),
                    OzRoltypeUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoltypeMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoltypeMappings_Mappings_ZaaktypenMappingId",
                        column: x => x.ZaaktypenMappingId,
                        principalTable: "Mappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RsinConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rsin = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsinConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetStatusNaam = table.Column<string>(type: "text", nullable: false),
                    OzStatustypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusMappings_Mappings_ZaaktypenMappingId",
                        column: x => x.ZaaktypenMappingId,
                        principalTable: "Mappings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_BesluittypeMapping_ZaaktypenMappingId_DetBesluittypeNaam_Unique",
                table: "BesluittypeMappings",
                columns: new[] { "ZaaktypenMappingId", "DetBesluittypeNaam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentstatusMapping_DetDocumentstatus_Unique",
                table: "DocumentstatusMappings",
                column: "DetDocumentstatus",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumenttypeMapping_ZaaktypenMappingId_DetDocumenttypeNaam_Unique",
                table: "DocumenttypeMappings",
                columns: new[] { "ZaaktypenMappingId", "DetDocumenttypeNaam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PdfInformatieobjecttypeMapping_ZaaktypenMappingId_Unique",
                table: "PdfInformatieobjecttypeMappings",
                column: "ZaaktypenMappingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicatieNiveauMapping_ZaaktypenMappingId_DetPublicatieNiveau_Unique",
                table: "PublicatieNiveauMappings",
                columns: new[] { "ZaaktypenMappingId", "DetPublicatieNiveau" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeNaam_Unique",
                table: "ResultaattypeMappings",
                columns: new[] { "ZaaktypenMappingId", "DetResultaattypeNaam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoltypeMapping_ZaaktypenMappingId_DetRol_Unique",
                table: "RoltypeMappings",
                columns: new[] { "ZaaktypenMappingId", "DetRol" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusMapping_ZaaktypenMappingId_DetStatusNaam_Unique",
                table: "StatusMappings",
                columns: new[] { "ZaaktypenMappingId", "DetStatusNaam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VertrouwelijkheidMapping_ZaaktypenMappingId_DetVertrouwelijkheid_Unique",
                table: "VertrouwelijkheidMappings",
                columns: new[] { "ZaaktypenMappingId", "DetVertrouwelijkheid" },
                unique: true);
        }
    }
}
