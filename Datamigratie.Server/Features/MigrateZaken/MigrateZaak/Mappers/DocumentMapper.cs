using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Plan;
using static Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers.StringTruncationHelper;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class DocumentMapper(
    string rsin,
    Dictionary<string, DocumentStatus> documentStatusMappings,
    Dictionary<string, DocumentVertrouwelijkheidaanduiding> publicatieNiveauMappings,
    Dictionary<string, Uri> documenttypeMappings)
{
    public DocumentMigrationPlan Map(DetDocument item)
    {
        var versions = item.DocumentVersies
            .OrderBy(v => v.Versienummer)
            .Select(v => Map(item, v))
            .ToList();
        return new DocumentMigrationPlan(versions);
    }

    private DocumentVersionPlan Map(DetDocument item, DetDocumentVersie versie)
    {
        const int MaxIdentificatieLength = 40;
        if (item.Kenmerk?.Length > MaxIdentificatieLength)
        {
            throw new InvalidDataException($"Document '{item.Titel}' migration failed: The 'kenmerk' field length ({item.Kenmerk.Length}) exceeds the maximum allowed length of {MaxIdentificatieLength} characters.");
        }

        const int MaxTitelLength = 200;
        var titel = TruncateWithDots(item.Titel, MaxTitelLength);

        const int MaxBeschijvingLength = 1000;
        var beschrijving = TruncateWithDots(item.Beschrijving, MaxBeschijvingLength);

        beschrijving ??= "";

        var verschijningsvorm = item.DocumentVorm?.Naam ?? "";

        var ozDocumentStatus = documentStatusMappings.TryGetValue(item.Documentstatus.Naam, out var ds) ? ds
            : throw new InvalidOperationException(
                $"Document '{item.Titel}' migration failed: No mapping found for DET document status '{item.Documentstatus.Naam}'.");

        if (string.IsNullOrWhiteSpace(item.Publicatieniveau))
        {
            throw new InvalidOperationException($"Document '{item.Titel}' migration failed: Publicatieniveau is required but was not provided.");
        }
        var vertrouwelijkheidaanduiding = publicatieNiveauMappings.TryGetValue(item.Publicatieniveau, out var pn) ? pn
            : throw new InvalidOperationException(
                $"Document '{item.Titel}' migration failed: Publicatieniveau '{item.Publicatieniveau}' has not been mapped to an OpenZaak vertrouwelijkheidaanduiding.");

        if (string.IsNullOrWhiteSpace(item.Documenttype?.Naam))
        {
            throw new InvalidOperationException($"Document '{item.Titel}' migration failed: Documenttype is required but was not provided.");
        }
        var informatieobjecttype = documenttypeMappings.TryGetValue(item.Documenttype.Naam, out var dt) ? dt
            : throw new InvalidOperationException(
                $"Document '{item.Titel}' migration failed: Documenttype '{item.Documenttype.Naam}' is set on this document, but is not mapped. Please add this documenttype to the corresponding zaaktype in the ESuite");

        var taal = item.Taal?.FunctioneelId.ToLower() ?? "dut";
        var auteur = versie.Auteur ?? "Auteur_onbekend";

        Ondertekening? ondertekening = null;
        if (versie.Ondertekeningen?.Count > 0)
        {
            var laasteOndertekening = versie.Ondertekeningen.OrderByDescending(o => o.OndertekenDatum).First();
            ondertekening = new Ondertekening
            {
                Datum = DateOnly.FromDateTime(laasteOndertekening.OndertekenDatum.DateTime),
                Soort = "digitaal"
            };
        }

        var doc = new OzDocument
        {
            Bestandsnaam = versie.Bestandsnaam,
            Bronorganisatie = rsin,
            Formaat = versie.Mimetype,
            Identificatie = item.Kenmerk ?? "",
            Informatieobjecttype = informatieobjecttype,
            Taal = taal,
            Titel = titel,
            Vertrouwelijkheidaanduiding = vertrouwelijkheidaanduiding,
            Bestandsomvang = versie.Documentgrootte,
            Auteur = auteur,
            Beschrijving = beschrijving,
            Creatiedatum = versie.Creatiedatum,
            Status = ozDocumentStatus,
            Verschijningsvorm = verschijningsvorm,
            Link = "",
            Trefwoorden = [],
            Ondertekening = ondertekening
        };

        return new DocumentVersionPlan(doc, versie.DocumentInhoudID);
    }
}
