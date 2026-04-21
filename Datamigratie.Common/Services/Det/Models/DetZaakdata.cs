using System.Text.Json.Serialization;
using Datamigratie.Common.Converters;

namespace Datamigratie.Common.Services.Det.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(DetStringDataElement), "string")]
    [JsonDerivedType(typeof(DetBooleanDataElement), "boolean")]
    [JsonDerivedType(typeof(DetCalendarDataElement), "calendar")]
    [JsonDerivedType(typeof(DetDatumMetTijdstipDataElement), "datummettijdstip")]
    [JsonDerivedType(typeof(DetDecimaalDataElement), "decimaal")]
    [JsonDerivedType(typeof(DetDecimalenDataElement), "decimalen")]
    [JsonDerivedType(typeof(DetOptieDataElement), "optie")]
    [JsonDerivedType(typeof(DetAdresgegevensDataElement), "adresgegevens")]
    [JsonDerivedType(typeof(DetReferentietabelRecordDataElement), "referentietabel_record")]
    [JsonDerivedType(typeof(DetAfstandDataElement), "afstand")]
    [JsonDerivedType(typeof(DetGeneriekeAfspraakDataElement), "generieke_afspraak")]
    [JsonDerivedType(typeof(DetGeoInformatieDataElement), "geo_informatie")]
    [JsonDerivedType(typeof(DetDigitaleNotificatiesDataElement), "digitale_notificaties")]
    [JsonDerivedType(typeof(DetZaakBesluitDataElement), "zaak_besluit")]
    [JsonDerivedType(typeof(DetOptiesDataElement), "opties")]
    [JsonDerivedType(typeof(DetStringsDataElement), "strings")]
    [JsonDerivedType(typeof(DetDocumentDataElement), "zaak_documenten")]
    [JsonDerivedType(typeof(DetSelectDocumentDataElement), "select_documents")]
    [JsonDerivedType(typeof(DetAanvullijstDataElement), "aanvullijst")]
    public abstract class DetZaakdata
    {
        public string? Type { get; set; }
        public required string Naam { get; set; }
        public string? Omschrijving { get; set; }
    }

    // --- Single-value types ---

    public class DetStringDataElement : DetZaakdata
    {
        public string? Waarde { get; set; }
    }

    public class DetBooleanDataElement : DetZaakdata
    {
        public bool? Waarde { get; set; }
    }

    public class DetCalendarDataElement : DetZaakdata
    {
        [JsonConverter(typeof(DetZonedDateTimeConverter))]
        public DateTimeOffset? Waarde { get; set; }
    }

    public class DetDatumMetTijdstipDataElement : DetCalendarDataElement;

    public class DetDecimaalDataElement : DetZaakdata
    {
        public decimal? Waarde { get; set; }
        public string? Formattering { get; set; }
    }

    // --- Complex types (waarde: string per spec) ---

    public abstract class DetComplexDataElement : DetZaakdata
    {
        public string? Waarde { get; set; }
    }

    public class DetOptieDataElement : DetComplexDataElement;
    public class DetAdresgegevensDataElement : DetComplexDataElement;
    public class DetReferentietabelRecordDataElement : DetComplexDataElement;
    public class DetAfstandDataElement : DetComplexDataElement;
    public class DetGeneriekeAfspraakDataElement : DetComplexDataElement;
    public class DetGeoInformatieDataElement : DetComplexDataElement;
    public class DetDigitaleNotificatiesDataElement : DetComplexDataElement;
    public class DetZaakBesluitDataElement : DetComplexDataElement;

    // --- Multi-value types ---

    public class DetDecimalenDataElement : DetZaakdata
    {
        public List<decimal>? Waarden { get; set; }
    }

    public abstract class DetStringListDataElement : DetZaakdata
    {
        public List<string>? Waarden { get; set; }
    }

    public class DetOptiesDataElement : DetStringListDataElement;
    public class DetStringsDataElement : DetStringListDataElement;
    public class DetDocumentDataElement : DetStringListDataElement;

    public class DetSelectDocumentDataElement : DetDocumentDataElement;

    public class DetAanvullijstDataElement : DetZaakdata
    {
        public List<DetAanvullijstRecord>? Waarden { get; set; }
    }

    public class DetAanvullijstRecord
    {
        public int? RecordNummer { get; set; }
        public required string ItemIdentificatie { get; set; }
        public string? ItemWaarde { get; set; }
    }
}
