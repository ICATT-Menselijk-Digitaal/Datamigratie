using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class MaxMigrationRecordErrorLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"MigrationRecords\" SET \"ErrorDetails\" = LEFT(\"ErrorDetails\", 10000) WHERE LENGTH(\"ErrorDetails\") > 10000;"
            );

            migrationBuilder.AlterColumn<string>(
                name: "ErrorDetails",
                table: "MigrationRecords",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ErrorDetails",
                table: "MigrationRecords",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000,
                oldNullable: true);
        }
    }
}
