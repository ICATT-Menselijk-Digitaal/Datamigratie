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
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.OzZaaknummer)
                .HasMaxLength(100);
            
            entity.Property(e => e.IsSuccessful)
                .IsRequired();
            
            entity.Property(e => e.ErrorTitle)
                .HasMaxLength(500);
            
            entity.Property(e => e.ErrorDetails)
                .HasMaxLength(2000);
            
            entity.Property(e => e.ProcessedAt)
                .IsRequired();
            
            entity.HasOne(mr => mr.Migration)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            
        });
    }

    public DbSet<ZaaktypenMapping> Mappings { get; set; }
    public DbSet<Migration> Migrations { get; set; }
    public DbSet<MigrationRecord> MigrationRecords { get; set; }
}
