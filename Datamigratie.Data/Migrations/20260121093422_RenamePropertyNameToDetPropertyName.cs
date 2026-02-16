using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datamigratie.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamePropertyNameToDetPropertyName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PropertyName",
                table: "DocumentPropertyMappings",
                newName: "DetPropertyName");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentPropertyMapping_ZaaktypenMappingId_PropertyName_DetValue_Unique",
                table: "DocumentPropertyMappings",
                newName: "IX_DocumentPropertyMapping_ZaaktypenMappingId_DetPropertyName_DetValue_Unique");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DetPropertyName",
                table: "DocumentPropertyMappings",
                newName: "PropertyName");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentPropertyMapping_ZaaktypenMappingId_DetPropertyName_DetValue_Unique",
                table: "DocumentPropertyMappings",
                newName: "IX_DocumentPropertyMapping_ZaaktypenMappingId_PropertyName_DetValue_Unique");
        }
    }
}
