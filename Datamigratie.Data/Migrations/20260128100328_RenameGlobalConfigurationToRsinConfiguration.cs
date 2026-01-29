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

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
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

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "GlobalConfigurations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlobalConfigurations",
                table: "GlobalConfigurations",
                column: "Id");
        }
    }
}
