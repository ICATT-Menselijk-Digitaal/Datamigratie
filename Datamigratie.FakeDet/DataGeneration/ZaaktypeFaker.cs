using System.Text.RegularExpressions;
using Bogus;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.FakeDet.DataGeneration;

public record ZaaktypeSeed(string Prefix, int Nr, string Naam, string[] Templates, string Categorie)
{
    public string Code => $"{Prefix}-{Nr:000}";
}

public record ZaakSeed(string ZaaktypeCode, string ZaaktypeNaam, string Omschrijving);

public static class ZgwSeed
{
    public static void Init(int seed = 42) => Randomizer.Seed = new Random(seed);

    public static readonly ZaaktypeSeed[] Zaaktypen = Build100Zaaktypen();

    public static void AssertCount100()
    {
        if (Zaaktypen.Length != 100)
            throw new InvalidOperationException($"Expected 100 zaaktypen, got {Zaaktypen.Length}.");
    }

    public static ZaakSeed GenZaak(Faker? faker = null)
    {
        var f = faker ?? new Faker("nl");
        var chosen = f.PickRandom(Zaaktypen);
        var template = f.PickRandom(chosen.Templates);
        var omschrijving = Render(f, template);
        return new ZaakSeed(chosen.Code, chosen.Naam, omschrijving);
    }

    public static string Render(Faker f, string template)
    {
        string Pick(params string[] xs) => f.PickRandom(xs);

        var adres = $"{f.Address.StreetAddress()}, {f.Address.City()}";
        var straat = f.Address.StreetName();
        var plaats = f.Address.City();
        var wijk = Pick("Centrum", "Noord", "Oost", "Zuid", "West", "Buitengebied", "Binnenstad");
        var datumSoon = f.Date.Soon(180).ToString("dd-MM-yyyy");
        var datumPast = f.Date.Past(1).ToString("dd-MM-yyyy");
        var kenmerk = $"ZK-{f.Random.ReplaceNumbers("######")}";
        var dossier = $"Dossier {f.Random.ReplaceNumbers("####-####")}";

        var objectBouw = Pick("dakkapel", "uitbouw", "aanbouw", "schuur", "carport", "schutting", "zonnepanelen", "warmtepomp");
        var actieBouw = Pick("plaatsen", "verbouwen", "uitbreiden", "slopen", "legaliseren");
        var gebouw = Pick("woning", "bedrijfspand", "bijgebouw", "monumentaal pand");
        var evenement = Pick("braderie", "buurtfestival", "hardloopwedstrijd", "foodtruckfestival", "kermis", "muziekevenement");

        var loc = $"{Pick("marktplein", "stadspark", "sportcomplex", "dorpskern", "gemeentehuis", "station")} te {plaats}";
        var persoon = $"{f.Name.FirstName()} {f.Name.LastName()}";
        var branche = Pick("horeca", "detailhandel", "bouw", "transport", "evenementenorganisatie");
        var hinder = Pick("geluidsoverlast", "stankoverlast", "parkeeroverlast", "rookoverlast", "zwerfafval");
        var onderwerp = Pick(
            $"verkeersmaatregel {straat}",
            $"herinrichting wijk {wijk} te {plaats}",
            $"vergunningverlening in {wijk}",
            $"handhaving {branche} in {wijk}"
        );

        var onderwerpWoo = Pick(
            $"verkeersbesluit {straat}",
            $"handhaving horeca in wijk {wijk}",
            $"vergunningverlening in {wijk}",
            $"project {Pick("Stationsomgeving", "Groene Wijk", "Dijkversterking", "Woonvisie", "Mobiliteitsplan")}"
        );

        var dict = new Dictionary<string, string>
        {
            ["{adres}"] = adres,
            ["{straat}"] = straat,
            ["{plaats}"] = plaats,
            ["{wijk}"] = wijk,
            ["{datum}"] = datumSoon,
            ["{datum_besluit}"] = datumPast,
            ["{kenmerk}"] = kenmerk,
            ["{dossier}"] = dossier,
            ["{object_bouw}"] = objectBouw,
            ["{actie_bouw}"] = actieBouw,
            ["{gebouw}"] = gebouw,
            ["{evenement}"] = evenement,
            ["{locatie}"] = loc,
            ["{persoon}"] = persoon,
            ["{branche}"] = branche,
            ["{hinder}"] = hinder,
            ["{onderwerp}"] = onderwerp,
            ["{onderwerp_woo}"] = onderwerpWoo,
            ["{periode}"] = $"{f.Date.Past(3):yyyy}–{DateTime.Now:yyyy}",
            ["{kanaal}"] = Pick("telefonisch", "e-mail", "webformulier", "balie"),
            ["{kade}"] = Pick("Dorpskade", "Havenkade", "Westkade", "Oostkade"),
            ["{activiteit}"] = Pick("sporttoernooi", "cultureel evenement", "wijkinitiatief", "energiebesparingsactie", "educatieproject"),
            ["{maatregel}"] = Pick("isolatie", "HR++ glas", "warmtepomp", "zonnepanelen", "ventilatievoorziening"),
            ["{kosten}"] = Pick("woonlasten", "medische kosten", "reiskosten", "inrichtingskosten", "noodzakelijke kleding"),
            ["{schooljaar}"] = $"{DateTime.Now.Year}/{DateTime.Now.Year + 1}",
            ["{bedrag}"] = $"€ {f.Random.Number(250, 25000):N0}",
            ["{termijn}"] = Pick("6 weken", "8 weken", "12 weken", "verlengd met 6 weken")
        };

        foreach (var (k, v) in dict)
            template = template.Replace(k, v);

        return Regex.Replace(template, @"\s{2,}", " ").Trim();
    }

    private static ZaaktypeSeed[] Build100Zaaktypen()
    {
        var list = new List<ZaaktypeSeed>();

        void Add(string prefix, int nr, string naam, string categorie, params string[] templates)
            => list.Add(new ZaaktypeSeed(prefix, nr, naam, templates, categorie));

        // 1) Vergunningen (25)
        Add("VRG", 1, "Aanvraag omgevingsvergunning (bouwen)", "Vergunning",
            "Aanvraag omgevingsvergunning voor het {actie_bouw} van {object_bouw} aan {adres}.",
            "Aanvraag omgevingsvergunning voor werkzaamheden aan {gebouw} op {adres} (kenmerk {kenmerk}).");
        Add("VRG", 2, "Melding bouwactiviteit / informatieplicht", "Vergunning",
            "Melding bouwactiviteit met beoogde startdatum {datum} voor {adres}.",
            "Informatieplicht bouwactiviteit: werkzaamheden bij {adres}.");
        Add("VRG", 3, "Omgevingsvergunning (monument)", "Vergunning",
            "Aanvraag omgevingsvergunning voor werkzaamheden aan monumentaal pand op {adres}.",
            "Werkzaamheden aan monument: aanvraag incl. bijlagen (dossier {dossier}).");
        Add("VRG", 4, "Omgevingsvergunning (kappen houtopstand)", "Vergunning",
            "Aanvraag kapvergunning voor houtopstand bij {adres}.",
            "Aanvraag verwijderen bomen i.v.m. werkzaamheden op {adres}.");
        Add("VRG", 5, "Sloopmelding", "Vergunning",
            "Sloopmelding voor {gebouw} op {adres}, incl. asbestinventarisatie indien van toepassing.",
            "Melding sloop en afvoer materialen locatie {adres}.");
        Add("VRG", 6, "Asbestmelding", "Vergunning",
            "Melding verwijderen asbesthoudend materiaal op {adres}.",
            "Asbestverwijdering gemeld; locatie {adres}, planning {datum}.");
        Add("VRG", 7, "Milieumelding (Activiteitenbesluit)", "Vergunning",
            "Milieumelding voor bedrijfsactiviteit ({branche}) op locatie {adres}.",
            "Melding milieubelastende activiteit bij {adres}.");
        Add("VRG", 8, "Evenementenvergunning", "Vergunning",
            "Aanvraag evenementenvergunning voor {evenement} op {datum} te {plaats}.",
            "Aanvraag {evenement} inclusief geluidsontheffing en verkeersmaatregelen op {datum}.");
        Add("VRG", 9, "Geluidsontheffing", "Vergunning",
            "Aanvraag geluidsontheffing t.b.v. {evenement} op {datum} te {plaats}.",
            "Verzoek tijdelijke afwijking geluidsnormen in {wijk} op {datum}.");
        Add("VRG", 10, "Terrasvergunning", "Vergunning",
            "Aanvraag terrasvergunning voor locatie {adres}.",
            "Wijzigingsverzoek terrasindeling/oppervlakte bij {adres}.");
        Add("VRG", 11, "Exploitatievergunning (horeca)", "Vergunning",
            "Aanvraag exploitatievergunning voor inrichting op {adres}.",
            "Wijziging exploitatievergunning i.v.m. nieuwe exploitant (locatie {adres}).");
        Add("VRG", 12, "Alcoholwetvergunning (horeca)", "Vergunning",
            "Aanvraag Alcoholwetvergunning voor horecagelegenheid op {adres}.",
            "Wijziging leidinggevenden op Alcoholwetvergunning (locatie {adres}).");
        Add("VRG", 13, "Standplaatsvergunning", "Vergunning",
            "Aanvraag standplaatsvergunning voor {branche} op {locatie}.",
            "Verzoek standplaats in {wijk} te {plaats}.");
        Add("VRG", 14, "Marktvergunning", "Vergunning",
            "Aanvraag marktvergunning voor deelname aan markt in {plaats}.",
            "Wijziging/overname marktvergunning (kenmerk {kenmerk}).");
        Add("VRG", 15, "Parkeervergunning bewoners", "Vergunning",
            "Aanvraag bewonersparkeervergunning voor {adres}.",
            "Wijziging kenteken op bewonersparkeervergunning (adres {adres}).");
        Add("VRG", 16, "Gehandicaptenparkeerplaats", "Vergunning",
            "Aanvraag gehandicaptenparkeerplaats t.h.v. {adres}.",
            "Beoordeling aanvraag gehandicaptenparkeerplaats (wijk {wijk}).");
        Add("VRG", 17, "Vergunning inrit/uitweg", "Vergunning",
            "Aanvraag vergunning voor aanleg/wijziging inrit bij {adres}.",
            "Verzoek uitwegvergunning i.v.m. herinrichting {adres}.");
        Add("VRG", 18, "Venten / ventvergunning (ontheffing)", "Vergunning",
            "Aanvraag/ontheffing venten in {wijk} te {plaats}.",
            "Beoordeling aanvraag venten; locatie {plaats}, datum {datum}.");
        Add("VRG", 19, "Reclamevergunning / uitstalling", "Vergunning",
            "Aanvraag vergunning voor reclame-uiting of uitstalling bij {adres}.",
            "Verzoek plaatsing reclame-object aan gevel {adres}.");
        Add("VRG", 20, "Plaatsing container / bouwplaatsinrichting", "Vergunning",
            "Melding/aanvraag plaatsing container/bouwplaatsinrichting bij {adres} (datum {datum}).",
            "Aanvraag innemen openbare ruimte t.b.v. bouw bij {adres}.");
        Add("VRG", 21, "APV-ontheffing (algemeen)", "Vergunning",
            "Aanvraag ontheffing op grond van APV voor activiteit bij {locatie} op {datum}.",
            "Ontheffing aangevraagd voor gebruik openbare ruimte ({locatie}).");
        Add("VRG", 22, "Ligplaatsvergunning", "Vergunning",
            "Aanvraag ligplaatsvergunning aan {kade} te {plaats}.",
            "Wijzigingsverzoek ligplaatsvergunning (kenmerk {kenmerk}).");
        Add("VRG", 23, "Ontheffing sluitingstijden", "Vergunning",
            "Aanvraag ontheffing sluitingstijden voor inrichting op {adres} op {datum}.",
            "Verzoek verruiming sluitingstijden in {wijk} (datum {datum}).");
        Add("VRG", 24, "Tijdelijke verhuur woonruimte", "Vergunning",
            "Aanvraag vergunning tijdelijke verhuur woonruimte op {adres}.",
            "Beoordeling tijdelijke verhuur; locatie {adres}, termijn {termijn}.");
        Add("VRG", 25, "Reisdocument / spoedprocedure", "Vergunning",
            "Spoedaanvraag reisdocument voor {persoon} op {datum}.",
            "Aanvraag nooddocument; reden reis, afspraak gepland.");

        // 2) Toezicht & Handhaving (15) => 40
        Add("HVH", 1, "Handhavingsverzoek (bouwen)", "Toezicht & Handhaving",
            "Verzoek om handhaving inzake (vermoedelijke) bouwovertreding bij {adres}.",
            "Melding illegale bouw/verbouw bij {adres}; onderzoek en opvolging.");
        Add("HVH", 2, "Handhavingsverzoek (milieu)", "Toezicht & Handhaving",
            "Verzoek om handhaving inzake milieuhinder ({hinder}) bij {adres}.",
            "Melding overlast door bedrijfsactiviteit ({branche}) nabij {adres}.");
        Add("MEL", 1, "Melding overlast / leefbaarheid", "Toezicht & Handhaving",
            "Melding overlast ({hinder}) in {wijk} te {plaats}.",
            "Melding leefbaarheidsissue in {wijk}; beoordeling en doorzet.");
        Add("TOZ", 1, "Toezichtsrapportage bouw", "Toezicht & Handhaving",
            "Toezichtrapportage bouwproject bij {adres} op {datum}.",
            "Controle bouwactiviteit bij {adres}; verslag en bevindingen.");
        Add("TOZ", 2, "Toezichtsrapportage horeca", "Toezicht & Handhaving",
            "Controleverslag horeca-inrichting op {adres} (datum {datum}).",
            "Inspectie naleving voorschriften bij {adres}; rapportage opgesteld.");
        Add("HVH", 3, "Vooraankondiging handhaving", "Toezicht & Handhaving",
            "Vooraankondiging handhavend optreden inzake constatering bij {adres}.",
            "Aanschrijven i.v.m. overtreding; hersteltermijn aangekondigd.");
        Add("HVH", 4, "Last onder dwangsom", "Toezicht & Handhaving",
            "Voorbereiding besluit last onder dwangsom naar aanleiding van constatering bij {adres}.",
            "Conceptbesluit dwangsom; overtreding en hersteltermijn vastgelegd.");
        Add("HVH", 5, "Last onder bestuursdwang", "Toezicht & Handhaving",
            "Voorbereiding besluit bestuursdwang inzake situatie bij {adres}.",
            "Bestuursdwangtraject gestart; dossier {dossier}.");
        Add("TOZ", 3, "Bestuurlijke rapportage", "Toezicht & Handhaving",
            "Bestuurlijke rapportage naar aanleiding van toezicht/controle bij {adres}.",
            "Rapportage opgesteld ten behoeve van besluitvorming (kenmerk {kenmerk}).");
        Add("MEL", 2, "Melding illegale afvaldumping", "Toezicht & Handhaving",
            "Melding illegale afvaldumping nabij {locatie}.",
            "Afvaldump aangetroffen; afhandeling en eventuele herleidbaarheid onderzoekt.");
        Add("MEL", 3, "Melding zwerfafval", "Toezicht & Handhaving",
            "Melding zwerfafval in {wijk} te {plaats}.",
            "Signalering vervuiling openbare ruimte; inzet reiniging gepland.");
        Add("TOZ", 4, "Controle brandveilig gebruik", "Toezicht & Handhaving",
            "Controle brandveilig gebruik bij {gebouw} op {adres}.",
            "Brandveiligheidsinspectie uitgevoerd; herstelpunten vastgelegd.");
        Add("TOZ", 5, "Vergunningcontrole evenement", "Toezicht & Handhaving",
            "Controle naleving vergunningvoorschriften tijdens {evenement} op {datum}.",
            "Toezicht tijdens {evenement}; verslag opgesteld.");
        Add("TOZ", 6, "Toezicht openbare ruimte", "Toezicht & Handhaving",
            "Controle/inspectie openbare ruimte op locatie {locatie}.",
            "Inspectie op locatie {locatie}; vervolgactie bepaald.");
        Add("SIG", 1, "Signaal ondermijning", "Toezicht & Handhaving",
            "Registratie en opvolging signaal mogelijke ondermijning ({branche}) in {wijk}.",
            "Signalering ondermijning; interne afstemming en vervolgonderzoek.");

        // 3) Subsidies (12) => 52
        Add("SUB", 1, "Subsidieaanvraag verduurzaming woning", "Subsidie",
            "Subsidieaanvraag verduurzaming voor {adres} (indicatief {bedrag}).",
            "Aanvraag subsidie voor {maatregel} bij {adres}.");
        Add("SUB", 2, "Subsidieaanvraag sport/cultuur", "Subsidie",
            "Subsidieaanvraag voor activiteit {activiteit} in {plaats} (bedrag {bedrag}).",
            "Aanvraag projectsubsidie {activiteit} (dossier {dossier}).");
        Add("SUB", 3, "Subsidieaanvraag maatschappelijke initiatieven", "Subsidie",
            "Aanvraag subsidie maatschappelijke initiatieven in {wijk} (bedrag {bedrag}).",
            "Subsidieaanvraag voor wijkinitiatief in {wijk}, {plaats}.");
        Add("SUB", 4, "Beschikking subsidieverlening", "Subsidie",
            "Opstellen beschikking subsidieverlening (bedrag {bedrag}) voor {activiteit}.",
            "Beschikking verlening subsidie; voorwaarden en termijn {termijn}.");
        Add("SUB", 5, "Subsidievaststelling", "Subsidie",
            "Vaststelling subsidie na verantwoording (dossier {dossier}).",
            "Controle verantwoording en vaststellingsbesluit opgesteld.");
        Add("SUB", 6, "Subsidiewijziging", "Subsidie",
            "Wijzigingsverzoek subsidievoorwaarden/budget (dossier {dossier}).",
            "Aanpassing beschikking i.v.m. gewijzigde planning/begroting.");
        Add("SUB", 7, "Terugvordering subsidie", "Subsidie",
            "Onderzoek en besluitvorming terugvordering subsidie (dossier {dossier}).",
            "Voornemen terugvordering; hoor/wederhoor toegepast.");
        Add("SUB", 8, "Stimuleringslening aanvraag", "Subsidie",
            "Aanvraag stimuleringslening voor {maatregel} op {adres}.",
            "Beoordeling financieringsaanvraag; kredietcheck en besluitvorming.");
        Add("SUB", 9, "Declaratie subsidie", "Subsidie",
            "Indiening declaratie en controle kosten t.b.v. subsidie (dossier {dossier}).",
            "Controle facturen en betaalbewijzen; vastlegging in dossier.");
        Add("SUB", 10, "Aanvraag energietoeslag", "Subsidie",
            "Aanvraag energietoeslag (betrokkene: {persoon}, peildatum {datum}).",
            "Beoordeling energietoeslag conform beleidsregels.");
        Add("SUB", 11, "Aanvraag minimaregeling", "Subsidie",
            "Aanvraag minimaregeling/bijdrage (betrokkene: {persoon}).",
            "Toetsing inkomen/vermogen en besluitvorming.");
        Add("SUB", 12, "Bezwaar subsidiebesluit", "Subsidie",
            "Bezwaar tegen subsidiebesluit d.d. {datum_besluit} (kenmerk {kenmerk}).",
            "Behandeling bezwaar subsidie incl. dossieropbouw.");

        // 4) Juridisch (12) => 64
        Add("BZW", 1, "Bezwaar tegen besluit", "Juridisch",
            "Bezwaarschrift tegen besluit d.d. {datum_besluit} inzake {onderwerp}.",
            "Bezwaar ontvangen (kenmerk {kenmerk}); ontvangstbevestiging verzonden.");
        Add("BZW", 2, "Hoorzitting bezwaar", "Juridisch",
            "Planning en verslag hoorzitting bezwaar (kenmerk {kenmerk}).",
            "Hoorzitting ingepland; stukken opgevraagd en gedeeld.");
        Add("BRP", 1, "Beroep rechtbank (bestuursrecht)", "Juridisch",
            "Dossieropbouw en verweerschrift inzake beroep (kenmerk {kenmerk}).",
            "Procesdossier samengesteld en aan rechtbank verzonden.");
        Add("JUR", 1, "Voorlopige voorziening", "Juridisch",
            "Behandeling verzoek voorlopige voorziening inzake {onderwerp} (kenmerk {kenmerk}).",
            "Spoedprocedure; standpunt opgesteld en ingediend.");
        Add("KLT", 1, "Klachtbehandeling", "Juridisch",
            "Behandeling klacht conform klachtenregeling (dossier {dossier}).",
            "Onderzoek klacht, hoor/wederhoor en afdoeningsbrief.");
        Add("JUR", 2, "Schadeclaim / aansprakelijkheid", "Juridisch",
            "Behandeling aansprakelijkstelling/schadeclaim inzake {locatie} (kenmerk {kenmerk}).",
            "Inventarisatie feiten, polisinfo en conceptreactie opgesteld.");
        Add("JUR", 3, "Verzoek nadeelcompensatie", "Juridisch",
            "Behandeling verzoek nadeelcompensatie i.v.m. werkzaamheden in {wijk}.",
            "Beoordeling schade en causaliteit; advies gevraagd.");
        Add("BZW", 3, "Bezwaar handhavingsbesluit", "Juridisch",
            "Bezwaar tegen handhavingsbesluit d.d. {datum_besluit} (locatie {adres}).",
            "Opstellen verweerschrift / heroverweging handhavingsbesluit.");
        Add("BZW", 4, "Bezwaar vergunningbesluit", "Juridisch",
            "Bezwaar tegen vergunningbesluit d.d. {datum_besluit} (locatie {adres}).",
            "Heroverweging vergunning; aanvullende stukken opgevraagd.");
        Add("JUR", 4, "Ingebrekestelling / dwangsom", "Juridisch",
            "Ingebrekestelling wegens niet tijdig beslissen (kenmerk {kenmerk}).",
            "Termijnbewaking en besluitvorming dwangsom.");
        Add("JUR", 5, "Verzoek voorlopige voorziening (handhaving)", "Juridisch",
            "Verzoek voorlopige voorziening m.b.t. handhaving (kenmerk {kenmerk}).",
            "Spoedbehandeling; stukken gereedgemaakt voor zitting.");
        Add("JUR", 6, "Minnelijke regeling / schikking", "Juridisch",
            "Onderhandeling minnelijke regeling/schikking inzake {onderwerp} (dossier {dossier}).",
            "Afstemming met betrokkenen en vastlegging afspraken.");

        // 5) Woo / Informatie (10) => 74
        Add("WOO", 1, "Woo-verzoek informatieverstrekking", "Informatie (Woo)",
            "Woo-verzoek inzake openbaarmaking van documenten over {onderwerp_woo} (periode {periode}).",
            "Woo-verzoek ontvangen; inventarisatie gestart (kenmerk {kenmerk}).");
        Add("WOO", 2, "Inventarisatie Woo-dossier", "Informatie (Woo)",
            "Inventarisatie documenten t.b.v. Woo-verzoek over {onderwerp_woo}.",
            "Zoekslag uitgezet bij teams; resultaten samengevoegd.");
        Add("WOO", 3, "Derden-zienswijze Woo", "Informatie (Woo)",
            "Verzoek om zienswijze aan derden in kader van Woo (dossier {dossier}).",
            "Zienswijzetermijn uitgezet; verwerking reacties.");
        Add("WOO", 4, "Deelbesluit Woo", "Informatie (Woo)",
            "Deelbesluit Woo met inventarisatielijst en weigeringsgronden (kenmerk {kenmerk}).",
            "Conceptdeelbesluit opgesteld; interne afstemming.");
        Add("WOO", 5, "Publicatie Woo-besluit", "Informatie (Woo)",
            "Voorbereiding publicatie Woo-besluit en geanonimiseerde stukken (kenmerk {kenmerk}).",
            "Publicatiepakket samengesteld; controle anonimisering.");
        Add("INF", 1, "Informatieverzoek (algemeen)", "Informatie (Woo)",
            "Verzoek om informatie over {onderwerp} ontvangen via {kanaal}.",
            "Informatievraag geregistreerd en doorgezet.");
        Add("INF", 2, "Inzageverzoek dossier", "Informatie (Woo)",
            "Verzoek om inzage in dossier {dossier} (betrokkene: {persoon}).",
            "Inzageprocedure opgestart; afspraak/tijdvak bepaald.");
        Add("AVG", 1, "AVG-verzoek (inzage)", "Informatie (Woo)",
            "Verzoek op grond van AVG: inzage persoonsgegevens (betrokkene: {persoon}).",
            "Identiteitscheck en verwerking inzageverzoek.");
        Add("AVG", 2, "AVG-verzoek (correctie/verwijdering)", "Informatie (Woo)",
            "Verzoek op grond van AVG: correctie/verwijdering persoonsgegevens (betrokkene: {persoon}).",
            "Beoordeling verzoek en terugkoppeling aan betrokkene.");
        Add("ARC", 1, "Archiefonderzoek / informatievraag", "Informatie (Woo)",
            "Archiefonderzoek naar stukken over {onderwerp} (periode {periode}).",
            "Zoekactie in archief; resultaten vastgelegd.");

        // 6) Burgerzaken (10) => 84
        Add("BRP", 2, "Aanvraag uittreksel BRP", "Burgerzaken",
            "Aanvraag uittreksel BRP (betrokkene: {persoon}) op {datum}.",
            "Verzoek uittreksel; afgifte en registratie.");
        Add("BRP", 3, "Adresonderzoek", "Burgerzaken",
            "Adresonderzoek gestart naar aanleiding van signaal (betrokkene: {persoon}).",
            "Onderzoek verblijfplaats; brieven en registraties gecontroleerd.");
        Add("BRP", 4, "Registratie verhuizing", "Burgerzaken",
            "Verwerking verhuizing naar {adres} (betrokkene: {persoon}).",
            "Inschrijving/adresmutatie verwerkt in BRP.");
        Add("BRZ", 1, "Aanvraag reisdocument", "Burgerzaken",
            "Aanvraag reisdocument (paspoort/ID) voor {persoon} op {datum}.",
            "Afspraak balie; aanvraag opgenomen en productie gestart.");
        Add("BRZ", 2, "Aanvraag rijbewijs", "Burgerzaken",
            "Aanvraag rijbewijs (eerste aanvraag/verlenging) voor {persoon}.",
            "Behandeling rijbewijsaanvraag; afgifte ingepland.");
        Add("BRZ", 3, "Geboorteaangifte", "Burgerzaken",
            "Verwerking geboorteaangifte op {datum} (dossier {dossier}).",
            "Akte opgemaakt en registratie afgerond.");
        Add("BRZ", 4, "Huwelijk/partnerschap", "Burgerzaken",
            "Melding voorgenomen huwelijk/partnerschap op {datum} te {plaats}.",
            "Dossier voorbereiding huwelijk/partnerschap; stukken gecontroleerd.");
        Add("BRZ", 5, "Overlijdensaangifte", "Burgerzaken",
            "Verwerking overlijdensaangifte (dossier {dossier}).",
            "Akte opgesteld; betrokken registraties bijgewerkt.");
        Add("BRP", 5, "Correctie BRP persoonsgegevens", "Burgerzaken",
            "Verzoek correctie persoonsgegevens in BRP (betrokkene: {persoon}).",
            "Beoordeling bewijsstukken en correctiebesluit.");
        Add("BRZ", 6, "Aanvraag verklaring omtrent gedrag (VOG)", "Burgerzaken",
            "Aanvraag/doorgeleiding VOG-procedure (betrokkene: {persoon}).",
            "Ondersteuning aanvraag VOG; doorverwijzing correct verwerkt.");

        // 7) Sociaal domein (8) => 92
        Add("WMO", 1, "Wmo-melding / aanvraag", "Sociaal domein",
            "Melding/aanvraag Wmo-ondersteuning (betrokkene: {persoon}).",
            "Registratie melding en start triage.");
        Add("WMO", 2, "Wmo-onderzoek (keukentafel)", "Sociaal domein",
            "Uitvoering Wmo-onderzoek en verslaglegging (betrokkene: {persoon}).",
            "Plan van aanpak opgesteld; beoordeling voorziening.");
        Add("JGD", 1, "Jeugdhulp aanvraag", "Sociaal domein",
            "Aanvraag jeugdhulp en triage (betrokkene: {persoon}).",
            "Casus aangemeld; toewijzing traject.");
        Add("PWT", 1, "Participatiewet intake", "Sociaal domein",
            "Intake bijstandsaanvraag Participatiewet (betrokkene: {persoon}).",
            "Controle gegevens en start beoordeling.");
        Add("PWT", 2, "Bijzondere bijstand", "Sociaal domein",
            "Aanvraag bijzondere bijstand (betrokkene: {persoon}, kostenpost {kosten}).",
            "Toetsing rechtmatigheid en besluitvorming.");
        Add("SHV", 1, "Schuldhulpverlening", "Sociaal domein",
            "Aanmelding schuldhulpverlening en plan van aanpak (betrokkene: {persoon}).",
            "Inventarisatie schulden; traject gestart.");
        Add("EDU", 1, "Leerlingenvervoer", "Sociaal domein",
            "Aanvraag leerlingenvervoer voor {persoon} (schooljaar {schooljaar}).",
            "Beoordeling afstand/criteria; beschikking opgesteld.");
        Add("INB", 1, "Inburgering / begeleiding", "Sociaal domein",
            "Aanmelding inburgeringstraject (betrokkene: {persoon}).",
            "Intake en trajectplan opgesteld.");

        // 8) Openbare ruimte (8) => 100
        Add("BOR", 1, "Melding straatverlichting", "Openbare ruimte",
            "Melding defecte straatverlichting in {straat} te {plaats}.",
            "Melding lampstoring; opdracht herstel uitgezet.");
        Add("BOR", 2, "Melding wegdek/schade", "Openbare ruimte",
            "Melding schade aan wegdek/stoep bij {locatie}.",
            "Schade gemeld; inspectie ingepland.");
        Add("BOR", 3, "Melding groenbeheer", "Openbare ruimte",
            "Melding over groenbeheer (snoei/omgevallen boom) nabij {locatie}.",
            "Werkorder groenbeheer; beoordeling urgentie.");
        Add("VRK", 1, "Verkeersbesluit", "Openbare ruimte",
            "Voorbereiding verkeersbesluit voor {straat} te {plaats} (kenmerk {kenmerk}).",
            "Afstemming verkeersmaatregel; ontwerpbesluit opgesteld.");
        Add("VRK", 2, "Wegafsluiting / omleiding", "Openbare ruimte",
            "Aanvraag/melding tijdelijke wegafsluiting bij {locatie} op {datum}.",
            "Omleidingsplan beoordeeld en afgestemd met beheer.");
        Add("CIV", 1, "Graafmelding / KLIC-afstemming", "Openbare ruimte",
            "Afstemming graafwerkzaamheden (KLIC) bij {locatie} te {plaats}.",
            "Vergunning/instemming civiele werkzaamheden; afspraken vastgelegd.");
        Add("BOR", 4, "Speeltoestel inspectie", "Openbare ruimte",
            "Inspectie en afhandeling melding speeltoestel op {locatie}.",
            "Veiligheidscheck speelplek; herstelactie bepaald.");
        Add("BOR", 5, "Afvalcontainer (plaatsing/wijziging)", "Openbare ruimte",
            "Melding/verzoek plaatsing of wijziging afvalcontainer nabij {locatie}.",
            "Locatieonderzoek uitgevoerd; besluit terugkoppeling.");

        return list.ToArray();
    }
}

public static class DetZaaktypeFactory
{
    public static Faker<DetZaaktype> Faker(int seed = 42)
    {
        ZgwSeed.Init(seed);
        ZgwSeed.AssertCount100();

        string ToSlug(string s)
        {
            s = s.ToLowerInvariant();
            s = Regex.Replace(s, @"[^a-z0-9]+", "-");
            return s.Trim('-');
        }

        return new Faker<DetZaaktype>("nl")
            .CustomInstantiator(_ => new DetZaaktype
            {
                Actief = true,
                Naam = "TEMP",
                Omschrijving = "TEMP",
                FunctioneleIdentificatie = "TEMP"
            })
            .FinishWith((f, zt) =>
            {
                var seedItem = f.PickRandom(ZgwSeed.Zaaktypen);

                zt.Actief = f.Random.Bool(0.85f);
                zt.Naam = seedItem.Naam;
                zt.Omschrijving = ZgwSeed.Render(f, f.PickRandom(seedItem.Templates));

                // FunctioneleIdentificatie: leesbaar + uniek genoeg in testdata
                // Voorbeeld: VRG-0001-aanvraag-omgevingsvergunning-bouwen
                var seq = f.Random.Number(1, 9999);
                zt.FunctioneleIdentificatie = $"{seedItem.Prefix}-{seq:0000}-{ToSlug(seedItem.Naam)}";
            });
    }
}
