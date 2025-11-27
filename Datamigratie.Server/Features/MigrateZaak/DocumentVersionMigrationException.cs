namespace Datamigratie.Server.Features.MigrateZaak
{
    /// <summary>
    /// Exception thrown when a specific document version fails to migrate
    /// </summary>
    public class DocumentVersionMigrationException : Exception
    {
        public string Zaaknummer { get; }
        public string DocumentTitle { get; }
        public int VersionNumber { get; }
        public string FileName { get; }

        public DocumentVersionMigrationException(
            string zaaknummer,
            string documentTitle,
            int versionNumber,
            string fileName,
            string message,
            Exception? innerException = null)
            : base(message, innerException)
        {
            Zaaknummer = zaaknummer;
            DocumentTitle = documentTitle;
            VersionNumber = versionNumber;
            FileName = fileName;
        }

        public override string ToString()
        {
            return $"Document version migration failed for Zaak '{Zaaknummer}': " +
                   $"Document '{DocumentTitle}', Version {VersionNumber}, File '{FileName}'. " +
                   $"{Message}. {InnerException?.Message}";
        }
    }
}
