using Bogus;
using Datamigratie.FakeDet.DataGeneration.Zaken.Models;

namespace Datamigratie.FakeDet.DataGeneration.Zaken.Fakers;

using Zaaktype = Catalogi.Models.Zaaktype;
using ZaaktypeOverzicht = Models.ZaaktypeOverzicht;
using Zaakstatus = Models.Zaakstatus;
using BewaartermijnWaardering = Models.BewaartermijnWaardering;
using Besluittype = Models.Besluittype;
using Besluitcategorie = Models.Besluitcategorie;
using ZaaktypeEigenschap = Catalogi.Models.ZaaktypeEigenschap;
using DocumentPublicatieniveau = Models.DocumentPublicatieniveau;

public sealed class ZaakFaker
{
    private readonly Faker _faker;
    private int _zaakCounter;
    private readonly Dictionary<long, string> _documentInhoudRegistry = new();

    /// <summary>
    /// Registry mapping documentInhoudID to the functioneleIdentificatie of the zaak.
    /// </summary>
    public IReadOnlyDictionary<long, string> DocumentInhoudRegistry => _documentInhoudRegistry;

    public ZaakFaker(int seed = 12345)
    {
        _faker = new Faker("nl");
        Randomizer.Seed = new Random(seed);
    }

    public Zaak GenerateZaak(Zaaktype zaaktype)
    {
        _zaakCounter++;
        var zaakFunctioneleIdentificatie = $"{zaaktype.Identificatie ?? zaaktype.Omschrijving}-{DateTime.Now.Year}-{_zaakCounter:D6}";
        var creatieDatum = _faker.Date.Between(DateTime.Now.AddYears(-2), DateTime.Now);
        var doorlooptijdDagen = ParseIsoDurationToDays(zaaktype.Doorlooptijd) ?? 30;
        var streefdatum = DateOnly.FromDateTime(creatieDatum.AddDays(doorlooptijdDagen));
        var isAfgerond = _faker.Random.Bool(0.7f);
        var startStatus = zaaktype.Statussen?.FirstOrDefault(s => s.IsStartstatus == true);
        var eindStatus = zaaktype.Statussen?.FirstOrDefault(s => s.IsEindstatus == true);
        var huidigeStatus = isAfgerond && eindStatus is not null ? eindStatus : (startStatus ?? zaaktype.Statussen?.FirstOrDefault());
        var huidigResultaat = isAfgerond
            ? zaaktype.Resultaten?.FirstOrDefault()
            : zaaktype.Resultaten?.LastOrDefault();

        var initiator = GenerateInitiator(zaaktype);
        var behandelaar = _faker.PickRandom(DutchDataSets.Medewerkers);
        var afdeling = _faker.PickRandom(DutchDataSets.Afdelingen);

        // 10% of open zaken are suspended
        var isOpgeschort = !isAfgerond && zaaktype.OpschortingEnAanhoudingMogelijk && _faker.Random.Bool(0.1f);
        var opschorttermijnStart = isOpgeschort ? DateOnly.FromDateTime(creatieDatum.AddDays(_faker.Random.Int(5, 20))) : (DateOnly?)null;
        var opschorttermijnEind = isOpgeschort ? opschorttermijnStart!.Value.AddDays(_faker.Random.Int(14, 60)) : (DateOnly?)null;

        // 20% of older zaken are migrated from ZTC1
        var isOudeZaak = creatieDatum < DateTime.Now.AddMonths(-6);
        var isMigrated = isOudeZaak && _faker.Random.Bool(0.2f);
        var ztc1MigratiedatumTijd = isMigrated ? creatieDatum.AddDays(_faker.Random.Int(30, 180)) : (DateTime?)null;

        var historie = GenerateHistorie(zaaktype, creatieDatum, isAfgerond, behandelaar);

        var isVertrouwelijk = IsVertrouwelijk(zaaktype.Vertrouwelijkheidaanduiding);

        return new Zaak
        {
            FunctioneleIdentificatie = zaakFunctioneleIdentificatie,
            ExterneIdentificatie = $"EXT-{Guid.NewGuid():N}"[..20].ToUpperInvariant(),
            Omschrijving = GenerateOmschrijving(zaaktype),
            RedenStart = zaaktype.Aanleiding ?? $"Aanvraag {zaaktype.Omschrijving}",
            Zaaktype = new ZaaktypeOverzicht
            {
                FunctioneleIdentificatie = zaaktype.Identificatie,
                Naam = zaaktype.Omschrijving,
                Omschrijving = zaaktype.OmschrijvingGeneriek ?? zaaktype.Omschrijving,
                Domein = zaaktype.Domein
            },
            Vertrouwelijk = isVertrouwelijk,
            Behandelaar = behandelaar,
            Initiator = initiator,
            Afdeling = afdeling,
            Groep = $"Team {afdeling}",
            AangemaaktDoor = _faker.PickRandom(DutchDataSets.Medewerkers),
            Kanaal = _faker.PickRandom<Kanaal>(),
            CreatieDatumTijd = creatieDatum,
            WijzigDatumTijd = isAfgerond ? creatieDatum.AddDays(_faker.Random.Int(1, doorlooptijdDagen)) : null,
            Startdatum = DateOnly.FromDateTime(creatieDatum),
            Streefdatum = streefdatum,
            Fataledatum = streefdatum.AddDays(14),
            Einddatum = isAfgerond ? streefdatum.AddDays(_faker.Random.Int(-10, 5)) : null,
            ZaakStatus = new Zaakstatus
            {
                Naam = huidigeStatus?.Omschrijving ?? "Intake afgerond",
                Omschrijving = huidigeStatus?.OmschrijvingGeneriek,
                Actief = true,
                Uitwisselingscode = huidigeStatus?.Omschrijving?.ToUpperInvariant().Replace(" ", "_") ?? "INTAKE",
                ExterneNaam = huidigeStatus?.Omschrijving ?? "Intake afgerond",
                Start = huidigeStatus?.IsStartstatus ?? false,
                Eind = huidigeStatus?.IsEindstatus ?? false
            },
            Resultaat = huidigResultaat is null
                ? null
                : new ZaakResultaat
                {
                    Actief = true,
                    Naam = huidigResultaat.Omschrijving,
                    Omschrijving = huidigResultaat.Toelichting,
                    Uitwisselingscode = huidigResultaat.SelectielijstItem ?? "ONBEKEND"
                },
            Intake = false,
            ArchiveerGegevens = isAfgerond ? GenerateArchiveerGegevens(streefdatum) : null,
            Geolocatie = GenerateGeolocatie(),
            Historie = historie,
            Zaakdata = GenerateZaakdata(zaaktype),
            Organisatie = _faker.PickRandom(DutchDataSets.Gemeentenamen),
            GeautoriseerdVoorMedewerkers = isVertrouwelijk,
            GeautoriseerdeMedewerkers = isVertrouwelijk
                ? Enumerable.Range(0, _faker.Random.Int(1, 3))
                    .Select(_ => _faker.PickRandom(DutchDataSets.Medewerkers))
                    .Distinct()
                    .Append(behandelaar)
                    .Distinct()
                    .ToList()
                : null,
            Notities = _faker.Random.Bool(0.3f) ? GenerateNotities(creatieDatum, behandelaar) : null,
            ProcesGestart = true,
            Heropend = false,
            Open = !isAfgerond,
            Vernietiging = false,
            Notificeerbaar = true,
            Gemigreerd = false,
            Betrokkenen = GenerateBetrokkenen(zaaktype, initiator),
            Documenten = GenerateDocumenten(zaaktype, creatieDatum, zaakFunctioneleIdentificatie),
            Betaalgegevens = GenerateBetaalgegevens(zaaktype, isAfgerond),
            Taken = GenerateTaken(zaaktype, creatieDatum, isAfgerond, behandelaar, afdeling),
            BagObjecten = GenerateBagObjecten(zaaktype),
            GekoppeldeZaken = null, // Populated later via PopulateGekoppeldeZaken
            Besluiten = GenerateBesluiten(zaaktype, isAfgerond, streefdatum),
            Contacten = GenerateContacten(),
            OpschorttermijnStartdatum = opschorttermijnStart,
            OpschorttermijnEinddatum = opschorttermijnEind,
            Ztc1MigratiedatumTijd = ztc1MigratiedatumTijd
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

    private static bool IsVertrouwelijk(string? vertrouwelijkheidaanduiding)
    {
        return vertrouwelijkheidaanduiding?.ToLowerInvariant() switch
        {
            "openbaar" or "beperkt_openbaar" => false,
            "intern" or "zaakvertrouwelijk" or "vertrouwelijk" or "confidentieel" or "geheim" or "zeer_geheim" => true,
            _ => false
        };
    }

    private Subject GenerateInitiator(Zaaktype zaaktype)
    {
        var isBedrijf = zaaktype.IndicatieInternOfExtern == "extern" && _faker.Random.Bool(0.3f);

        if (isBedrijf)
        {
            return GenerateBedrijf();
        }

        return GeneratePersoon();
    }

    private Persoon GeneratePersoon()
    {
        var geslacht = _faker.PickRandom<Geslacht>();
        var heeftVoorvoegsel = _faker.Random.Bool(0.3f);

        return new Persoon
        {
            Bsn = GenerateBsn(),
            Voornaam = _faker.PickRandom(DutchDataSets.Voornamen),
            Voorvoegsel = heeftVoorvoegsel ? _faker.PickRandom(DutchDataSets.Voorvoegsels) : null,
            Achternaam = _faker.PickRandom(DutchDataSets.Achternamen),
            Geslacht = geslacht,
            Geboortedatum = DateOnly.FromDateTime(_faker.Date.Between(DateTime.Now.AddYears(-80), DateTime.Now.AddYears(-18))),
            Emailadres = _faker.Internet.Email(),
            Telefoonnummer = GenerateDutchPhoneNumber(),
            TelefoonnummerAlternatief = _faker.Random.Bool(0.2f) ? GenerateDutchPhoneNumber() : null,
            Rekeningnummer = _faker.Random.Bool(0.3f) ? GenerateIban() : null,
            OntvangenZaakNotificaties = _faker.Random.Bool(0.7f),
            ToestemmingZaakNotificatiesAlleenDigitaal = _faker.Random.Bool(0.5f),
            HandmatigToegevoegd = _faker.Random.Bool(0.2f),
            Adressen = [GenerateAdres()]
        };
    }

    private Bedrijf GenerateBedrijf()
    {
        return new Bedrijf
        {
            KvkNummer = _faker.Random.Replace("########"),
            Vestigingsnummer = _faker.Random.Replace("############"),
            Bedrijfsnaam = _faker.PickRandom(DutchDataSets.Bedrijfsnamen),
            Rechtsvorm = _faker.PickRandom(DutchDataSets.Rechtsvormen),
            Emailadres = _faker.Internet.Email(),
            Telefoonnummer = GenerateDutchPhoneNumber(),
            TelefoonnummerAlternatief = _faker.Random.Bool(0.2f) ? GenerateDutchPhoneNumber() : null,
            Rekeningnummer = _faker.Random.Bool(0.4f) ? GenerateIban() : null,
            OntvangenZaakNotificaties = _faker.Random.Bool(0.6f),
            ToestemmingZaakNotificatiesAlleenDigitaal = _faker.Random.Bool(0.4f),
            HandmatigToegevoegd = _faker.Random.Bool(0.15f),
            Adressen = [GenerateAdres(AdresType.VestigingsAdresBedrijf)]
        };
    }

    private Adres GenerateAdres(AdresType type = AdresType.VerblijfAdresPersoon)
    {
        return new Adres
        {
            Type = type,
            Straatnaam = _faker.PickRandom(DutchDataSets.Straatnamen),
            Huisnummer = _faker.Random.Int(1, 250),
            Huisletter = _faker.Random.Bool(0.1f) ? _faker.Random.String2(1, "ABCDEFGH") : null,
            Huisnummertoevoeging = _faker.Random.Bool(0.1f) ? _faker.Random.Int(1, 10).ToString() : null,
            Postcode = $"{_faker.Random.Int(1000, 9999)}{_faker.Random.String2(2, "ABCDEFGHJKLMNPRSTUVWXYZ")}",
            Plaatsnaam = _faker.PickRandom(DutchDataSets.Plaatsnamen),
            BuitenlandsAdres = false,
            Land = new Land { Code = "NL", Naam = "Nederland" }
        };
    }

    private string GenerateBsn()
    {
        while (true)
        {
            var digits = Enumerable.Range(0, 9).Select(_ => _faker.Random.Int(0, 9)).ToArray();
            var checksum = 0;
            for (var i = 0; i < 9; i++)
            {
                checksum += digits[i] * (9 - i);
            }

            if (checksum % 11 == 0)
            {
                return string.Join("", digits);
            }

            digits[8] = (11 - checksum % 11) % 11;
            if (digits[8] < 10)
            {
                return string.Join("", digits);
            }
        }
    }

    private string GenerateDutchPhoneNumber()
    {
        var isMobile = _faker.Random.Bool();
        return isMobile
            ? $"06-{_faker.Random.Int(10000000, 99999999)}"
            : $"0{_faker.Random.Int(10, 88)}-{_faker.Random.Int(1000000, 9999999)}";
    }

    private string GenerateIban()
    {
        // Generate a Dutch IBAN (NL + 2 check digits + 4 bank code + 10 account number)
        var bankCodes = new[] { "ABNA", "INGB", "RABO", "SNSB", "TRIO", "ASNB" };
        var bankCode = _faker.PickRandom(bankCodes);
        var accountNumber = _faker.Random.Long(1000000000, 9999999999);

        // Simplified IBAN - real check digits would require modulo 97 calculation
        var checkDigits = _faker.Random.Int(10, 99);
        return $"NL{checkDigits}{bankCode}{accountNumber}";
    }

    private string GenerateOmschrijving(Zaaktype zaaktype)
    {
        var templates = new[]
        {
            $"{zaaktype.HandelingInitiator ?? "Aanvraag"} {zaaktype.Onderwerp ?? zaaktype.Omschrijving}",
            $"{zaaktype.Omschrijving} voor {_faker.PickRandom(DutchDataSets.Straatnamen)} {_faker.Random.Int(1, 100)}",
            $"{zaaktype.Omschrijving} - {_faker.PickRandom(DutchDataSets.Plaatsnamen)}",
            $"Behandeling {zaaktype.Omschrijving.ToLowerInvariant()}"
        };
        return _faker.PickRandom(templates);
    }

    private ArchiveerGegevens GenerateArchiveerGegevens(DateOnly einddatum)
    {
        return new ArchiveerGegevens
        {
            BewaartermijnEinddatum = einddatum.AddYears(_faker.Random.Int(1, 10)),
            BewaartermijnWaardering = _faker.PickRandom<BewaartermijnWaardering>(),
            BeperkingOpenbaarheid = _faker.Random.Bool(0.1f),
            BeperkingOpenbaarheidReden = null
        };
    }

    private Geolocatie GenerateGeolocatie()
    {
        // Netherlands RD_NEW (EPSG:28992) coordinates
        // X range: ~7,000 to ~300,000 meters
        // Y range: ~289,000 to ~629,000 meters
        return new Geolocatie
        {
            Type = "Point",
            Coordinates =
            [
                _faker.Random.Double(100000, 200000),  // X coordinate (Easting)
                _faker.Random.Double(400000, 500000)   // Y coordinate (Northing)
            ]
        };
    }

    private IReadOnlyList<ZaakHistorie> GenerateHistorie(Zaaktype zaaktype, DateTime startDatum, bool isAfgerond, string behandelaar)
    {
        var historie = new List<ZaakHistorie>
        {
            new()
            {
                TypeWijziging = ZaakHistorieTypeWijziging.Status,
                WijzigingDatum = DateOnly.FromDateTime(startDatum),
                Toelichting = $"Zaak geregistreerd voor {zaaktype.Omschrijving}",
                GewijzigdDoor = _faker.PickRandom(DutchDataSets.Medewerkers)
            }
        };

        var currentDatum = startDatum;

        if (zaaktype.Statussen is not null)
        {
            var statussenToProcess = isAfgerond
                ? zaaktype.Statussen
                : zaaktype.Statussen.Take(_faker.Random.Int(1, Math.Max(1, zaaktype.Statussen.Count - 1)));

            foreach (var status in statussenToProcess.Skip(1))
            {
                currentDatum = currentDatum.AddDays(_faker.Random.Int(1, 5));
                historie.Add(new ZaakHistorie
                {
                    TypeWijziging = ZaakHistorieTypeWijziging.Status,
                    WijzigingDatum = DateOnly.FromDateTime(currentDatum),
                    Toelichting = $"Status gewijzigd naar: {status.Omschrijving}",
                    GewijzigdDoor = behandelaar
                });
            }
        }

        // Add document received event
        if (_faker.Random.Bool(0.5f))
        {
            historie.Add(new ZaakHistorie
            {
                TypeWijziging = ZaakHistorieTypeWijziging.NieuwDocument,
                WijzigingDatum = DateOnly.FromDateTime(startDatum.AddDays(_faker.Random.Int(1, 5))),
                Toelichting = "Aanvullende documentatie ontvangen",
                GewijzigdDoor = behandelaar
            });
        }

        // Add transfer event (overdragen)
        if (_faker.Random.Bool(0.2f))
        {
            var oudeMedewerker = _faker.PickRandom(DutchDataSets.Medewerkers);
            historie.Add(new ZaakHistorie
            {
                TypeWijziging = ZaakHistorieTypeWijziging.Overdragen,
                WijzigingDatum = DateOnly.FromDateTime(startDatum.AddDays(_faker.Random.Int(3, 10))),
                OudeWaarde = oudeMedewerker,
                NieuweWaarde = behandelaar,
                Toelichting = $"Zaak overgedragen van {oudeMedewerker} naar {behandelaar}",
                GewijzigdDoor = oudeMedewerker
            });
        }

        // Add streefdatum change
        if (_faker.Random.Bool(0.15f))
        {
            var oudeStreefdatum = DateOnly.FromDateTime(startDatum.AddDays(14));
            var nieuweStreefdatum = oudeStreefdatum.AddDays(_faker.Random.Int(7, 21));
            historie.Add(new ZaakHistorie
            {
                TypeWijziging = ZaakHistorieTypeWijziging.Streefdatum,
                WijzigingDatum = DateOnly.FromDateTime(startDatum.AddDays(_faker.Random.Int(5, 12))),
                OudeWaarde = oudeStreefdatum.ToString("yyyy-MM-dd"),
                NieuweWaarde = nieuweStreefdatum.ToString("yyyy-MM-dd"),
                Toelichting = "Streefdatum verlengd wegens complexiteit",
                GewijzigdDoor = behandelaar
            });
        }

        // Add kanaal change
        if (_faker.Random.Bool(0.1f))
        {
            historie.Add(new ZaakHistorie
            {
                TypeWijziging = ZaakHistorieTypeWijziging.Kanaal,
                WijzigingDatum = DateOnly.FromDateTime(startDatum.AddDays(_faker.Random.Int(1, 3))),
                OudeWaarde = "email",
                NieuweWaarde = "telefoon",
                Toelichting = "Kanaal aangepast na telefonisch contact",
                GewijzigdDoor = behandelaar
            });
        }

        // Add zaak_omschrijving change
        if (_faker.Random.Bool(0.1f))
        {
            historie.Add(new ZaakHistorie
            {
                TypeWijziging = ZaakHistorieTypeWijziging.ZaakOmschrijving,
                WijzigingDatum = DateOnly.FromDateTime(startDatum.AddDays(_faker.Random.Int(2, 7))),
                Toelichting = "Omschrijving verduidelijkt",
                GewijzigdDoor = behandelaar
            });
        }

        return historie.OrderBy(h => h.WijzigingDatum).ToList();
    }

    private IReadOnlyList<ZaakDataElement>? GenerateZaakdata(Zaaktype zaaktype)
    {
        if (zaaktype.Eigenschappen is null || zaaktype.Eigenschappen.Count == 0)
            return null;

        return zaaktype.Eigenschappen
            .Where(e => e.Naam != "-")
            .Select(e => new ZaakDataElement
            {
                Naam = e.Naam,
                Waarde = e.Waarde ?? GeneratePropertyValue(e),
                Datatype = e.Datatype
            })
            .ToList();
    }

    private string GeneratePropertyValue(ZaaktypeEigenschap eigenschap)
    {
        return eigenschap.Datatype?.ToLowerInvariant() switch
        {
            "string" or "tekst" => _faker.Lorem.Word(),
            "number" or "integer" or "nummer" or "decimaal" => _faker.Random.Int(1, 1000).ToString(),
            "boolean" => _faker.Random.Bool().ToString().ToLowerInvariant(),
            "date" or "datum" => DateOnly.FromDateTime(_faker.Date.Past()).ToString("yyyy-MM-dd"),
            "datum_tijd" => _faker.Date.Past().ToString("yyyy-MM-ddTHH:mm:ss"),
            "optie" => eigenschap.Opties?.FirstOrDefault() ?? _faker.Lorem.Word(),
            "opties" => eigenschap.Opties != null ? string.Join(", ", eigenschap.Opties.Take(2)) : _faker.Lorem.Word(),
            _ => _faker.Lorem.Word()
        };
    }

    private IReadOnlyList<ZaakNotitie> GenerateNotities(DateTime startDatum, string behandelaar)
    {
        var aantalNotities = _faker.Random.Int(1, 3);
        return Enumerable.Range(0, aantalNotities)
            .Select(i => new ZaakNotitie
            {
                Medewerker = i == 0 ? behandelaar : _faker.PickRandom(DutchDataSets.Medewerkers),
                DatumTijd = startDatum.AddDays(_faker.Random.Int(1, 30)),
                Notitie = _faker.Lorem.Sentence()
            })
            .OrderBy(n => n.DatumTijd)
            .ToList();
    }

    private IReadOnlyList<ZaakBetrokkene> GenerateBetrokkenen(Zaaktype zaaktype, Subject initiator)
    {
        var zaakStartdatum = DateOnly.FromDateTime(DateTime.Now.AddDays(-_faker.Random.Int(1, 60)));

        var betrokkenen = new List<ZaakBetrokkene>
        {
            new()
            {
                IndCorrespondentie = true,
                Startdatum = zaakStartdatum,
                Betrokkene = initiator,
                TypeBetrokkenheid = ZaakBetrokkenetype.Belanghebbende
            }
        };

        if (zaaktype.Betrokkenetypen is not null)
        {
            var extraBetrokkenen = zaaktype.Betrokkenetypen
                .Where(b => b.Naam != "Initiator" && _faker.Random.Bool(0.3f))
                .Take(2)
                .Select(b => new ZaakBetrokkene
                {
                    IndCorrespondentie = _faker.Random.Bool(0.5f),
                    Startdatum = zaakStartdatum.AddDays(_faker.Random.Int(0, 10)),
                    Betrokkene = GeneratePersoon(),
                    TypeBetrokkenheid = MapBetrokkenetype(b.Naam),
                    Toelichting = _faker.Random.Bool(0.3f) ? _faker.Lorem.Sentence() : null
                });
            betrokkenen.AddRange(extraBetrokkenen);
        }

        return betrokkenen;
    }

    private static ZaakBetrokkenetype MapBetrokkenetype(string naam)
    {
        return naam.ToLowerInvariant() switch
        {
            "gemachtigde" => ZaakBetrokkenetype.Gemachtigde,
            "medeaanvrager" => ZaakBetrokkenetype.Medeaanvrager,
            "melder" => ZaakBetrokkenetype.Melder,
            "plaatsvervanger" => ZaakBetrokkenetype.Plaatsvervanger,
            "belanghebbende" => ZaakBetrokkenetype.Belanghebbende,
            _ => ZaakBetrokkenetype.Overig
        };
    }

    private IReadOnlyList<ZaakDocument> GenerateDocumenten(Zaaktype zaaktype, DateTime startDatum, string zaakFunctioneleIdentificatie)
    {
        // If no documenttypen defined, generate generic documents
        if (zaaktype.Documenttypen is not { Count: > 0 }) return [];

        var verplichteDocs = zaaktype.Documenttypen.Where(d => d.Verplicht == true);
        var optioneleDocs = zaaktype.Documenttypen.Where(d => d.Verplicht != true).Take(_faker.Random.Int(0, 2));

        return verplichteDocs.Concat(optioneleDocs)
            .Select((d, i) =>
            {
                var docId = $"DOC-{DateTime.Now.Year}-{_zaakCounter:D6}-{i + 1:D3}";
                var creatieDatumTijd = startDatum.AddDays(_faker.Random.Int(0, 10)).AddHours(_faker.Random.Int(8, 17));
                var auteur = _faker.PickRandom(DutchDataSets.Medewerkers);
                var formaat = _faker.PickRandom(DutchDataSets.DocumentFormaten);
                var extensie = GetExtensieVoorFormaat(formaat);
                var docNaam = d.Naam ?? d.Omschrijving ?? "Document";
                var bestandsnaam = $"{docNaam.Replace(" ", "_").ToLowerInvariant()}_{_faker.Random.AlphaNumeric(8)}.{extensie}";
                var richting = MapRichtingToEnum(d.Richting);
                var publicatieniveau = MapVertrouwelijkheidToPublicatieniveau(d.Vertrouwelijkheidaanduiding);
                var isAanvraagDocument = i == 0 && richting == DocumentRichting.Inkomend;

                // Create documentversie and register it in the dictionary
                var documentInhoudID = _faker.Random.Long(100000, 999999999);
                var documentversie = new Documentversie
                {
                    Versienummer = 1,
                    DocumentInhoudID = documentInhoudID,
                    Creatiedatum = DateOnly.FromDateTime(creatieDatumTijd),
                    Bestandsnaam = bestandsnaam,
                    Mimetype = formaat,
                    Compressed = false,
                    Auteur = auteur,
                    Afzender = richting == DocumentRichting.Inkomend ? _faker.Name.FullName() : null,
                    Documentgrootte = _faker.Random.Long(10000, 5000000)
                };
                _documentInhoudRegistry[documentInhoudID] = zaakFunctioneleIdentificatie;

                return new ZaakDocument
                {
                    FunctioneleIdentificatie = docId,
                    Documenttype = new ZaakDocumenttype
                    {
                        Naam = docNaam,
                        Omschrijving = d.Omschrijving,
                        Actief = true,
                        Documentcategorie = d.Documentcategorie,
                        Publicatieniveau = publicatieniveau
                    },
                    DocumentStatus = new ZaakDocumentStatus
                    {
                        Naam = "Definitief",
                        Omschrijving = "Document is definitief",
                        Actief = true
                    },
                    Titel = $"{docNaam} - {_faker.Lorem.Words(2).Aggregate((a, b) => $"{a} {b}")}",
                    Kenmerk = _faker.Random.Bool(0.3f) ? $"KNM-{_faker.Random.AlphaNumeric(8).ToUpperInvariant()}" : null,
                    CreatieDatumTijd = creatieDatumTijd,
                    WijzigDatumTijd = _faker.Random.Bool(0.4f) ? creatieDatumTijd.AddDays(_faker.Random.Int(1, 5)) : null,
                    Publicatieniveau = publicatieniveau,
                    AanvraagDocument = isAanvraagDocument,
                    OntvangstDatum = richting == DocumentRichting.Inkomend ? DateOnly.FromDateTime(creatieDatumTijd) : null,
                    VerzendDatum = richting == DocumentRichting.Uitgaand ? DateOnly.FromDateTime(creatieDatumTijd.AddDays(1)) : null,
                    Documentrichting = richting,
                    Locatie = null,
                    Beschrijving = d.Omschrijving,
                    Documentversies = [documentversie],
                    Historie =
                    [
                        new Documenthistorie
                        {
                            TypeWijziging = DocumentHistorieTypeWijziging.Status,
                            WijzigingDatum = DateOnly.FromDateTime(creatieDatumTijd),
                            GewijzigdDoor = auteur,
                            Toelichting = "Document aangemaakt"
                        }
                    ],
                    GeautoriseerdVoorMedewerkers = true,
                    ConverterenNaarPdfa = formaat.Equals("PDF", StringComparison.OrdinalIgnoreCase),
                    Locked = false,
                    LockEigenaarId = null,
                    LockDatumTijd = null
                };
            })
            .ToList();
    }

    private static DocumentRichting MapRichtingToEnum(string? richting)
    {
        return richting?.ToLowerInvariant() switch
        {
            "inkomend" => DocumentRichting.Inkomend,
            "uitgaand" => DocumentRichting.Uitgaand,
            "intern" => DocumentRichting.Intern,
            _ => DocumentRichting.Intern
        };
    }

    private static DocumentPublicatieniveau MapVertrouwelijkheidToPublicatieniveau(string? vertrouwelijkheidaanduiding)
    {
        return vertrouwelijkheidaanduiding?.ToLowerInvariant() switch
        {
            "openbaar" or "beperkt_openbaar" => DocumentPublicatieniveau.Extern,
            "intern" or "zaakvertrouwelijk" => DocumentPublicatieniveau.Intern,
            "vertrouwelijk" or "confidentieel" or "geheim" or "zeer_geheim" => DocumentPublicatieniveau.Vertrouwelijk,
            _ => DocumentPublicatieniveau.Intern
        };
    }

    private static string GetExtensieVoorFormaat(string formaat)
    {
        return formaat.ToLowerInvariant() switch
        {
            "application/pdf" => "pdf",
            "application/msword" => "doc",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => "docx",
            "application/vnd.ms-excel" => "xls",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => "xlsx",
            "text/plain" => "txt",
            "image/jpeg" => "jpg",
            "image/png" => "png",
            "application/zip" => "zip",
            "application/xml" => "xml",
            "text/xml" => "xml",
            _ => "pdf"
        };
    }

    private Betaalgegevens? GenerateBetaalgegevens(Zaaktype zaaktype, bool isAfgerond)
    {
        // Only generate for payment-related zaaktypen (subsidies, permits, etc.)
        var paymentKeywords = new[] { "subsidie", "vergunning", "leges", "heffing", "belasting", "betaal", "kosten" };
        var isPaymentRelated = paymentKeywords.Any(k =>
            zaaktype.Omschrijving.Contains(k, StringComparison.OrdinalIgnoreCase) ||
            (zaaktype.OmschrijvingGeneriek?.Contains(k, StringComparison.OrdinalIgnoreCase) ?? false));

        if (!isPaymentRelated || !_faker.Random.Bool(0.6f))
            return null;

        var isPaid = isAfgerond && _faker.Random.Bool(0.8f);

        return new Betaalgegevens
        {
            TransactieId = isPaid ? $"TXN-{_faker.Random.AlphaNumeric(16).ToUpperInvariant()}" : null,
            Kenmerk = $"BET-{DateTime.Now.Year}-{_zaakCounter:D6}",
            Bedrag = _faker.Random.Decimal(25m, 2500m),
            TransactieDatum = isPaid ? DateOnly.FromDateTime(_faker.Date.Recent(30)) : null,
            Betaalstatus = isPaid ? Betaalstatus.Geslaagd :
                          _faker.Random.Bool(0.3f) ? Betaalstatus.InBehandeling : null
        };
    }

    private IReadOnlyList<Taak>? GenerateTaken(Zaaktype zaaktype, DateTime creatieDatum, bool isAfgerond, string behandelaar, string afdeling)
    {
        if (!_faker.Random.Bool(0.5f))
            return null;

        var taakNamen = new[]
        {
            "Beoordelen aanvraag", "Controleren documenten", "Opstellen advies",
            "Uitvoeren controle", "Afhandelen correspondentie", "Verwerken gegevens",
            "Plannen inspectie", "Opstellen besluit", "Archiveren dossier"
        };

        var aantalTaken = _faker.Random.Int(1, 3);
        var taken = new List<Taak>();
        var currentDatum = DateOnly.FromDateTime(creatieDatum);

        for (var i = 0; i < aantalTaken; i++)
        {
            var taakStartdatum = currentDatum.AddDays(_faker.Random.Int(1, 5));
            var taakStreefdatum = taakStartdatum.AddDays(_faker.Random.Int(3, 14));
            var isTaakAfgerond = isAfgerond || _faker.Random.Bool(0.6f);

            taken.Add(new Taak
            {
                FunctioneelIdentificatie = $"TAAK-{DateTime.Now.Year}-{_zaakCounter:D6}-{i + 1:D2}",
                Afdeling = afdeling,
                Groep = $"Team {afdeling}",
                Behandelaar = _faker.Random.Bool(0.7f) ? behandelaar : _faker.PickRandom(DutchDataSets.Medewerkers),
                Startdatum = taakStartdatum,
                Streefdatum = taakStreefdatum,
                Fataledatum = taakStreefdatum.AddDays(7),
                Einddatum = isTaakAfgerond ? taakStreefdatum.AddDays(_faker.Random.Int(-3, 2)).ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(_faker.Random.Int(8, 17)))) : null,
                IndicatieExternToegankelijk = zaaktype.IndicatieInternOfExtern == "extern" && _faker.Random.Bool(0.2f),
                AfgehandeldDoor = isTaakAfgerond ? behandelaar : null,
                Taaktype = _faker.PickRandom<Taaktype>(),
                TaaktypeOrigineel = Taaktype.Standaard,
                Historie =
                [
                    new TaakHistorie
                    {
                        TypeWijziging = TaakHistorieTypeWijziging.Status,
                        WijzigingDatum = taakStartdatum,
                        GewijzigdDoor = behandelaar,
                        Toelichting = "Taak aangemaakt"
                    }
                ]
            });

            currentDatum = taakStartdatum;
        }

        return taken;
    }

    private IReadOnlyList<BAGObject>? GenerateBagObjecten(Zaaktype zaaktype)
    {
        // Only generate for location-related zaaktypen
        var locationKeywords = new[] { "bouw", "vergunning", "woning", "pand", "adres", "ruimte", "omgeving", "sloop", "verbouw" };
        var isLocationRelated = locationKeywords.Any(k =>
            zaaktype.Omschrijving.Contains(k, StringComparison.OrdinalIgnoreCase) ||
            zaaktype.Domein?.Contains("omgeving", StringComparison.OrdinalIgnoreCase) == true);

        if (!isLocationRelated || !_faker.Random.Bool(0.7f))
            return null;

        var aantalObjecten = _faker.Random.Int(1, 2);
        return Enumerable.Range(0, aantalObjecten)
            .Select(_ => new BAGObject
            {
                // BAG nummeraanduiding format: 4 digits gemeentecode + 2 digits objecttype + 10 digits volgnummer
                BagObjectId = $"{_faker.Random.Int(1, 9999):D4}20{_faker.Random.Long(1000000000, 9999999999)}"
            })
            .ToList();
    }

    /// <summary>
    /// Populates gekoppeldeZaken for a zaak based on allowed zaaktype relations and existing zaken.
    /// </summary>
    /// <param name="zaak">The zaak to update</param>
    /// <param name="zaaktype">The zaaktype definition with gerelateerdeZaaktypen</param>
    /// <param name="zakenByZaaktype">Dictionary mapping zaaktype functioneleIdentificatie to list of zaken</param>
    /// <returns>Updated zaak with gekoppeldeZaken populated</returns>
    public Zaak PopulateGekoppeldeZaken(
        Zaak zaak,
        Zaaktype zaaktype,
        IReadOnlyDictionary<string, IReadOnlyList<Zaak>> zakenByZaaktype)
    {
        var koppelingen = GenerateGekoppeldeZaken(zaaktype, zakenByZaaktype);
        if (koppelingen is null || koppelingen.Count == 0)
            return zaak;

        return zaak with { GekoppeldeZaken = koppelingen };
    }

    private IReadOnlyList<ZaakZaakKoppeling>? GenerateGekoppeldeZaken(
        Zaaktype zaaktype,
        IReadOnlyDictionary<string, IReadOnlyList<Zaak>> zakenByZaaktype)
    {
        // If no gerelateerdeZaaktypen defined, no links possible
        if (zaaktype.GerelateerdeZaaktypen is null || zaaktype.GerelateerdeZaaktypen.Count == 0)
            return null;

        // 15% chance of having linked zaken
        if (!_faker.Random.Bool(0.15f))
            return null;

        var koppelingen = new List<ZaakZaakKoppeling>();
        var aantalKoppelingen = _faker.Random.Int(1, Math.Min(2, zaaktype.GerelateerdeZaaktypen.Count));

        // Pick random related zaaktypen to link to
        var gekozenRelaties = zaaktype.GerelateerdeZaaktypen
            .OrderBy(_ => _faker.Random.Int())
            .Take(aantalKoppelingen)
            .ToList();

        foreach (var relatie in gekozenRelaties)
        {
            // Find existing zaken of the related type
            if (!zakenByZaaktype.TryGetValue(relatie.ZaaktypeIdentificatie, out var kandidaatZaken) ||
                kandidaatZaken.Count == 0)
                continue;

            // Pick a random zaak from the candidates
            var randomIndex = _faker.Random.Int(0, kandidaatZaken.Count - 1);
            var gekoppeldeZaak = kandidaatZaken[randomIndex];

            koppelingen.Add(new ZaakZaakKoppeling
            {
                GekoppeldeZaak = gekoppeldeZaak.FunctioneleIdentificatie,
                Relatietype = MapAardRelatieToZaakRelatietype(relatie.AardRelatie),
                DossierEigenaar = _faker.Random.Bool(0.3f)
            });
        }

        return koppelingen.Count > 0 ? koppelingen : null;
    }

    private static ZaakRelatietype MapAardRelatieToZaakRelatietype(string? aardRelatie)
    {
        return aardRelatie?.ToLowerInvariant() switch
        {
            "hoofdzaak" => ZaakRelatietype.Hoofdzaak,
            "deelzaak" => ZaakRelatietype.Deelzaak,
            "vervolg" or "vervolgzaak" => ZaakRelatietype.Vervolgzaak,
            "voorafgaand" or "voorafgaande_zaak" => ZaakRelatietype.VoorafgaandeZaak,
            _ => ZaakRelatietype.GerelateerdeZaak
        };
    }

    private IReadOnlyList<Besluit>? GenerateBesluiten(Zaaktype zaaktype, bool isAfgerond, DateOnly streefdatum)
    {
        // Only generate for decision-related zaaktypen that are completed
        var decisionKeywords = new[] { "vergunning", "aanvraag", "bezwaar", "beroep", "ontheffing", "subsidie", "beschikking" };
        var requiresDecision = decisionKeywords.Any(k =>
            zaaktype.Omschrijving.Contains(k, StringComparison.OrdinalIgnoreCase));

        if (!requiresDecision || !isAfgerond || !_faker.Random.Bool(0.7f))
            return null;

        var besluitCategorien = new[] { "Toekenning", "Afwijzing", "Gedeeltelijke toekenning", "Intrekking" };
        var besluitNamen = new[] { "Vergunning verleend", "Aanvraag afgewezen", "Subsidie toegekend", "Bezwaar gegrond", "Bezwaar ongegrond" };

        var besluitDatum = streefdatum.AddDays(_faker.Random.Int(-5, 0));

        return
        [
            new Besluit
            {
                FunctioneleIdentificatie = $"BESL-{DateTime.Now.Year}-{_zaakCounter:D6}",
                Besluittype = new Besluittype
                {
                    Naam = _faker.PickRandom(besluitNamen),
                    Omschrijving = $"Besluit inzake {zaaktype.Omschrijving}",
                    Besluitcategorie = new Besluitcategorie
                    {
                        Naam = _faker.PickRandom(besluitCategorien)
                    },
                    ReactietermijnInDagen = _faker.Random.Int(14, 42),
                    PublicatieIndicatie = _faker.Random.Bool(0.3f)
                },
                BesluitDatum = besluitDatum,
                Ingangsdatum = besluitDatum,
                Vervaldatum = _faker.Random.Bool(0.5f) ? besluitDatum.AddYears(_faker.Random.Int(1, 5)) : null,
                Reactiedatum = besluitDatum.AddDays(_faker.Random.Int(14, 42)),
                BerekenVervaldatum = false,
                Toelichting = _faker.Random.Bool(0.4f) ? _faker.Lorem.Sentence() : null
            }
        ];
    }

    private IReadOnlyList<string>? GenerateContacten()
    {
        // 20% chance of having contact references
        if (!_faker.Random.Bool(0.2f))
            return null;

        var aantalContacten = _faker.Random.Int(1, 3);
        return Enumerable.Range(0, aantalContacten)
            .Select(_ => $"CONTACT-{DateTime.Now.Year}-{_faker.Random.Int(1, 99999):D6}")
            .ToList();
    }
}
