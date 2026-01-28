using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameGlobalConfigurationToRsinConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GlobalConfigurations",
                table: "GlobalConfigurations");

            migrationBuilder.RenameTable(
                name: "GlobalConfigurations",
                newName: "RsinConfigurations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RsinConfigurations",
                table: "RsinConfigurations",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RsinConfigurations",
                table: "RsinConfigurations");

            migrationBuilder.RenameTable(
                name: "RsinConfigurations",
                newName: "GlobalConfigurations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalConfigurations",
                table: "GlobalConfigurations",
                column: "Id");
        }
    }
}
