using System.ComponentModel.DataAnnotations;
using Bogus;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.Shared.Models;
using Datamigratie.FakeDet.Catalogi;
using Datamigratie.FakeDet.DataGeneration;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Datamigratie.FakeDet
{
    public record ZaaktypeDatabase(
        IReadOnlyDictionary<string, Zaaktype> Zaaktypen)
    {
        public static async Task<ZaaktypeDatabase> Create()
        {
            var zaaktypes = new OrderedDictionary<string, FakeDet.Zaaktype>();

            await foreach (var catalogus in GetCatalogusData.GetCatalogi())
            {
                foreach (var type in catalogus.Zaaktypen)
                {
                    zaaktypes[type.FunctioneleIdentificatie] = type.ToDetZaaktype();
                }
            }

            return new(zaaktypes);
        }
    }
    
    
    public static class FakeDetEndpoints
    {
        private static readonly Task<ZaaktypeDatabase> s_dataPromise = ZaaktypeDatabase.Create();

        private static readonly Task<IReadOnlyDictionary<long, string>> s_documentVersiePromise =
            ZaakDatabase.GetDocumentDictionary();

        public static async Task<Ok<PagedResponse<ZaaktypeOverzicht>>> GetAllZaaktypen()
        {
            var data = await s_dataPromise;
            var result = new PagedResponse<ZaaktypeOverzicht>
            {
                Count = data.Zaaktypen.Count,
                Results = data.Zaaktypen.Values.Cast<ZaaktypeOverzicht>().ToList()
            };
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<Zaaktype>, NotFound>> GetZaaktype(string zaaktypeName)
        {
            var data = await s_dataPromise;
            return data.Zaaktypen.TryGetValue(zaaktypeName, out var z)
                ? TypedResults.Ok(z)
                : TypedResults.NotFound();
        }

        public static async Task<Ok<PagedResponse<DetZaakMinimal>>> GetZakenByZaaktype(string zaaktype, [Range(1, int.MaxValue)] int page = 1)
        {
            var results = new List<DetZaakMinimal>();
            await foreach (var item in ZaakDatabase.GetZakenByZaaktype(zaaktype, page))
            {
                results.Add(item);
            }
            return TypedResults.Ok(new PagedResponse<DetZaakMinimal>
            {
                Results = results,
                Count = 1000,
                PreviousPage = page > 2,
                NextPage = page < 50
            });
        }

        public static async Task<Results<Ok<DetZaak>, NotFound>> GetZaak(string zaaknummer) => (await ZaakDatabase.GetZaak(zaaknummer)) is DetZaak zaak
            ? TypedResults.Ok(zaak)
            : TypedResults.NotFound();

        public static async Task<Results<PushStreamHttpResult, NotFound>> DownloadBestand(long id)
        {
            var dict = await s_documentVersiePromise;
            if (!dict.TryGetValue(id, out var functioneleIedntentificatie)) return TypedResults.NotFound();
            var zaak = await ZaakDatabase.GetZaak(functioneleIedntentificatie);
            var versie = zaak?.Documenten?.SelectMany(d => d.DocumentVersies).FirstOrDefault(v => v.DocumentInhoudID == id);
            return versie is { Documentgrootte: > 0 }
                ? TypedResults.Stream(
                    stream => TestFileGenerator.Generate(stream, versie.Mimetype, versie.Documentgrootte.Value, versie.Bestandsnaam),
                    contentType: versie.Mimetype)
                : TypedResults.NotFound();
        }
    }
}
