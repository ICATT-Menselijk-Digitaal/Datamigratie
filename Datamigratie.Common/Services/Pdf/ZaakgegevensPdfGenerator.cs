using Datamigratie.Common.Services.Det.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Datamigratie.Common.Services.Pdf
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
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}
