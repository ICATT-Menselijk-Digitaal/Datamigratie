namespace Datamigratie.FakeDet;

using Source = Catalogi;

/// <summary>
/// Mapper voor het converteren van Catalogus Zaaktype naar Det Zaaktype.
/// </summary>
public static class ZaaktypeMapper
{
    /// <summary>
    /// Standaard waarden voor verplichte velden die niet in de bron aanwezig zijn.
    /// </summary>
    public static class Defaults
    {
        public const int AantalDagenVoorStreefdatumVoorEersteSignalering = 14;
        public const int AantalDagenVoorStreefdatumVoorTweedeSignalering = 7;
        public const int AantalDagenVoorFataledatumVoorEersteSignalering = 7;
        public const int AantalDagenVoorFataledatumVoorTweedeSignalering = 3;
        public const string SamenvattingDocumentNaam = "Samenvatting";
    }

    /// <summary>
    /// Converteert een Catalogus Zaaktype naar een Det Zaaktype.
    /// </summary>
    public static Zaaktype ToDetZaaktype(this Source.Zaaktype source) => new()
    {
        // Referentie properties
        Naam = source.Naam,
        Omschrijving = source.Omschrijving,
        Actief = true,

        // ZaaktypeOverzicht properties
        FunctioneleIdentificatie = source.FunctioneleIdentificatie,

        // Zaaktype properties
        HandelingInitiator = source.MapHandelingInitiator(),
        InternExtern = source.InternExtern.MapInternExtern(),
        Categorie = source.Categorie.MapCategorie(),
        Iv3Categorie = source.MapIv3Categorie(),
        Afdeling = source.Verantwoordelijke?.Afdeling,
        Groep = source.Verantwoordelijke?.Groep,
        Intake = false,
        BeginGeldigheidDatum = source.Geldigheid?.BeginDatum ?? DateOnly.FromDateTime(DateTime.Today),
        EindeGeldigheidDatum = source.Geldigheid?.EindeDatum,
        DoorlooptijdGewenst = source.Doorlooptijd?.Gewenst,
        DoorlooptijdVereist = source.Doorlooptijd?.Vereist,
        DoorlooptijdAanpassenToegestaan = source.Doorlooptijd?.AanpassenToegestaan ?? false,
        AantalDagenVoorStreefdatumVoorEersteSignalering = Defaults.AantalDagenVoorStreefdatumVoorEersteSignalering,
        AantalDagenVoorStreefdatumVoorTweedeSignalering = Defaults.AantalDagenVoorStreefdatumVoorTweedeSignalering,
        AantalDagenVoorFataledatumVoorEersteSignalering = Defaults.AantalDagenVoorFataledatumVoorEersteSignalering,
        AantalDagenVoorFataledatumVoorTweedeSignalering = Defaults.AantalDagenVoorFataledatumVoorTweedeSignalering,
        Status = source.Statussen?.FirstOrDefault(s => s.IsStartstatus == true)?.MapZaakstatus(),
        ArchiveringReviewPeriode = source.Archivering?.ReviewPeriode,
        StartenProces = false,
        Proces = null,
        Startformulier = null,
        StartformulierVersie = null,
        Vertrouwelijk = source.Vertrouwelijk ?? false,
        Authenticaties = null,
        GeautoriseerdVoorMedewerkers = false,
        GeautoriseerdeMedewerkers = null,
        NotificatiesVersturen = false,
        Statussen = source.Statussen?.Select(s => s.MapZaakstatus()).ToList(),
        Resultaten = source.Resultaten?.Select(r => r.MapZaaktypeResultaat(source.Archivering)).ToList(),
        Besluiten = source.Besluittypen?.Select(b => b.MapZaaktypeBesluittype()).ToList(),
        Documenttypen = source.Documenttypen?.Select(d => d.MapZaaktypeDocumenttype()).ToList(),
        DocumentTags = null,
        GekoppeldeZaaktypen = source.GerelateerdeZaaktypen?.DistinctBy(g => g.ZaaktypeIdentificatie).Select(g => g.MapZaaktypeOverzicht()).ToList(),
        TaakDocumentGroepen = null,
        SamenvattingDocumentNaam = Defaults.SamenvattingDocumentNaam,
        ZaakStartParameters = null,
        Productaanvraagtype = source.ProductenDiensten?.FirstOrDefault()?.Naam
    };

    /// <summary>
    /// Converteert een lijst van Catalogus Zaaktypen naar Det Zaaktypen.
    /// </summary>
    public static IReadOnlyList<Zaaktype> ToDetZaaktypen(this IEnumerable<Source.Zaaktype> sources) =>
        sources.Select(s => s.ToDetZaaktype()).ToList();

    #region HandelingInitiator Mapping

    private static HandelingInitiator MapHandelingInitiator(this Source.Zaaktype source) => new()
    {
        Naam = source.HandelingInitiator ?? "Indienen",
        Omschrijving = source.Aanleiding,
        Actief = true
    };

    #endregion

    #region InternExtern Mapping

    private static InternExtern MapInternExtern(this Source.InternExtern? source) => source switch
    {
        Source.InternExtern.Intern => InternExtern.Intern,
        Source.InternExtern.Extern => InternExtern.Extern,
        Source.InternExtern.Beide => InternExtern.Extern,
        _ => InternExtern.Extern
    };

    #endregion

    #region Categorie Mapping

    private static Categorie MapCategorie(this Source.Referentie? source) => new()
    {
        Naam = source?.Naam ?? "Onbekend",
        Omschrijving = source?.Omschrijving,
        Actief = source?.Actief ?? true
    };

    #endregion

    #region Iv3Categorie Mapping

    private static Iv3Categorie MapIv3Categorie(this Source.Zaaktype source) => new()
    {
        Naam = source.Domein ?? "Overig",
        Omschrijving = null,
        Actief = true,
        ExterneCode = null
    };

    #endregion

    #region Zaakstatus Mapping

    private static Zaakstatus MapZaakstatus(this Source.Zaakstatus source) => new()
    {
        Naam = source.Naam,
        Omschrijving = source.Omschrijving,
        Actief = true,
        Uitwisselingscode = source.Naam.ToUpperInvariant().Replace(' ', '_'),
        ExterneNaam = source.Naam,
        Start = source.IsStartstatus ?? false,
        Eind = source.IsEindstatus ?? false
    };

    #endregion

    #region Resultaat Mapping

    private static ZaaktypeResultaat MapZaaktypeResultaat(this Source.Resultaat source, Source.Archivering? zaaktypeArchivering) => new()
    {
        Resultaat = source.MapResultaat(),
        Selectielijstitem = source.MapSelectielijstitem(zaaktypeArchivering),
        BewaartermijnWaardering = source.Archivering?.Waardering.MapBewaartermijnWaardering()
            ?? zaaktypeArchivering?.Waardering.MapBewaartermijnWaardering(),
        Bewaartermijn = source.Archivering?.Bewaartermijn ?? zaaktypeArchivering?.Bewaartermijn,
        BewaartermijnEenheid = source.Archivering?.BewaartermijnEenheid.MapBewaartermijnEenheid()
            ?? zaaktypeArchivering?.BewaartermijnEenheid.MapBewaartermijnEenheid()
    };

    private static Resultaat MapResultaat(this Source.Resultaat source) => new()
    {
        Naam = source.Naam,
        Omschrijving = source.Omschrijving,
        Actief = true,
        Uitwisselingscode = source.Naam.ToUpperInvariant().Replace(' ', '_')
    };

    private static Selectielijstitem MapSelectielijstitem(this Source.Resultaat source, Source.Archivering? zaaktypeArchivering) => new()
    {
        Naam = source.SelectielijstItem ?? zaaktypeArchivering?.SelectielijstItem ?? "Onbekend",
        Omschrijving = null,
        Actief = true,
        Jaar = null,
        Domein = null,
        Subdomein = null,
        BewaartermijnWaardering = source.Archivering?.Waardering.MapBewaartermijnWaardering()
            ?? zaaktypeArchivering?.Waardering.MapBewaartermijnWaardering(),
        Bewaartermijn = source.Archivering?.Bewaartermijn ?? zaaktypeArchivering?.Bewaartermijn,
        BewaartermijnEenheid = source.Archivering?.BewaartermijnEenheid.MapBewaartermijnEenheid()
            ?? zaaktypeArchivering?.BewaartermijnEenheid.MapBewaartermijnEenheid()
    };

    private static BewaartermijnWaardering? MapBewaartermijnWaardering(this Source.Waardering? source) => source switch
    {
        Source.Waardering.Bewaar => BewaartermijnWaardering.Bewaar,
        Source.Waardering.Vernietig => BewaartermijnWaardering.Vernietig,
        _ => null
    };

    private static BewaartermijnEenheid? MapBewaartermijnEenheid(this Source.BewaartermijnEenheid? source) => source switch
    {
        Source.BewaartermijnEenheid.Maanden => BewaartermijnEenheid.Maanden,
        Source.BewaartermijnEenheid.Jaren => BewaartermijnEenheid.Jaren,
        _ => null
    };

    #endregion

    #region Besluittype Mapping

    private static ZaaktypeBesluittype MapZaaktypeBesluittype(this Source.Besluittype source) => new()
    {
        Besluittype = source.MapBesluittype(),
        Documenttype = null,
        Procestermijn = null
    };

    private static Besluittype MapBesluittype(this Source.Besluittype source) => new()
    {
        Naam = source.Naam,
        Omschrijving = source.Omschrijving,
        Actief = true,
        Besluitcategorie = new Besluitcategorie
        {
            Naam = source.Besluitcategorie ?? "Overig",
            Omschrijving = null,
            Actief = true
        },
        ReactietermijnInDagen = source.ReactietermijnInDagen ?? 42,
        PublicatieIndicatie = source.PublicatieIndicatie ?? false,
        Publicatietekst = null,
        PublicatietermijnInDagen = source.PublicatietermijnInDagen
    };

    #endregion

    #region Documenttype Mapping

    private static ZaaktypeDocumenttype MapZaaktypeDocumenttype(this Source.Documenttype source) => new()
    {
        Documenttype = source.MapDocumenttype(),
        Dspcode = null,
        Titels = null
    };

    private static Documenttype MapDocumenttype(this Source.Documenttype source) => new()
    {
        Naam = source.Naam,
        Omschrijving = source.Omschrijving,
        Actief = true,
        Documentcategorie = source.Documentcategorie,
        Publicatieniveau = source.Publicatieniveau.MapDocumentPublicatieniveau()
    };

    private static DocumentPublicatieniveau MapDocumentPublicatieniveau(this Source.Publicatieniveau? source) => source switch
    {
        Source.Publicatieniveau.Extern => DocumentPublicatieniveau.Extern,
        Source.Publicatieniveau.Intern => DocumentPublicatieniveau.Intern,
        Source.Publicatieniveau.Vertrouwelijk => DocumentPublicatieniveau.Vertrouwelijk,
        _ => DocumentPublicatieniveau.Intern
    };

    #endregion

    #region GerelateerdeZaaktype Mapping

    private static ZaaktypeOverzicht MapZaaktypeOverzicht(this Source.GerelateerdeZaaktype source) => new()
    {
        Naam = source.ZaaktypeIdentificatie ?? "Onbekend",
        Omschrijving = source.Toelichting,
        Actief = true,
        FunctioneleIdentificatie = source.ZaaktypeIdentificatie ?? "ONBEKEND"
    };

    #endregion
}
