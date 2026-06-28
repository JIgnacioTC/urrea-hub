using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrreaHub.Domain.Catalogos;

namespace UrreaHub.Infrastructure.Persistence.Configurations;

public class CatalogoEstadoConfiguration : IEntityTypeConfiguration<CatalogoEstado>
{
    public void Configure(EntityTypeBuilder<CatalogoEstado> builder)
    {
        builder.ToTable("Estados", "Catalogos");
        builder.HasIndex(e => e.Codigo).IsUnique();
        builder.Property(e => e.Codigo).HasMaxLength(10).IsRequired();
        builder.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Pais).HasMaxLength(3).HasDefaultValue("MEX");
    }
}

public class CatalogoMunicipioConfiguration : IEntityTypeConfiguration<CatalogoMunicipio>
{
    public void Configure(EntityTypeBuilder<CatalogoMunicipio> builder)
    {
        builder.ToTable("Municipios", "Catalogos");
        builder.HasIndex(m => new { m.EstadoId, m.Codigo }).IsUnique();
        builder.Property(m => m.Codigo).HasMaxLength(10).IsRequired();
        builder.Property(m => m.Nombre).HasMaxLength(150).IsRequired();

        builder.HasOne(m => m.Estado)
            .WithMany(e => e.Municipios)
            .HasForeignKey(m => m.EstadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CatalogoEstadoCivilConfiguration : IEntityTypeConfiguration<CatalogoEstadoCivil>
{
    public void Configure(EntityTypeBuilder<CatalogoEstadoCivil> builder)
    {
        builder.ToTable("EstadosCiviles", "Catalogos");
        builder.HasIndex(e => e.Codigo).IsUnique();
        builder.Property(e => e.Codigo).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Nombre).HasMaxLength(80).IsRequired();
    }
}

public class RazonTerminoConfiguration : IEntityTypeConfiguration<RazonTermino>
{
    public void Configure(EntityTypeBuilder<RazonTermino> builder)
    {
        builder.ToTable("RazonesTermino", "Catalogos");
        builder.HasIndex(r => r.Codigo).IsUnique();
        builder.Property(r => r.Codigo).HasMaxLength(30).IsRequired();
        builder.Property(r => r.Nombre).HasMaxLength(150).IsRequired();
    }
}

public class RegistroPatronalConfiguration : IEntityTypeConfiguration<RegistroPatronal>
{
    public void Configure(EntityTypeBuilder<RegistroPatronal> builder)
    {
        builder.ToTable("RegistrosPatronales", "Catalogos");
        builder.HasIndex(r => r.Codigo).IsUnique();
        builder.HasIndex(r => r.NumeroImss).IsUnique();
        builder.Property(r => r.Codigo).HasMaxLength(30).IsRequired();
        builder.Property(r => r.NumeroImss).HasMaxLength(20).IsRequired();
        builder.Property(r => r.RazonSocial).HasMaxLength(250).IsRequired();
        builder.Property(r => r.Rfc).HasMaxLength(13);

        builder.HasOne(r => r.Estado)
            .WithMany()
            .HasForeignKey(r => r.EstadoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class CatalogoJerarquiaConfiguration : IEntityTypeConfiguration<CatalogoJerarquia>
{
    public void Configure(EntityTypeBuilder<CatalogoJerarquia> builder)
    {
        builder.ToTable("Jerarquias", "Catalogos");
        builder.HasIndex(j => j.Codigo).IsUnique();
        builder.Property(j => j.Codigo).HasMaxLength(30).IsRequired();
        builder.Property(j => j.Nombre).HasMaxLength(100).IsRequired();
    }
}
