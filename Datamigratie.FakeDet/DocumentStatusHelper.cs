using System.Text.Json;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.FakeDet;

public static class DocumentStatusHelper
{
    public static IEnumerable<DetDocumentstatus> GetDetDocumentstatusen() => JsonSerializer.Deserialize<IEnumerable<DetDocumentstatus>>(Json, JsonSerializerOptions.Web) ?? [];

    private const string Json = """
    [
        {
            "naam": "Concept",
            "omschrijving": "Concept"
        },
        {
            "naam": "Afgerond",
            "omschrijving": "Afgerond"
        },
        {
            "naam": "In bewerking",
            "omschrijving": "In bewerking"
        },
        {
            "naam": "Klad",
            "omschrijving": "Klad"
        },
        {
            "naam": "Ontwerp",
            "omschrijving": "Ontwerp"
        },
        {
            "naam": "Definitief",
            "omschrijving": "Definitief (systeemstatus)"
        },
        {
            "naam": "Nieuw",
            "omschrijving": "Nieuw (systeemstatus)"
        },
        {
            "naam": "Vastgesteld",
            "omschrijving": "Vastgesteld"
        },
        {
            "naam": "Vervallen",
            "omschrijving": "Vervallen"
        },
        {
            "naam": "Ontvangen",
            "omschrijving": "Ontvangen"
        },
        {
            "naam": "Afgehandeld",
            "omschrijving": "Afgehandelde status"
        },
        {
            "naam": "in bewerking",
            "omschrijving": "Gebruikt binnen StUF Test Platform compliancy test"
        },
        {
            "naam": "Regressietest",
            "omschrijving": "Regressietest"
        },
        {
            "naam": "Aanvullende informatie",
            "omschrijving": "Aanvullende informatie"
        },
        {
            "naam": "Behoort bij besluit",
            "omschrijving": "Behoort bij besluit"
        },
        {
            "naam": "Gereed voor ondertekening",
            "omschrijving": "OG Status voor document wachtend op ondertekening"
        },
        {
            "naam": "In behandeling",
            "omschrijving": null
        },
        {
            "naam": "Verzonden",
            "omschrijving": "Verzonden"
        },
        {
            "naam": "Gepubliceerd",
            "omschrijving": "Gepubliceerd"
        },
        {
            "naam": "Concept 2.0",
            "omschrijving": "Concept"
        },
        {
            "naam": "Klad 2.0",
            "omschrijving": "Klad"
        },
        {
            "naam": "Aanvraag",
            "omschrijving": null
        },
        {
            "naam": "Inbehandeling",
            "omschrijving": ""
        },
        {
            "naam": "Autovue",
            "omschrijving": "Autovue"
        },
        {
            "naam": "Als ingediend",
            "omschrijving": "Als ingediend"
        },
        {
            "naam": "definitief",
            "omschrijving": "definitief voor ZGW API"
        },
        {
            "naam": "in_bewerking",
            "omschrijving": "in bewerking voor ZGW API"
        },
        {
            "naam": "Buiten behandeling gesteld",
            "omschrijving": "De zaak is buiten behandeling gesteld"
        },
        {
            "naam": "Ondertekend",
            "omschrijving": null
        },
        {
            "naam": "Gewaarmerkt",
            "omschrijving": null
        },
        {
            "naam": "Fysieke verwerking Jeugdzorg",
            "omschrijving": "Fysieke verwerking Jeugdzorg"
        },
        {
            "naam": "Fysieke verwerking W&I/WMO",
            "omschrijving": "Fysieke verwerking W&I/WMO"
        },
        {
            "naam": "Aangenomen",
            "omschrijving": "Aangenomen"
        },
        {
            "naam": "Aangehouden",
            "omschrijving": "Aangehouden"
        },
        {
            "naam": "Agenderen in commissie",
            "omschrijving": "Agenderen in commissie"
        },
        {
            "naam": "Verworpen",
            "omschrijving": "Verworpen"
        },
        {
            "naam": "In Behandeling",
            "omschrijving": "Status voor een document In Behandeling"
        },
        {
            "naam": "Gearchiveerd",
            "omschrijving": "Gearchiveerd"
        },
        {
            "naam": "Ter vaststelling",
            "omschrijving": "Ter vaststelling"
        },
        {
            "naam": "Kopie",
            "omschrijving": "Kopie"
        }
    ]
    """;
}
