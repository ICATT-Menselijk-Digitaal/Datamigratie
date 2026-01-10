using System.ComponentModel.DataAnnotations;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.Shared.Models;
using Datamigratie.FakeDet.Catalogi;
using Datamigratie.FakeDet.DataGeneration;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
                    zaaktypes[type.Identificatie] = type.ToDetZaaktype();
                }
            }

            return new(zaaktypes);
        }
    }
    
    
    public static class FakeDetEndpoints
    {
        private static readonly Task<ZaaktypeDatabase> s_zaakTypePromise = ZaaktypeDatabase.Create();

        private static Task<IReadOnlyDictionary<long, string>>? s_documentVersiePromise;

        public static async Task<Ok<PagedResponse<ZaaktypeOverzicht>>> GetAllZaaktypen()
        {
            var data = await s_zaakTypePromise;
            var result = new PagedResponse<ZaaktypeOverzicht>
            {
                Count = data.Zaaktypen.Count,
                Results = data.Zaaktypen.Values.Cast<ZaaktypeOverzicht>().ToList()
            };
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<Zaaktype>, NotFound>> GetZaaktype(string zaaktypeName)
        {
            var data = await s_zaakTypePromise;
            return data.Zaaktypen.TryGetValue(zaaktypeName, out var z)
                ? TypedResults.Ok(z)
                : TypedResults.NotFound();
        }

        public static async Task<Ok<PagedResponse<DetZaakMinimal>>> GetZakenByZaaktype([FromServices] ZaakDatabase zaakDatabase, string zaaktype, [Range(1, int.MaxValue)] int page = 1)
        {
            var results = new List<DetZaakMinimal>();
            await foreach (var item in zaakDatabase.GetZakenByZaaktype(zaaktype, page))
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

        public static async Task<Results<Ok<DetZaak>, NotFound>> GetZaak([FromServices] ZaakDatabase zaakDatabase, string zaaknummer) => (await zaakDatabase.GetZaak(zaaknummer)) is DetZaak zaak
            ? TypedResults.Ok(zaak)
            : TypedResults.NotFound();

        public static async Task<Results<PushStreamHttpResult, NotFound>> DownloadBestand([FromServices] ZaakDatabase zaakDatabase, long id)
        {
            var dict = await (s_documentVersiePromise ??= zaakDatabase.GetDocumentDictionary());
            if (!dict.TryGetValue(id, out var functioneleIedntentificatie)) return TypedResults.NotFound();
            var zaak = await zaakDatabase.GetZaak(functioneleIedntentificatie);
            var versie = zaak?.Documenten?.SelectMany(d => d.DocumentVersies).FirstOrDefault(v => v.DocumentInhoudID == id);
            return versie is { Documentgrootte: > 0 }
                ? TypedResults.Stream(
                    stream => TestFileGenerator.Generate(stream, versie.Mimetype, versie.Documentgrootte.Value, versie.Bestandsnaam),
                    contentType: versie.Mimetype)
                : TypedResults.NotFound();
        }
    }
}
