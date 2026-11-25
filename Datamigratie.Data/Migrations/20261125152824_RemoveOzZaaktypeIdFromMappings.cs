using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOzZaaktypeIdFromMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OzZaaktypeId",
                table: "Mappings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OzZaaktypeId",
                table: "Mappings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
