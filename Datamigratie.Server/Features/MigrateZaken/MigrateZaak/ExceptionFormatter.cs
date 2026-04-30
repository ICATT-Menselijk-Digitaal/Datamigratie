namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak;

public static class ExceptionFormatter
{
    public static string FormatHttpStatusInfo(Exception ex)
    {
        var orphaned = FindOrphanedDocumentException(ex);
        if (orphaned != null)
        {
            return $" | Het document is aangemaakt in OpenZaak (ID: {orphaned.DocumentId}) maar kon niet worden gekoppeld of opgeruimd. Dit document moet handmatig worden verwijderd.";
        }

        var httpExceptions = FindHttpRequestExceptions(ex);
        if (httpExceptions.Count > 0)
        {
            var parts = httpExceptions.Select(http =>
                http.StatusCode is { } status
                    ? $"HTTP {(int)status}: {http.Message}"
                    : http.Message);
            return $" | {string.Join("; ", parts)}";
        }

        if (ex is AggregateException agg)
        {
            var parts = agg.Flatten().InnerExceptions.Select(inner => $"{inner.GetType().Name}: {inner.Message}");
            return $" | {string.Join("; ", parts)}";
        }

        return $" | {ex.GetType().Name}: {ex.Message}";
    }

    public static OrphanedDocumentException? FindOrphanedDocumentException(Exception ex) => ex switch
    {
        OrphanedDocumentException orphaned => orphaned,
        AggregateException agg => agg.Flatten().InnerExceptions
            .OfType<OrphanedDocumentException>()
            .FirstOrDefault(),
        _ => ex.InnerException is null ? null : FindOrphanedDocumentException(ex.InnerException),
    };

    public static List<HttpRequestException> FindHttpRequestExceptions(Exception ex) => ex switch
    {
        HttpRequestException http => [http],
        AggregateException agg => agg.Flatten().InnerExceptions
            .SelectMany(FindHttpRequestExceptions)
            .ToList(),
        _ => ex.InnerException is null ? [] : FindHttpRequestExceptions(ex.InnerException),
    };
}
