using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrreaHub.Domain.Plataforma;

namespace UrreaHub.Infrastructure.Persistence.Configurations;

public class MetadatoPlataformaConfiguration : IEntityTypeConfiguration<MetadatoPlataforma>
{
    public void Configure(EntityTypeBuilder<MetadatoPlataforma> builder)
    {
        builder.ToTable("Metadatos", "Plataforma");
        builder.HasIndex(m => m.CreatedAt);
        builder.HasIndex(m => new { m.Tipo, m.Origen });
        builder.Property(m => m.Etiqueta).HasMaxLength(200).IsRequired();
        builder.Property(m => m.VersionTag).HasMaxLength(100);
        builder.Property(m => m.Notas).HasMaxLength(1000);
        builder.Property(m => m.MigracionId).HasMaxLength(200);
        builder.Property(m => m.ContenidoJson).IsRequired();
    }
}

public class ConfiguracionGlobalConfiguration : IEntityTypeConfiguration<ConfiguracionGlobal>
{
    public void Configure(EntityTypeBuilder<ConfiguracionGlobal> builder)
    {
        builder.ToTable("ConfiguracionesGlobales", "Plataforma");
        builder.HasIndex(c => c.Clave).IsUnique();
        builder.Property(c => c.Clave).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Valor).IsRequired();
        builder.Property(c => c.Descripcion).HasMaxLength(1000);
    }
}
