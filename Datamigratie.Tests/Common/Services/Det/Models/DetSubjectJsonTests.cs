using System.Text.Json;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Tests.Common.Services.Det.Models
{
    public class DetSubjectJsonTests
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

        [Fact]
        public void Deserialize_Persoon_ReturnsDetPersoon()
        {
            var json = """
                {
                    "subjecttype": "persoon",
                    "handmatigToegevoegd": false,
                    "burgerServiceNummer": "123456789",
                    "voornamen": "Jan",
                    "geslachtsNaam": "Jansen",
                    "geblokkeerd": false,
                    "curateleRegister": false,
                    "inOnderzoek": false,
                    "beperkingVerstrekking": false,
                    "afnemerIndicatie": true
                }
                """;

            var result = JsonSerializer.Deserialize<DetSubject>(json, Options);

            var persoon = Assert.IsType<DetPersoon>(result);
            Assert.Equal("123456789", persoon.BurgerServiceNummer);
            Assert.Equal("Jan", persoon.Voornamen);
            Assert.Equal("Jansen", persoon.GeslachtsNaam);
            Assert.False(persoon.Geblokkeerd);
            Assert.True(persoon.AfnemerIndicatie);
        }

        [Fact]
        public void Deserialize_Bedrijf_ReturnsDetBedrijf()
        {
            var json = """
                {
                    "subjecttype": "bedrijf",
                    "handmatigToegevoegd": false,
                    "kvkNummer": "12345678",
                    "bedrijfsnaam": "Acme BV",
                    "inSurceance": false,
                    "failliet": false,
                    "ingangsdatum": "2020-01-01",
                    "vestigingstype": "hoofdvestiging"
                }
                """;

            var result = JsonSerializer.Deserialize<DetSubject>(json, Options);

            var bedrijf = Assert.IsType<DetBedrijf>(result);
            Assert.Equal("12345678", bedrijf.KvkNummer);
            Assert.Equal("Acme BV", bedrijf.Bedrijfsnaam);
            Assert.False(bedrijf.Failliet);
            Assert.Equal(new DateOnly(2020, 1, 1), bedrijf.Ingangsdatum);
        }

        [Fact]
        public void Deserialize_ZaakWithPersoonInitiator_MapsCorrectly()
        {
            var json = """
                {
                    "functioneleIdentificatie": "ZAAK-001",
                    "open": true,
                    "omschrijving": "Test zaak",
                    "startdatum": "2024-01-01",
                    "streefdatum": "2024-06-01",
                    "handmatigToegevoegd": false,
                    "historie": [],
                    "initiator": {
                        "subjecttype": "persoon",
                        "handmatigToegevoegd": false,
                        "burgerServiceNummer": "987654321",
                        "voornamen": "Piet",
                        "geblokkeerd": false,
                        "curateleRegister": false,
                        "inOnderzoek": false,
                        "beperkingVerstrekking": false,
                        "afnemerIndicatie": false
                    }
                }
                """;

            var zaak = JsonSerializer.Deserialize<DetZaak>(json, Options);

            var persoon = Assert.IsType<DetPersoon>(zaak!.Initiator);
            Assert.Equal("987654321", persoon.BurgerServiceNummer);
            Assert.Equal("Piet", persoon.Voornamen);
        }

        [Fact]
        public void Deserialize_ZaakWithBedrijfInitiator_MapsCorrectly()
        {
            var json = """
                {
                    "functioneleIdentificatie": "ZAAK-002",
                    "open": true,
                    "omschrijving": "Test zaak",
                    "startdatum": "2024-01-01",
                    "streefdatum": "2024-06-01",
                    "handmatigToegevoegd": false,
                    "historie": [],
                    "initiator": {
                        "subjecttype": "bedrijf",
                        "handmatigToegevoegd": false,
                        "kvkNummer": "87654321",
                        "bedrijfsnaam": "Test BV",
                        "inSurceance": false,
                        "failliet": false,
                        "ingangsdatum": "2019-03-15",
                        "vestigingstype": "nevenvestiging"
                    }
                }
                """;

            var zaak = JsonSerializer.Deserialize<DetZaak>(json, Options);

            var bedrijf = Assert.IsType<DetBedrijf>(zaak!.Initiator);
            Assert.Equal("87654321", bedrijf.KvkNummer);
            Assert.Equal("Test BV", bedrijf.Bedrijfsnaam);
            Assert.Equal(new DateOnly(2019, 3, 15), bedrijf.Ingangsdatum);
        }

        [Fact]
        public void Deserialize_ZaakWithNullInitiator_InitiatorIsNull()
        {
            var json = """
                {
                    "functioneleIdentificatie": "ZAAK-003",
                    "open": false,
                    "omschrijving": "Test zaak",
                    "startdatum": "2024-01-01",
                    "streefdatum": "2024-06-01",
                    "handmatigToegevoegd": false,
                    "historie": [],
                    "initiator": null
                }
                """;

            var zaak = JsonSerializer.Deserialize<DetZaak>(json, Options);

            Assert.Null(zaak!.Initiator);
        }
    }
}
