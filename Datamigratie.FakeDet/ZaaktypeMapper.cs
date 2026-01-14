

namespace Datamigratie.FakeDet;

using CatalogusZaaktype = Catalogi.Models.Zaaktype;
using CatalogusZaaktypeStatus = Catalogi.Models.ZaaktypeStatus;
using CatalogusZaaktypeResultaat = Catalogi.Models.ZaaktypeResultaat;
using CatalogusZaaktypeDocumenttype = Catalogi.Models.ZaaktypeDocumenttype;
using CatalogusZaaktypeBesluittype = Catalogi.Models.ZaaktypeBesluittype;
using CatalogusGerelateerdeZaaktype = Catalogi.Models.GerelateerdeZaaktype;
/// <summary>
/// Maps zaaktype from catalog format to FakeDet format.
/// </summary>
public static class ZaaktypeMapper
{
    /// <summary>
    /// Maps a catalog zaaktype to FakeDet zaaktype format.
    /// </summary>
    public static Zaaktype ToDetZaaktype(this CatalogusZaaktype source)
    {
        var doorlooptijdDagen = ParseIsoDurationToDays(source.Doorlooptijd);
        var servicenormDagen = ParseIsoDurationToDays(source.Servicenorm);
        var startStatus = source.Statussen?.FirstOrDefault(s => s.IsStartstatus == true);

        return new Zaaktype
        {
            // From ZaaktypeOverzicht (base class)
            FunctioneleIdentificatie = source.Identificatie ?? source.Omschrijving,
            Naam = $"{source.Domein}-{source.Omschrijving}",
            Omschrijving = source.OmschrijvingGeneriek ?? source.Toelichting,
            Actief = true,

            // Core properties
            HandelingInitiator = new HandelingInitiator
            {
                Naam = source.HandelingInitiator ?? "indienen",
                Omschrijving = source.HandelingInitiator,
                Actief = true
            },
            InternExtern = MapInternExtern(source.IndicatieInternOfExtern),
            Categorie = new Categorie
            {
                Naam = source.Categorie?.Naam ?? source.Domein ?? "Algemeen",
                Omschrijving = source.Categorie?.Omschrijving,
                Actief = source.Categorie?.Actief ?? true
            },
            Iv3Categorie = new Iv3Categorie
            {
                Naam = source.Domein ?? "Onbekend",
                Omschrijving = null,
                Actief = true,
                ExterneCode = null
            },

            // Assignment
            Afdeling = source.Verantwoordelijke,
            Groep = null,
            Intake = false,

            // Dates
            BeginGeldigheidDatum = ParseDateOrDefault(source.BeginGeldigheid),
            EindeGeldigheidDatum = ParseDateOrNull(source.EindeGeldigheid),

            // Doorlooptijd
            DoorlooptijdGewenst = servicenormDagen ?? doorlooptijdDagen,
            DoorlooptijdVereist = doorlooptijdDagen,
            DoorlooptijdAanpassenToegestaan = source.VerlengingMogelijk,

            // Signalering (defaults)
            AantalDagenVoorStreefdatumVoorEersteSignalering = 7,
            AantalDagenVoorStreefdatumVoorTweedeSignalering = 3,
            AantalDagenVoorFataledatumVoorEersteSignalering = 5,
            AantalDagenVoorFataledatumVoorTweedeSignalering = 2,

            // Initial status
            Status = startStatus != null ? MapStatus(startStatus) : null,

            // Archiving
            ArchiveringReviewPeriode = source.Archivering?.ReviewPeriode,

            // Process
            StartenProces = true,
            Proces = source.Referentieproces?.Naam,
            Startformulier = null,
            StartformulierVersie = null,

            // Security
            Vertrouwelijk = IsVertrouwelijk(source.Vertrouwelijkheidaanduiding),
            Authenticaties = MapAuthenticaties(source.IndicatieInternOfExtern),
            GeautoriseerdVoorMedewerkers = IsVertrouwelijk(source.Vertrouwelijkheidaanduiding),
            GeautoriseerdeMedewerkers = null,
            NotificatiesVersturen = source.PublicatieIndicatie,

            // Collections
            Statussen = source.Statussen?.Select(MapStatus).ToList(),
            Resultaten = source.Resultaten?.Select(MapResultaat).ToList(),
            Besluiten = source.Besluittypen?.Select(MapBesluittype).ToList(),
            Documenttypen = source.Documenttypen?.Select(MapDocumenttype).ToList(),
            DocumentTags = null,
            GekoppeldeZaaktypen = source.GerelateerdeZaaktypen?.Select(MapGekoppeldZaaktype).ToList(),
            TaakDocumentGroepen = null,

            // Other
            SamenvattingDocumentNaam = $"Samenvatting {source.Omschrijving}",
            ZaakStartParameters = null,
            Productaanvraagtype = null
        };
    }

    private static InternExtern MapInternExtern(string? value)
    {
        return value?.ToLowerInvariant() switch
        {
            "intern" => InternExtern.Intern,
            "extern" => InternExtern.Extern,
            _ => InternExtern.Extern
        };
    }

    private static bool IsVertrouwelijk(string? vertrouwelijkheidaanduiding)
    {
        return vertrouwelijkheidaanduiding?.ToLowerInvariant() switch
        {
            "openbaar" or "beperkt_openbaar" => false,
            "intern" or "zaakvertrouwelijk" or "vertrouwelijk" or "confidentieel" or "geheim" or "zeer_geheim" => true,
            _ => false
        };
    }

    private static int? ParseIsoDurationToDays(string? duration)
    {
        if (string.IsNullOrEmpty(duration))
            return null;

        // Parse ISO 8601 duration format like P30D, P14D, etc.
        if (duration.StartsWith("P") && duration.EndsWith("D"))
        {
            var daysStr = duration[1..^1];
            if (int.TryParse(daysStr, out var days))
                return days;
        }

        return null;
    }

    private static DateOnly ParseDateOrDefault(string? date)
    {
        if (string.IsNullOrEmpty(date))
            return DateOnly.FromDateTime(DateTime.Today);

        if (DateOnly.TryParse(date, out var result))
            return result;

        return DateOnly.FromDateTime(DateTime.Today);
    }

    private static DateOnly? ParseDateOrNull(string? date)
    {
        if (string.IsNullOrEmpty(date))
            return null;

        if (DateOnly.TryParse(date, out var result))
            return result;

        return null;
    }

    private static Zaakstatus MapStatus(CatalogusZaaktypeStatus source)
    {
        return new Zaakstatus
        {
            Naam = source.Omschrijving,
            Omschrijving = source.OmschrijvingGeneriek ?? source.Toelichting,
            Actief = true,
            Uitwisselingscode = source.Omschrijving.ToUpperInvariant().Replace(" ", "_"),
            ExterneNaam = source.Omschrijving,
            Start = source.IsStartstatus ?? false,
            Eind = source.IsEindstatus ?? false
        };
    }

    private static ZaaktypeResultaat MapResultaat(CatalogusZaaktypeResultaat source)
    {
        var archivering = source.Archivering;

        return new ZaaktypeResultaat
        {
            Resultaat = new Resultaat
            {
                Naam = source.Omschrijving,
                Omschrijving = source.OmschrijvingGeneriek ?? source.Toelichting,
                Actief = true,
                Uitwisselingscode = source.Omschrijving.ToUpperInvariant().Replace(" ", "_")
            },
            Selectielijstitem = new Selectielijstitem
            {
                Naam = source.SelectielijstItem ?? source.Omschrijving,
                Omschrijving = null,
                Actief = true,
                Jaar = 2020,
                Domein = null,
                Subdomein = null,
                BewaartermijnWaardering = MapBewaartermijnWaardering(archivering?.Waardering),
                Bewaartermijn = archivering?.Bewaartermijn,
                BewaartermijnEenheid = MapBewaartermijnEenheid(archivering?.BewaartermijnEenheid)
            },
            BewaartermijnWaardering = MapBewaartermijnWaardering(archivering?.Waardering),
            Bewaartermijn = archivering?.Bewaartermijn,
            BewaartermijnEenheid = MapBewaartermijnEenheid(archivering?.BewaartermijnEenheid)
        };
    }

    private static BewaartermijnWaardering? MapBewaartermijnWaardering(string? value)
    {
        return value?.ToLowerInvariant() switch
        {
            "bewaar" or "bewaren" => BewaartermijnWaardering.Bewaar,
            "vernietig" or "vernietigen" => BewaartermijnWaardering.Vernietig,
            _ => null
        };
    }

    private static BewaartermijnEenheid? MapBewaartermijnEenheid(string? value)
    {
        return value?.ToLowerInvariant() switch
        {
            "maanden" => BewaartermijnEenheid.Maanden,
            "jaren" => BewaartermijnEenheid.Jaren,
            _ => null
        };
    }

    private static Datamigratie.FakeDet.ZaaktypeDocumenttype MapDocumenttype(CatalogusZaaktypeDocumenttype source)
    {
        return new Datamigratie.FakeDet.ZaaktypeDocumenttype
        {
            Documenttype = new Documenttype
            {
                Naam = source.Naam ?? source.Omschrijving ?? "Document",
                Omschrijving = source.Omschrijving,
                Actief = true,
                Documentcategorie = source.Documentcategorie,
                Publicatieniveau = MapPublicatieniveau(source.Vertrouwelijkheidaanduiding)
            },
            Dspcode = null,
            Titels = null
        };
    }

    private static DocumentPublicatieniveau MapPublicatieniveau(string? vertrouwelijkheidaanduiding)
    {
        return vertrouwelijkheidaanduiding?.ToLowerInvariant() switch
        {
            "openbaar" or "beperkt_openbaar" => DocumentPublicatieniveau.Extern,
            "intern" or "zaakvertrouwelijk" => DocumentPublicatieniveau.Intern,
            "vertrouwelijk" or "confidentieel" or "geheim" or "zeer_geheim" => DocumentPublicatieniveau.Vertrouwelijk,
            _ => DocumentPublicatieniveau.Intern
        };
    }

    private static Datamigratie.FakeDet.ZaaktypeBesluittype MapBesluittype(CatalogusZaaktypeBesluittype source)
    {
        return new Datamigratie.FakeDet.ZaaktypeBesluittype
        {
            Besluittype = new Besluittype
            {
                Naam = source.Naam ?? source.Omschrijving ?? "Besluit",
                Omschrijving = source.Omschrijving,
                Actief = true,
                Besluitcategorie = new Besluitcategorie
                {
                    Naam = source.Besluitcategorie ?? "Overig",
                    Omschrijving = null,
                    Actief = true
                },
                ReactietermijnInDagen = source.ReactietermijnInDagen ?? 42,
                PublicatieIndicatie = source.PublicatieIndicatie,
                Publicatietekst = null,
                PublicatietermijnInDagen = source.PublicatietermijnInDagen
            },
            Documenttype = null,
            Procestermijn = null
        };
    }

    private static ZaaktypeOverzicht MapGekoppeldZaaktype(CatalogusGerelateerdeZaaktype source)
    {
        return new ZaaktypeOverzicht
        {
            FunctioneleIdentificatie = source.ZaaktypeIdentificatie,
            Naam = source.ZaaktypeIdentificatie,
            Omschrijving = source.AardRelatie,
            Actief = true
        };
    }

    private static IReadOnlyList<ZaaktypeAuthenticatie>? MapAuthenticaties(string? internExtern)
    {
        // Only external zaaktypen need authentication
        if (internExtern?.ToLowerInvariant() != "extern")
            return null;

        return
        [
            new ZaaktypeAuthenticatie
            {
                Doelgroep = AuthenticatieDoelgroep.Burger,
                Niveau = AuthenticatieNiveau.Digid2
            },
            new ZaaktypeAuthenticatie
            {
                Doelgroep = AuthenticatieDoelgroep.Bedrijf,
                Niveau = AuthenticatieNiveau.Eherkenning2
            }
        ];
    }
}
