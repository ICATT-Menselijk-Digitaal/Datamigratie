namespace Datamigratie.FakeDet.DataGeneration.Zaken.Fakers;

public static class DutchDataSets
{
    public static readonly string[] Voornamen =
    [
        "Jan", "Piet", "Klaas", "Willem", "Henk", "Johan", "Peter", "Dirk", "Cornelis", "Johannes",
        "Maria", "Anna", "Elisabeth", "Johanna", "Cornelia", "Wilhelmina", "Hendrika", "Petronella",
        "Daan", "Sem", "Lucas", "Levi", "Finn", "Noah", "Milan", "Jesse", "Luuk", "Thijs",
        "Emma", "Julia", "Sophie", "Anna", "Lisa", "Eva", "Sara", "Lotte", "Fleur", "Isa",
        "Mohammed", "Ahmed", "Ali", "Fatima", "Aisha", "Youssef", "Omar", "Hassan", "Mariam"
    ];

    public static readonly string[] Voorvoegsels =
    [
        "van", "de", "van de", "van der", "van den", "ter", "ten", "in 't", "op de", "van 't"
    ];

    public static readonly string[] Achternamen =
    [
        "Jansen", "De Jong", "De Vries", "Van den Berg", "Van Dijk", "Bakker", "Janssen", "Visser",
        "Smit", "Meijer", "De Boer", "Mulder", "De Groot", "Bos", "Vos", "Peters", "Hendriks",
        "Van Leeuwen", "Dekker", "Brouwer", "De Wit", "Dijkstra", "Smits", "De Graaf", "Van der Meer",
        "Van der Linden", "Kok", "Jacobs", "De Haan", "Vermeer", "Van der Berg", "Van der Heijden",
        "Schouten", "Van Beek", "Willems", "Van Vliet", "Van der Veen", "Hoekstra", "Maas", "Verhoeven"
    ];

    public static readonly string[] Straatnamen =
    [
        "Hoofdstraat", "Kerkstraat", "Schoolstraat", "Molenweg", "Dorpsstraat", "Stationsweg",
        "Nieuwstraat", "Burgemeester de Jongstraat", "Markt", "Raadhuisstraat", "Julianastraat",
        "Beatrixlaan", "Wilhelminalaan", "Oranjelaan", "Koningstraat", "Prinsenstraat",
        "Lindelaan", "Eikenlaan", "Beukenlaan", "Kastanjelaan", "Dennenlaan", "Berkenlaan",
        "Tulpstraat", "Rozenstraat", "Leliestraat", "Zonnebloemstraat", "Hyacintstraat",
        "Van Goghstraat", "Rembrandtlaan", "Vermeerstraat", "Mondriaan Avenue"
    ];

    public static readonly string[] Plaatsnamen =
    [
        "Amsterdam", "Rotterdam", "Den Haag", "Utrecht", "Eindhoven", "Groningen", "Tilburg",
        "Almere", "Breda", "Nijmegen", "Apeldoorn", "Enschede", "Haarlem", "Arnhem", "Zaanstad",
        "Amersfoort", "Haarlemmermeer", "s-Hertogenbosch", "Zoetermeer", "Zwolle", "Maastricht",
        "Leiden", "Dordrecht", "Ede", "Alphen aan den Rijn", "Alkmaar", "Delft", "Emmen",
        "Deventer", "Leeuwarden", "Venlo", "Westland", "Sittard-Geleen", "Helmond", "Hilversum"
    ];

    public static readonly string[] Gemeentenamen =
    [
        "Gemeente Amsterdam", "Gemeente Rotterdam", "Gemeente Den Haag", "Gemeente Utrecht",
        "Gemeente Eindhoven", "Gemeente Groningen", "Gemeente Tilburg", "Gemeente Almere",
        "Gemeente Breda", "Gemeente Nijmegen", "Gemeente Apeldoorn", "Gemeente Haarlem",
        "Gemeente Arnhem", "Gemeente Zaanstad", "Gemeente Amersfoort", "Gemeente s-Hertogenbosch"
    ];

    public static readonly string[] Afdelingen =
    [
        "Burgerzaken", "Publiekszaken", "Vergunningen", "Handhaving", "Ruimtelijke Ordening",
        "Sociale Zaken", "Werk en Inkomen", "Financien", "Juridische Zaken", "Belastingen",
        "Milieu", "Openbare Werken", "Stadsbeheer", "Communicatie", "ICT", "Personeelszaken"
    ];

    public static readonly string[] Medewerkers =
    [
        "j.jansen", "p.devries", "m.bakker", "a.smit", "k.dejong", "s.meijer", "r.dekker",
        "l.vandenBerg", "h.visser", "b.mulder", "c.hendriks", "d.boer", "e.groot", "f.bos",
        "g.peters", "w.brouwer", "t.kok", "n.dijkstra", "o.vos", "v.vermeer", "u.maas"
    ];

    public static readonly string[] Bedrijfsnamen =
    [
        "Bakkerij Het Broodje", "Restaurant De Gouden Leeuw", "Aannemersbedrijf Van der Berg",
        "Kapsalon Knipkunst", "Autogarage Peters", "Schildersbedrijf De Kwast", "Adviesbureau Visie",
        "Hoveniersbedrijf Groenrijk", "Installatiebedrijf TechnoBouw", "Architectenbureau Lijn & Vorm",
        "IT Solutions BV", "Logistiek Centrum Noord", "Transportbedrijf Snelweg", "Metaalbedrijf Staal",
        "Textiel Import BV", "Machinefabriek Holland", "Bouwbedrijf Fundament", "Administratiekantoor Plus"
    ];

    public static readonly string[] Rechtsvormen =
    [
        "Eenmanszaak", "VOF", "Maatschap", "BV", "NV", "Stichting", "Vereniging", "Cooperatie"
    ];

    public static readonly string[] DocumentFormaten =
    [
        "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "image/jpeg", "image/png", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "text/plain"
    ];

    public static readonly string[] ZaakActies =
    [
        "Zaak aangemaakt", "Status gewijzigd", "Document toegevoegd", "Betrokkene toegevoegd",
        "Notitie toegevoegd", "Behandelaar toegewezen", "Termijn verlengd", "Resultaat vastgesteld"
    ];
}
