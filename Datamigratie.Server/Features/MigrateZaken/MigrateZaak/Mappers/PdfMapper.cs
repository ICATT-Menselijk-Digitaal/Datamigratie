using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class PdfMapper(string rsin, Uri informatieobjecttypeUri)
{
    public OzDocument Map(DetZaak detZaak)
    {
        var fileName = $"zaakgegevens_{detZaak.FunctioneleIdentificatie}.pdf";

        return new OzDocument
        {
            Bestandsnaam = fileName,
            Bronorganisatie = rsin,
            Formaat = "application/pdf",
            Identificatie = $"zaakgegevens-{detZaak.FunctioneleIdentificatie}",
            Informatieobjecttype = informatieobjecttypeUri,
            Taal = "dut",
            Titel = $"e-Suite zaakgegevens {detZaak.FunctioneleIdentificatie}",
            Vertrouwelijkheidaanduiding = DocumentVertrouwelijkheidaanduiding.openbaar,
            Bestandsomvang = 0, // will be set by the shell after PDF generation
            Auteur = "Automatisch gegenereerd bij migratie vanuit e-Suite",
            Beschrijving = "Automatisch gegenereerd document met basisgegevens van de zaak uit het bronsysteem",
            Creatiedatum = DateOnly.FromDateTime(DateTime.Now),
            Status = DocumentStatus.definitief,
            Link = "",
            Verschijningsvorm = "",
            Trefwoorden = [],
        };
    }
}
