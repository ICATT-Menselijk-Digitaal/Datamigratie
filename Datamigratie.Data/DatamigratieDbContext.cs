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

        modelBuilder.Entity<ResultaattypeMapping>(entity =>
        {
            entity.HasKey(m => m.Id);

            // Required fields (non-nullable)
            entity.Property(m => m.DetZaaktypeId)
                .IsRequired();

            entity.Property(m => m.DetResultaattypeId)
                .IsRequired();

            entity.Property(m => m.OzZaaktypeId)
                .IsRequired();

            entity.Property(m => m.OzResultaattypeId)
                .IsRequired();

            // Unique constraint: One DET Resultaattype (per zaaktype) can only map to one OZ Resultaattype
            entity.HasIndex(m => new { m.DetZaaktypeId, m.DetResultaattypeId })
                .IsUnique()
                .HasDatabaseName("IX_ResultaattypeMapping_DetZaaktypeId_DetResultaattypeId_Unique");
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
    }

    public DbSet<ZaaktypenMapping> Mappings { get; set; }
    public DbSet<ResultaattypeMapping> ResultaattypeMappings { get; set; }
    public DbSet<Migration> Migrations { get; set; }
    public DbSet<MigrationRecord> MigrationRecords { get; set; }
}
