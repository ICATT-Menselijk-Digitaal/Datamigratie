using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAlleenPdfToRoltypeMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlleenPdf",
                table: "RoltypeMappings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "OzRoltypeUrl",
                table: "RoltypeMappings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RoltypeMappings_AlleenPdf_OzRoltypeUrl",
                table: "RoltypeMappings",
                sql: "\"AlleenPdf\" = false OR \"OzRoltypeUrl\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_RoltypeMappings_AlleenPdf_OzRoltypeUrl",
                table: "RoltypeMappings");

            migrationBuilder.DropColumn(
                name: "AlleenPdf",
                table: "RoltypeMappings");

            migrationBuilder.AlterColumn<string>(
                name: "OzRoltypeUrl",
                table: "RoltypeMappings",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
