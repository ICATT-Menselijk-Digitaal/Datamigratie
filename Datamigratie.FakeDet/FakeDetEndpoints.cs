using Bogus;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.Shared.Models;
using Datamigratie.FakeDet.DataGeneration;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Datamigratie.FakeDet
{
    public static class FakeDetEndpoints
    {
        private static readonly (IReadOnlyList<DetZaaktype> Zaaktypes, IReadOnlyList<DetZaak> Zaken) s_data = GetData();

        public static Ok<PagedResponse<DetZaaktype>> GetAllZaaktypen() => TypedResults.Ok(new PagedResponse<DetZaaktype>
        {
            Results = [.. s_data.Zaaktypes],
            Count = s_data.Zaken.Count,
        });

        public static Results<Ok<DetZaaktype>, NotFound> GetZaaktype(string zaaktypeName) => s_data.Zaaktypes.FirstOrDefault(z => z.FunctioneleIdentificatie == zaaktypeName) is DetZaaktype zaaktype
            ? TypedResults.Ok(zaaktype)
            : TypedResults.NotFound();

        public static Ok<PagedResponse<DetZaakMinimal>> GetZakenByZaaktype(string zaaktype)
        {
            var results = s_data.Zaken.Where(x => x.Zaaktype?.FunctioneleIdentificatie == zaaktype).Cast<DetZaakMinimal>().ToList();
            return TypedResults.Ok(new PagedResponse<DetZaakMinimal>
            {
                Results = results,
                Count = results.Count,
            });
        }

        public static Results<Ok<DetZaak>, NotFound> GetZaak(string zaaknummer) => s_data.Zaken.FirstOrDefault(z => z.FunctioneleIdentificatie == zaaknummer) is DetZaak zaak
            ? TypedResults.Ok(zaak)
            : TypedResults.NotFound();


        public static Results<PushStreamHttpResult, NotFound> DownloadBestand(long id)
        {
            var document = s_data.Zaken.SelectMany(z => z.Documenten ?? [])
                .SelectMany(x => x.DocumentVersies)
                .Where(d => d.DocumentInhoudID == id && d.Documentgrootte != null)
                .FirstOrDefault();

            return document?.Documentgrootte == null || document.Mimetype == null
                ? TypedResults.NotFound()
                : TypedResults.Stream(
                    stream => TestFileGenerator.Generate(stream, document.Mimetype, document.Documentgrootte.Value, document.Bestandsnaam),
                    contentType: document.Mimetype);
        }

        private static (IReadOnlyList<DetZaaktype> Zaaktypes, IReadOnlyList<DetZaak> Zaken) GetData(int seed = 123, int zaaktypenCount = 100, int zakenCount = 456)
        {
            // Maak 100 zaaktypen (eenmalig, herbruikbaar)
            var zaaktypeFaker = DetZaaktypeFactory.Faker(seed: seed);
            var zaaktypen = zaaktypeFaker.Generate(zaaktypenCount);

            // Zorg dat zaken één van deze zaaktypen “pakken”
            var f = new Faker("nl");
            DetZaaktype PickZaaktype() => f.PickRandom(zaaktypen);

            // Genereer zaken
            var zaakFaker = DetZaakFakers.ZaakFaker(PickZaaktype, seed: seed);
            var zaken = zaakFaker.Generate(zakenCount);

            return (zaaktypen, zaken);
        }
    }
}
