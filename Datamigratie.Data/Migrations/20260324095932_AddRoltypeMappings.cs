using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoltypeMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoltypeMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ZaaktypenMappingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetRol = table.Column<string>(type: "text", nullable: false),
                    OzRoltypeUrl = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_RoltypeMapping_ZaaktypenMappingId_DetRol_Unique",
                table: "RoltypeMappings",
                columns: new[] { "ZaaktypenMappingId", "DetRol" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoltypeMappings");
        }
    }
}
