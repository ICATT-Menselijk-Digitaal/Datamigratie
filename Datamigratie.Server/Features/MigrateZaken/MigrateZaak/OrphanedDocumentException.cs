namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak;

public class OrphanedDocumentException(Guid documentId, Exception innerException)
    : Exception($"Document '{documentId}' is aangemaakt in OpenZaak maar kon niet worden opgeruimd na een fout tijdens de verwerking.", innerException)
{
    public Guid DocumentId { get; } = documentId;
}
