using Datamigratie.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Data;

public class DatamigratieDbContext(DbContextOptions options) : DbContext(options)
{
    public const int MaxLengthForIndexProperties = 256;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ZaaktypeMapping>(entity =>
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
    }

    public required DbSet<ZaaktypeMapping> Mappings { get; set; }
}
