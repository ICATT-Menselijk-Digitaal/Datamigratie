using System.Text.Json;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Tests.Common.Services.Det.Models
{
    /// <summary>
    /// Tests based on the DET OpenAPI spec DataElement schemas.
    /// Each DataElement has a required "type" (discriminator) and "naam".
    /// Subtypes add "waarde" (single value) or "waarden" (array).
    /// Spec: /q/openapi - components/schemas/DataElement
    /// </summary>
    public class DetZaakdataJsonTests
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

        // --- StringDataElement ---
        // waarde: string

        [Fact]
        public void Deserialize_StringDataElement_MapsWaarde()
        {
            var json = """{"type":"string","naam":"Omschrijving","waarde":"Testwaarde"}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetStringDataElement>(result);
            Assert.Equal("Omschrijving", element.Naam);
            Assert.Equal("Testwaarde", element.Waarde);
        }

        [Fact]
        public void Deserialize_StringDataElement_NullWaarde_IsAllowed()
        {
            var json = """{"type":"string","naam":"Omschrijving","waarde":null}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetStringDataElement>(result);
            Assert.Null(element.Waarde);
        }

        // --- BooleanDataElement ---
        // waarde: boolean

        [Fact]
        public void Deserialize_BooleanDataElement_TrueWaarde()
        {
            var json = """{"type":"boolean","naam":"IsActief","waarde":true}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetBooleanDataElement>(result);
            Assert.True(element.Waarde);
        }

        [Fact]
        public void Deserialize_BooleanDataElement_FalseWaarde()
        {
            var json = """{"type":"boolean","naam":"IsActief","waarde":false}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetBooleanDataElement>(result);
            Assert.False(element.Waarde);
        }

        // --- CalendarDataElement ---
        // waarde: string, format: date-time, example: 2022-03-10T12:15:50-04:00
        // Also handles discriminator "datummettijdstip"

        [Fact]
        public void Deserialize_CalendarDataElement_MapsWaarde()
        {
            var json = """{"type":"calendar","naam":"Afspraakdatum","waarde":"2022-03-10T12:15:50-04:00"}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetCalendarDataElement>(result);
            Assert.Equal(new DateTimeOffset(2022, 3, 10, 12, 15, 50, TimeSpan.FromHours(-4)), element.Waarde);
        }

        [Fact]
        public void Deserialize_DatumMetTijdstipDataElement_MapsToDetDatumMetTijdstipDataElement()
        {
            var json = """{"type":"datummettijdstip","naam":"Tijdstip","waarde":"2022-03-10T12:15:50-04:00"}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetDatumMetTijdstipDataElement>(result);
            Assert.Equal(new DateTimeOffset(2022, 3, 10, 12, 15, 50, TimeSpan.FromHours(-4)), element.Waarde);
        }

        // --- DecimaalDataElement ---
        // waarde: number
        // formattering: NummerFormattering enum (geheel_getal | geheel_getal_geformatteerd |
        //               twee_decimalen_geformatteerd | maximaal_decimalen_geformatteerd)

        [Fact]
        public void Deserialize_DecimaalDataElement_MapsWaardeAndFormattering()
        {
            var json = """{"type":"decimaal","naam":"Bedrag","waarde":123.45,"formattering":"twee_decimalen_geformatteerd"}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetDecimaalDataElement>(result);
            Assert.Equal(123.45m, element.Waarde);
            Assert.Equal("twee_decimalen_geformatteerd", element.Formattering);
        }

        [Fact]
        public void Deserialize_DecimaalDataElement_NullFormattering_IsAllowed()
        {
            var json = """{"type":"decimaal","naam":"Bedrag","waarde":42.0}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetDecimaalDataElement>(result);
            Assert.Equal(42.0m, element.Waarde);
            Assert.Null(element.Formattering);
        }

        // --- DecimalenDataElement ---
        // waarden: array of number, uniqueItems: true

        [Fact]
        public void Deserialize_DecimalenDataElement_MapsWaarden()
        {
            var json = """{"type":"decimalen","naam":"Metingen","waarden":[1.1,2.2,3.3]}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetDecimalenDataElement>(result);
            Assert.Equal([1.1m, 2.2m, 3.3m], element.Waarden);
        }

        // --- ComplexDataElement ---
        // waarde: string (description: "Complexe waarde data element")
        // Handles: optie, adresgegevens, referentietabel_record, afstand,
        //          generieke_afspraak, geo_informatie, digitale_notificaties, zaak_besluit

        [Theory]
        [InlineData("optie")]
        [InlineData("adresgegevens")]
        [InlineData("referentietabel_record")]
        [InlineData("afstand")]
        [InlineData("generieke_afspraak")]
        [InlineData("geo_informatie")]
        [InlineData("digitale_notificaties")]
        [InlineData("zaak_besluit")]
        public void Deserialize_ComplexDataElement_AllTypesMapToDetComplexDataElement(string type)
        {
            var json = $$"""{"type":"{{type}}","naam":"Veld","waarde":"somevalue"}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsAssignableFrom<DetComplexDataElement>(result);
            Assert.Equal("somevalue", element.Waarde);
        }

        // --- OptiesDataElement ---
        // waarden: array of string (required)

        [Fact]
        public void Deserialize_OptiesDataElement_MapsWaarden()
        {
            var json = """{"type":"opties","naam":"Keuzes","waarden":["optie1","optie2"]}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetOptiesDataElement>(result);
            Assert.Equal(["optie1", "optie2"], element.Waarden);
        }

        // --- StringsDataElement ---
        // waarden: array of string (required)

        [Fact]
        public void Deserialize_StringsDataElement_MapsWaarden()
        {
            var json = """{"type":"strings","naam":"Tags","waarden":["a","b","c"]}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetStringsDataElement>(result);
            Assert.Equal(["a", "b", "c"], element.Waarden);
        }

        // --- DocumentDataElement ---
        // waarden: array of string (bestandsnamen, required)
        // Also handles "select_documents"

        [Fact]
        public void Deserialize_DocumentDataElement_MapsWaarden()
        {
            var json = """{"type":"zaak_documenten","naam":"Bijlagen","waarden":["bestand1.pdf","bestand2.pdf"]}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetDocumentDataElement>(result);
            Assert.Equal(["bestand1.pdf", "bestand2.pdf"], element.Waarden);
        }

        [Fact]
        public void Deserialize_SelectDocumentDataElement_MapsToDetSelectDocumentDataElement()
        {
            var json = """{"type":"select_documents","naam":"Selectie","waarden":["doc1.pdf"]}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetSelectDocumentDataElement>(result);
            Assert.Equal(["doc1.pdf"], element.Waarden);
        }

        // --- AanvullijstDataElement ---
        // waarden: array of AanvullijstRecord (required)
        // AanvullijstRecord: recordNummer (int32, optional), itemIdentificatie (string, required), itemWaarde (string, optional)

        [Fact]
        public void Deserialize_AanvullijstDataElement_MapsWaarden()
        {
            var json = """
                {
                    "type": "aanvullijst",
                    "naam": "Producten",
                    "waarden": [
                        {"recordNummer": 1, "itemIdentificatie": "P001", "itemWaarde": "Product A"},
                        {"itemIdentificatie": "P002"}
                    ]
                }
                """;

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            var element = Assert.IsType<DetAanvullijstDataElement>(result);
            Assert.Equal(2, element.Waarden!.Count);
            Assert.Equal(1, element.Waarden[0].RecordNummer);
            Assert.Equal("P001", element.Waarden[0].ItemIdentificatie);
            Assert.Equal("Product A", element.Waarden[0].ItemWaarde);
            Assert.Null(element.Waarden[1].RecordNummer);
            Assert.Equal("P002", element.Waarden[1].ItemIdentificatie);
            Assert.Null(element.Waarden[1].ItemWaarde);
        }

        // --- Base DataElement fields ---
        // naam: string (required), omschrijving: string (optional)

        [Fact]
        public void Deserialize_DataElement_MapsOmschrijving()
        {
            var json = """{"type":"string","naam":"Veld","omschrijving":"Toelichting op het veld","waarde":"x"}""";

            var result = JsonSerializer.Deserialize<DetZaakdata>(json, Options);

            Assert.Equal("Toelichting op het veld", result!.Omschrijving);
        }
    }
}
