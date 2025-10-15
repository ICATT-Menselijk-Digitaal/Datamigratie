using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameToDetZaaktypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MigrationTrackers_ZaaktypeId",
                table: "MigrationTrackers");

            migrationBuilder.DropColumn(
                name: "ZaaktypeId",
                table: "MigrationTrackers");

            migrationBuilder.AddColumn<string>(
                name: "DetZaaktypeId",
                table: "MigrationTrackers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MigrationTrackers_DetZaaktypeId",
                table: "MigrationTrackers",
                column: "DetZaaktypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MigrationTrackers_DetZaaktypeId",
                table: "MigrationTrackers");

            migrationBuilder.DropColumn(
                name: "DetZaaktypeId",
                table: "MigrationTrackers");

            migrationBuilder.AddColumn<Guid>(
                name: "ZaaktypeId",
                table: "MigrationTrackers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MigrationTrackers_ZaaktypeId",
                table: "MigrationTrackers",
                column: "ZaaktypeId");
        }
    }
}
