using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class ZaaktypenMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mappings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    oz_zaaktype_id = table.Column<Guid>(type: "uuid", nullable: false),
                    det_zaaktype_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mappings", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mapping_DetZaaktypeId_Unique",
                table: "mappings",
                column: "det_zaaktype_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mappings");
        }
    }
}
