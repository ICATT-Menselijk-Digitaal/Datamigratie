using System.Text.RegularExpressions;
using Bogus;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.FakeDet.DataGeneration;

public static class DetZaakFakers
{
    // --- kleine woordenlijsten voor realistische NL bestandsnamen/titels
    private static readonly string[] DocTypes =
    [
        "Aanvraag", "Besluit", "Bijlage", "Rapport", "Inspectieverslag", "Foto", "Tekening", "Mail", "Notitie", "Factuur"
    ];

    private static readonly (string ext, string mime)[] MimePool =
    [
        (".pdf",  "application/pdf"),
        (".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
        (".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
        (".jpg",  "image/jpeg"),
        (".png",  "image/png"),
        (".txt",  "text/plain")
    ];

    private static readonly string[] Afdelingen =
    [
        "Vergunningen", "Toezicht & Handhaving", "Juridische Zaken", "Burgerzaken", "Ruimte & Wonen",
        "Openbare Ruimte", "Sociaal Domein", "Financiën", "Publiekszaken", "Informatiebeheer"
    ];

    private static readonly string[] Betaalstatussen =
    [
        "Open", "In behandeling", "Betaald", "Geannuleerd", "Terugbetaald"
    ];

    public static Faker<DetBetaalgegevens> BetaalgegevensFaker() =>
        new Faker<DetBetaalgegevens>("nl")
            .RuleFor(x => x.Bedrag, f => Math.Round(f.Random.Decimal(15m, 1250m), 2))
            .RuleFor(x => x.Betaalstatus, f => f.PickRandom(Betaalstatussen))
            .RuleFor(x => x.Kenmerk, f => $"BET-{f.Random.ReplaceNumbers("########")}");

    public static Faker<DetDocumentVersie> DocumentVersieFaker(DateOnly minDate, DateOnly maxDate, int versienr) =>
        new Faker<DetDocumentVersie>("nl")
            .RuleFor(v => v.Versienummer, _ => versienr)
            .RuleFor(v => v.DocumentInhoudID, f => f.Random.Long(10_000_000_000, 99_999_999_999))
            .RuleFor(v => v.Mimetype, f => f.PickRandom(MimePool).mime)
            .RuleFor(v => v.Bestandsnaam, (f, v) =>
            {
                // match ext bij gekozen mimetype
                var ext = MimePool.First(p => p.mime == v.Mimetype).ext;
                var baseName = $"{f.Date.Past(1):yyyyMMdd}_{f.Random.ReplaceNumbers("####")}";
                return $"{baseName}{ext}";
            })
            .RuleFor(v => v.Documentgrootte, (f, _) => f.Random.Bool(0.75f) ? f.Random.Long(5_000, 8_000_000) : null)
            .RuleFor(v => v.Creatiedatum, f => DateOnly.FromDateTime(f.Date.Between(
                minDate.ToDateTime(TimeOnly.MinValue),
                maxDate.ToDateTime(TimeOnly.MinValue))));

    public static Faker<DetDocument> DocumentFaker(DateOnly minDate, DateOnly maxDate) =>
        new Faker<DetDocument>("nl")
            .RuleFor(d => d.Kenmerk, f => f.Random.Bool(0.6f) ? $"DOC-{f.Random.ReplaceNumbers("######")}" : null)
            .RuleFor(d => d.Titel, f =>
            {
                var dt = f.PickRandom(DocTypes);
                return dt switch
                {
                    "Aanvraag" => $"Aanvraag {f.Random.Word()}",
                    "Besluit" => $"Besluit {f.Random.Word()}",
                    "Bijlage" => $"Bijlage {f.Random.Word()}",
                    "Rapport" => $"Rapportage {f.Random.Word()}",
                    "Inspectieverslag" => $"Inspectieverslag {f.Date.Past(1):MMMM yyyy}",
                    "Foto" => $"Foto's locatie {f.Address.City()}",
                    "Tekening" => $"Tekening {f.Random.AlphaNumeric(6).ToUpper()}",
                    "Mail" => $"E-mailcorrespondentie {f.Company.CompanyName()}",
                    "Notitie" => $"Interne notitie {f.Random.Word()}",
                    "Factuur" => $"Factuur {f.Random.ReplaceNumbers("######")}",
                    _ => $"{dt} {f.Random.Word()}"
                };
            })
            .RuleFor(d => d.DocumentVersies, (f, d) =>
            {
                // 1–4 versies, oplopend
                var n = f.Random.Number(1, 4);
                var versions = new List<DetDocumentVersie>(n);
                for (int i = 1; i <= n; i++)
                    versions.Add(DocumentVersieFaker(minDate, maxDate, i).Generate());
                return versions;
            });

    public static Faker<DetZaak> ZaakFaker(Func<DetZaaktype> zaaktypeFactory, int seed = 42)
    {
        Randomizer.Seed = new Random(seed);

        string Slug(string s)
        {
            s = s.ToLowerInvariant();
            s = Regex.Replace(s, @"[^a-z0-9]+", "-");
            return s.Trim('-');
        }

        bool IsBetaalZaak(DetZaaktype? zt)
        {
            if (zt?.Naam is null) return false;
            var n = zt.Naam.ToLowerInvariant();
            // simpele heuristiek; tweak naar je eigen domein
            return n.Contains("vergunning") || n.Contains("evenement") || n.Contains("alcoholwet")
                   || n.Contains("exploitatie") || n.Contains("standplaats") || n.Contains("markt");
        }

        return new Faker<DetZaak>("nl")
            .CustomInstantiator(_ => new DetZaak
            {
                FunctioneleIdentificatie = "TEMP",
                Open = true,
                CreatieDatumTijd = default,
                WijzigDatumTijd = default,
                Startdatum = default,
                Streefdatum = default,
                Omschrijving = "TEMP",
                Documenten = new List<DetDocument>(),
            })
            .FinishWith((f, z) =>
            {
                // Zaaktype (optioneel in jouw class, maar meestal wil je 'm zetten)
                var zt = zaaktypeFactory();
                z.Zaaktype = zt;

                // Datums (realistisch)
                var start = DateOnly.FromDateTime(f.Date.Past(2));
                var streef = start.AddDays(f.Random.Number(7, 90)); // target
                var isOpen = f.Random.Bool(0.65f);

                DateOnly? eind = null;
                if (!isOpen)
                {
                    // afgesloten: einddatum tussen start en start+180
                    eind = start.AddDays(f.Random.Number(3, 180));
                    // streefdatum vaak vóór of rond eind
                    streef = start.AddDays(f.Random.Number(7, 120));
                    if (streef > eind.Value) streef = eind.Value;
                }

                // Creatie/Wijzig (DateTimeOffset) consistent met Start/Eind
                var startDT = start.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(f.Random.Number(8, 16))));
                var wijzigDT = (eind ?? start.AddDays(f.Random.Number(1, 120)))
                    .ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(f.Random.Number(8, 17))));

                // Zorg dat wijzig >= creatie
                if (wijzigDT < startDT) wijzigDT = startDT.AddHours(f.Random.Number(1, 72));

                z.Startdatum = start;
                z.Streefdatum = streef;
                z.Einddatum = eind;

                z.CreatieDatumTijd = new DateTimeOffset(startDT, TimeSpan.FromHours(1)); // CET-ish (simpel)
                z.WijzigDatumTijd = new DateTimeOffset(wijzigDT, TimeSpan.FromHours(1));

                z.Open = isOpen;
                z.Heropend = !isOpen && f.Random.Bool(0.12f);
                z.Intake = f.Random.Bool(0.25f);
                z.ProcesGestart = f.Random.Bool(0.7f);
                z.Notificeerbaar = f.Random.Bool(0.4f);
                z.Vertrouwelijk = f.Random.Bool(0.08f);
                z.Vernietiging = f.Random.Bool(0.05f);
                z.GeautoriseerdVoorMedewerkers = f.Random.Bool(0.85f);

                z.AangemaaktDoor = f.Random.Bool(0.8f) ? $"{f.Internet.UserName()}@{f.Internet.DomainName()}" : null;
                z.Afdeling = f.Random.Bool(0.85f) ? f.PickRandom(Afdelingen) : null;
                z.ExterneIdentificatie = f.Random.Bool(0.25f) ? $"EXT-{f.Random.ReplaceNumbers("########")}" : null;

                // Fataledatum: soms, typisch vóór streef/eind of als signaal
                z.Fataledatum = f.Random.Bool(0.18f)
                    ? start.AddDays(f.Random.Number(3, 60))
                    : null;

                // Betaalgegevens alleen als logisch
                z.Betaalgegevens = IsBetaalZaak(zt) && f.Random.Bool(0.65f)
                    ? BetaalgegevensFaker().Generate()
                    : null;

                // Omschrijving: combineer zaaktype-omschrijving + concrete casusregel
                // (Assumptie: je gebruikt de seed/render uit mijn eerdere DetZaaktypeFactory,
                // dus zt.Omschrijving is al “ambtelijk”.)
                var casus = f.PickRandom(new[]
                {
                    $"Locatie: {f.Address.StreetAddress()}, {f.Address.City()}.",
                    $"Betreft {f.Random.Word()} in wijk {f.PickRandom("Centrum","Noord","Zuid","Oost","West")}.",
                    $"Kenmerk: ZK-{f.Random.ReplaceNumbers("######")}.",
                    $"Aanvullende stukken volgen; termijnbewaking {f.Random.Number(1, 12)} weken."
                });

                z.Omschrijving =
                    $"{zt.Naam}: {CapFirst(zt.Omschrijving.Trim().TrimEnd('.'))}. {casus}";

                // FunctioneleIdentificatie: koppel aan type + jaartal + volgnummer
                var seq = f.Random.Number(1, 99999);
                var typeSlug = Slug(zt.Naam);
                z.FunctioneleIdentificatie = $"{zt.FunctioneleIdentificatie}-{start:yyyy}-{seq:00000}-{typeSlug}";

                // Documenten: aantal afhankelijk van open/gesloten
                var docCount = isOpen ? f.Random.Number(0, 6) : f.Random.Number(2, 12);
                var docMin = start;
                var docMax = eind ?? DateOnly.FromDateTime(z.WijzigDatumTijd.DateTime);

                z.Documenten = docCount == 0
                    ? new List<DetDocument>()
                    : DocumentFaker(docMin, docMax).Generate(docCount);
            });
    }

    private static string CapFirst(string s)
        => string.IsNullOrWhiteSpace(s) ? s : char.ToUpperInvariant(s[0]) + s[1..];
}
