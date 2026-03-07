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
                name: "DocumentPropertyMappings");

            migrationBuilder.DropTable(
                name: "DocumentstatusMappings");

            migrationBuilder.DropTable(
                name: "ResultaattypeMappings");

            migrationBuilder.DropTable(
                name: "StatusMappings");

            migrationBuilder.DropTable(
                name: "VertrouwelijkheidMappings");

            migrationBuilder.CreateTable(
                name: "PropertyMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MappingId = table.Column<Guid>(type: "uuid", nullable: true),
                    Property = table.Column<string>(type: "text", nullable: false),
                    SourceId = table.Column<string>(type: "text", nullable: true),
                    TargetId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyMappings_Mappings_MappingId",
                        column: x => x.MappingId,
                        principalTable: "Mappings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyMappings_MappingId_Property",
                table: "PropertyMappings",
                columns: new[] { "MappingId", "Property" });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyMappings_MappingId_Property_SourceId",
                table: "PropertyMappings",
                columns: new[] { "MappingId", "Property", "SourceId" },
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
                name: "IX_DocumentPropertyMapping_ZaaktypenMappingId_DetPropertyName_DetValue_Unique",
                table: "DocumentPropertyMappings",
                columns: new[] { "ZaaktypenMappingId", "DetPropertyName", "DetValue" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentstatusMapping_DetDocumentstatus_Unique",
                table: "DocumentstatusMappings",
                column: "DetDocumentstatus",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeNaam_Unique",
                table: "ResultaattypeMappings",
                columns: new[] { "ZaaktypenMappingId", "DetResultaattypeNaam" },
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
