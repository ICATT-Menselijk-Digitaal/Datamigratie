using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.ShowOzZaaktypeInfo.Models
{
    public class EnrichedOzZaaktype : OzZaaktype
    {
        public List<OzStatustype> Statustypes { get; set; } = [];

        public List<OzResultaattype> Resultaattypen { get; set; } = [];

        public List<OzInformatieobjecttype> Informatieobjecttypen { get; set; } = [];

        public List<OzBesluittype> Besluittypen { get; set; } = [];

        public List<OzZaakVertrouwelijkheidaanduiding> OzZaakVertrouwelijkheidaanduidingen { get; set; } = [];

        public List<OzDocumentVertrouwelijkheidaanduiding> OzDocumentVertrouwelijkheidaanduidingen { get; set; } = [];
    }

    public class OzZaakVertrouwelijkheidaanduiding
    {
        public required string Value { get; set; }
        public required string Label { get; set; }
    }

    public class OzDocumentVertrouwelijkheidaanduiding
    {
        public required string Value { get; set; }
        public required string Label { get; set; }
    }
}
