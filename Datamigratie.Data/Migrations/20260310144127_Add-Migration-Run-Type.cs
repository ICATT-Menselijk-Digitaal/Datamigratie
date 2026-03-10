using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMigrationRunType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationType",
                table: "Migrations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationType",
                table: "Migrations");
        }
    }
}
