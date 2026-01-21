using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameGlobalConfigurationToRsinConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalConfigurations");

            migrationBuilder.RenameIndex(
                name: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeNaam_Unique",
                table: "ResultaattypeMappings",
                newName: "IX_ResultaattypeMappings_ZaaktypenMappingId_DetResultaattypeNa~");

            migrationBuilder.CreateTable(
                name: "DocumentstatusMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DetDocumentstatus = table.Column<string>(type: "text", nullable: false),
                    OzDocumentstatus = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentstatusMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RsinConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rsin = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsinConfigurations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentstatusMapping_DetDocumentstatus_Unique",
                table: "DocumentstatusMappings",
                column: "DetDocumentstatus",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentstatusMappings");

            migrationBuilder.DropTable(
                name: "RsinConfigurations");

            migrationBuilder.RenameIndex(
                name: "IX_ResultaattypeMappings_ZaaktypenMappingId_DetResultaattypeNa~",
                table: "ResultaattypeMappings",
                newName: "IX_ResultaattypeMapping_ZaaktypenMappingId_DetResultaattypeNaam_Unique");

            migrationBuilder.CreateTable(
                name: "GlobalConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rsin = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalConfigurations", x => x.Id);
                });
        }
    }
}
