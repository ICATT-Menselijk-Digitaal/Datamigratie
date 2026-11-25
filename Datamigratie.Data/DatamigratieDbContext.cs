using Datamigratie.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Data;

public class DatamigratieDbContext(DbContextOptions options) : DbContext(options)
{
    public const int MaxLengthForIndexProperties = 256;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ZaaktypenMapping>(entity =>
        {
            entity.HasKey(m => m.Id);

            // Required fields (non-nullable)
            entity.Property(m => m.OzZaaktypeId)
                .IsRequired();

            entity.Property(m => m.DetZaaktypeId)
                .IsRequired();

            // Unique constraint: One DetZaaktypeId can only reference one OzZaaktypeId
            entity.HasIndex(m => m.DetZaaktypeId)
                .IsUnique()
                .HasDatabaseName("IX_Mapping_DetZaaktypeId_Unique");
        });

        modelBuilder.Entity<ZaaktypeVersieMapping>(entity =>
        {
            entity.HasOne(v => v.ZaaktypenMapping)
                .WithMany(m => m.Versies)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: one mapped OpenZaak zaaktype version per selectielijst year
            entity.HasIndex(v => new { v.ZaaktypenMappingId, v.SelectielijstJaar })
                .IsUnique()
                .HasDatabaseName("IX_ZaaktypeVersieMapping_ZaaktypenMappingId_SelectielijstJaar_Unique");
        });

        modelBuilder.Entity<ResultaattypeMapping>(entity =>
        {
            // Unique constraint: One DET Resultaattype (per zaaktype version) can only map to one OZ Resultaattype
            entity.HasIndex(m => new { m.ZaaktypeVersieMappingId, m.DetResultaattypeNaam })
            .IsUnique()
            .HasDatabaseName("IX_ResultaattypeMapping_ZaaktypeVersieMappingId_DetResultaattypeNaam_Unique");

        });

        modelBuilder.Entity<Migration>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DetZaaktypeId)
                .IsRequired();

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.LastUpdated)
                .IsRequired();

            entity.Property(e => e.ErrorMessage)
                .HasMaxLength(1000);

            entity.HasIndex(e => e.DetZaaktypeId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        modelBuilder.Entity<MigrationRecord>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DetZaaknummer)
                .IsRequired();

            entity.Property(e => e.OzZaaknummer);

            entity.Property(e => e.IsSuccessful)
                .IsRequired();

            entity.Property(e => e.ErrorTitle);

            entity.Property(e => e.ErrorDetails);

            entity.Property(e => e.ProcessedAt)
                .IsRequired();

            entity.HasOne(mr => mr.Migration)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<RsinConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Rsin)
                .HasMaxLength(9);
        });

        modelBuilder.Entity<StatusMapping>(entity =>
        {
            // Unique constraint: One DET status per zaaktype version can only map to one OZ status
            entity.HasIndex(e => new { e.ZaaktypeVersieMappingId, e.DetStatusNaam })
                .IsUnique()
                .HasDatabaseName("IX_StatusMapping_ZaaktypeVersieMappingId_DetStatusNaam_Unique");
        });

        modelBuilder.Entity<DocumentstatusMapping>(entity =>
        {
            // Unique constraint: Each DET document status can only map to one OZ document status
            entity.HasIndex(e => e.DetDocumentstatus)
                .IsUnique()
                .HasDatabaseName("IX_DocumentstatusMapping_DetDocumentstatus_Unique");
        });

        modelBuilder.Entity<BesluittypeMapping>(entity =>
        {
            // Unique constraint: One DET besluittype per zaaktype version can only map to one OZ besluittype
            entity.HasIndex(e => new { e.ZaaktypeVersieMappingId, e.DetBesluittypeNaam })
                .IsUnique()
                .HasDatabaseName("IX_BesluittypeMapping_ZaaktypeVersieMappingId_DetBesluittypeNaam_Unique");
        });

        modelBuilder.Entity<VertrouwelijkheidMapping>(entity =>
        {
            entity.HasIndex(e => new { e.ZaaktypeVersieMappingId, e.DetVertrouwelijkheid })
                .IsUnique()
                .HasDatabaseName("IX_VertrouwelijkheidMapping_ZaaktypeVersieMappingId_DetVertrouwelijkheid_Unique");
        });

        modelBuilder.Entity<PdfInformatieobjecttypeMapping>(entity =>
        {
            entity.HasIndex(e => e.ZaaktypeVersieMappingId)
                .IsUnique()
                .HasDatabaseName("IX_PdfInformatieobjecttypeMapping_ZaaktypeVersieMappingId_Unique");
        });

        modelBuilder.Entity<PublicatieNiveauMapping>(entity =>
        {
            entity.HasIndex(e => new { e.ZaaktypeVersieMappingId, e.DetPublicatieNiveau })
                .IsUnique()
                .HasDatabaseName("IX_PublicatieNiveauMapping_ZaaktypeVersieMappingId_DetPublicatieNiveau_Unique");
        });

        modelBuilder.Entity<DocumenttypeMapping>(entity =>
        {
            entity.HasIndex(e => new { e.ZaaktypeVersieMappingId, e.DetDocumenttypeNaam })
                .IsUnique()
                .HasDatabaseName("IX_DocumenttypeMapping_ZaaktypeVersieMappingId_DetDocumenttypeNaam_Unique");
        });

        modelBuilder.Entity<RoltypeMapping>(entity =>
        {
            entity.HasIndex(e => new { e.ZaaktypeVersieMappingId, e.DetRol })
                .IsUnique()
                .HasDatabaseName("IX_RoltypeMapping_ZaaktypeVersieMappingId_DetRol_Unique");
        });
    }

    public DbSet<ZaaktypenMapping> Mappings { get; set; }
    public DbSet<ZaaktypeVersieMapping> ZaaktypeVersieMappings { get; set; }
    public DbSet<ResultaattypeMapping> ResultaattypeMappings { get; set; }
    public DbSet<Migration> Migrations { get; set; }
    public DbSet<MigrationRecord> MigrationRecords { get; set; }
    public DbSet<RsinConfiguration> RsinConfigurations { get; set; }
    public DbSet<StatusMapping> StatusMappings { get; set; }
    public DbSet<BesluittypeMapping> BesluittypeMappings { get; set; }
    public DbSet<DocumentstatusMapping> DocumentstatusMappings { get; set; }
    public DbSet<VertrouwelijkheidMapping> VertrouwelijkheidMappings { get; set; }
    public DbSet<PdfInformatieobjecttypeMapping> PdfInformatieobjecttypeMappings { get; set; }
    public DbSet<PublicatieNiveauMapping> PublicatieNiveauMappings { get; set; }
    public DbSet<DocumenttypeMapping> DocumenttypeMappings { get; set; }
    public DbSet<RoltypeMapping> RoltypeMappings { get; set; }
}
