using System.ComponentModel.DataAnnotations;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.FakeDet.Catalogi;
using Datamigratie.FakeDet.DataGeneration;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.FakeDet
{
    public static class FakeDetEndpoints
    {
        public static async Task<Ok<DetPagedResponse<ZaaktypeOverzicht>>> GetAllZaaktypen([Range(1, int.MaxValue)] int page = 1)
        {
            const int PageSize = 100;

            var zaaktypen = new OrderedDictionary<string, ZaaktypeOverzicht>();

            await foreach (var catalogus in CatalogusDatabase.GetCatalogi())
            {
                foreach (var type in catalogus.Zaaktypen)
                {
                    zaaktypen[type.Identificatie] = type.ToDetZaaktype();
                }
            }

            var chunks = zaaktypen.Values.Chunk(PageSize).ToList();

            var result = new DetPagedResponse<ZaaktypeOverzicht>
            {
                Count = zaaktypen.Count,
                Results = chunks.ElementAtOrDefault(page - 1)?.ToList() ?? [],
                PreviousPage = page > 1,
                NextPage = chunks.Count > page
            };

            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<Zaaktype>, NotFound>> GetZaaktype(string zaaktypeName)
        {
            await foreach (var catalogus in CatalogusDatabase.GetCatalogi())
            {
                foreach (var type in catalogus.Zaaktypen)
                {
                    if (type.Identificatie == zaaktypeName)
                    {
                        return TypedResults.Ok(type.ToDetZaaktype());
                    }
                }
            }
            return TypedResults.NotFound();
        }

        public static async Task<Ok<DetPagedResponse<DetZaakMinimal>>> GetZakenByZaaktype([FromServices] ZaakDatabase zaakDatabase, string zaaktype, [Range(1, int.MaxValue)] int page = 1)
        {
            var results = new List<DetZaakMinimal>();
            var count = await zaakDatabase.GetZakenCountByZaaktype(zaaktype);
            await foreach (var item in zaakDatabase.GetZakenByZaaktype(zaaktype, page))
            {
                results.Add(item);
            }
            return TypedResults.Ok(new DetPagedResponse<DetZaakMinimal>
            {
                Results = results,
                Count = count,
                PreviousPage = page > 2,
                NextPage = page < count / ZaakDatabase.PageSize
            });
        }

        public static async Task<Results<Ok<DetZaak>, NotFound>> GetZaak([FromServices] ZaakDatabase zaakDatabase, string zaaknummer) => (await zaakDatabase.GetZaak(zaaknummer)) is DetZaak zaak
            ? TypedResults.Ok(zaak)
            : TypedResults.NotFound();

        public static async Task<Results<PushStreamHttpResult, NotFound>> DownloadBestand([FromServices] ZaakDatabase zaakDatabase, long id)
        {
            var dict = await zaakDatabase.GetDocumentDictionary();
            if (!dict.TryGetValue(id, out var functioneleIedntentificatie)) return TypedResults.NotFound();
            var zaak = await zaakDatabase.GetZaak(functioneleIedntentificatie);
            var versie = zaak?.Documenten?.SelectMany(d => d.DocumentVersies).FirstOrDefault(v => v.DocumentInhoudID == id);
            return versie is { Documentgrootte: > 0 }
                ? TypedResults.Stream(
                    stream => TestFileGenerator.Generate(stream, versie.Mimetype, versie.Documentgrootte.Value, versie.Bestandsnaam),
                    contentType: versie.Mimetype)
                : TypedResults.NotFound();
        }

        public static Ok<DetPagedResponse<DetDocumentstatus>> GetAllDocumentStatussen()
        {
            var results = DocumentStatusHelper.GetDetDocumentstatusen().ToList();
            return TypedResults.Ok(new DetPagedResponse<DetDocumentstatus>
            {
                Results = results,
                Count = results.Count
            });
        }
    }
}
