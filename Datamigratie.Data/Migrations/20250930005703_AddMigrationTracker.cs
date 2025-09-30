using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMigrationTracker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_mappings",
                table: "mappings");

            migrationBuilder.RenameTable(
                name: "mappings",
                newName: "Mappings");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Mappings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "oz_zaaktype_id",
                table: "Mappings",
                newName: "OzZaaktypeId");

            migrationBuilder.RenameColumn(
                name: "det_zaaktype_id",
                table: "Mappings",
                newName: "DetZaaktypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mappings",
                table: "Mappings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MigrationTrackers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ZaaktypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TotalRecords = table.Column<int>(type: "integer", nullable: false),
                    ProcessedRecords = table.Column<int>(type: "integer", nullable: false),
                    SuccessfulRecords = table.Column<int>(type: "integer", nullable: false),
                    FailedRecords = table.Column<int>(type: "integer", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MigrationTrackers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MigrationTrackers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mappings",
                table: "Mappings");

            migrationBuilder.RenameTable(
                name: "Mappings",
                newName: "mappings");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "mappings",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OzZaaktypeId",
                table: "mappings",
                newName: "oz_zaaktype_id");

            migrationBuilder.RenameColumn(
                name: "DetZaaktypeId",
                table: "mappings",
                newName: "det_zaaktype_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_mappings",
                table: "mappings",
                column: "id");
        }
    }
}
