using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMaxLengthAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MigrationRecords_IsSuccessful",
                table: "MigrationRecords");

            migrationBuilder.DropIndex(
                name: "IX_MigrationRecords_ProcessedAt",
                table: "MigrationRecords");

            migrationBuilder.AlterColumn<string>(
                name: "DetZaaknummer",
                table: "MigrationRecords",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "OzZaaknummer",
                table: "MigrationRecords",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorTitle",
                table: "MigrationRecords",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorDetails",
                table: "MigrationRecords",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MigrationRecords_IsSuccessful",
                table: "MigrationRecords",
                column: "IsSuccessful");

            migrationBuilder.CreateIndex(
                name: "IX_MigrationRecords_ProcessedAt",
                table: "MigrationRecords",
                column: "ProcessedAt");

            migrationBuilder.AlterColumn<string>(
                name: "DetZaaknummer",
                table: "MigrationRecords",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "OzZaaknummer",
                table: "MigrationRecords",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorTitle",
                table: "MigrationRecords",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorDetails",
                table: "MigrationRecords",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
