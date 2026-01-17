using Datamigratie.Common.Services.Det.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Datamigratie.Server.Features.Migrate.MigrateZaak.Pdf
{
    public interface IZaakgegevensPdfGenerator
    {
        /// <summary>
        /// Generates a PDF document containing basic zaak information
        /// </summary>
        /// <param name="zaak">The DET zaak to generate PDF for</param>
        /// <returns>PDF document as byte array</returns>
        byte[] GenerateZaakgegevensPdf(DetZaak zaak);
    }

    public class ZaakgegevensPdfGenerator : IZaakgegevensPdfGenerator
    {
        public byte[] GenerateZaakgegevensPdf(DetZaak zaak)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(200);
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("functioneleIdentificatie");
                            table.Cell().Text(zaak.FunctioneleIdentificatie);

                            table.Cell().Text("aangemaaktDoor");
                            table.Cell().Text(zaak.AangemaaktDoor ?? "-");

                            table.Cell().Text("afdeling");
                            table.Cell().Text(zaak.Afdeling ?? "-");

                            table.Cell().Text("betaalgegevens");
                            table.Cell().Text(FormatBetaalgegevens(zaak.Betaalgegevens));
                        });
                });
            });

            return document.GeneratePdf();
        }

        private static string FormatBetaalgegevens(DetBetaalgegevens? betaalgegevens)
        {
            if (betaalgegevens == null)
                return "-";

            var parts = new List<string>();
            
            if (betaalgegevens.Bedrag.HasValue)
                parts.Add($"Bedrag: €{betaalgegevens.Bedrag.Value:F2}");
            
            if (!string.IsNullOrEmpty(betaalgegevens.Betaalstatus))
                parts.Add($"Status: {betaalgegevens.Betaalstatus}");
            
            if (!string.IsNullOrEmpty(betaalgegevens.Kenmerk))
                parts.Add($"Kenmerk: {betaalgegevens.Kenmerk}");

            return parts.Count > 0 ? string.Join(", ", parts) : "-";
        }
    }
}
