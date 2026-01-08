using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddResultaattypeMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResultaattypeMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DetZaaktypeId = table.Column<string>(type: "text", nullable: false),
                    DetResultaattypeId = table.Column<string>(type: "text", nullable: false),
                    OzZaaktypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OzResultaattypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultaattypeMappings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResultaattypeMapping_DetZaaktypeId_DetResultaattypeId_Unique",
                table: "ResultaattypeMappings",
                columns: new[] { "DetZaaktypeId", "DetResultaattypeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultaattypeMappings");
        }
    }
}
