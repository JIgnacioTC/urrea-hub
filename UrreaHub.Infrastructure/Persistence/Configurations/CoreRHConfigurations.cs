using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Infrastructure.Persistence.Configurations;

public class ColaboradorConfiguration : IEntityTypeConfiguration<Colaborador>
{
    public void Configure(EntityTypeBuilder<Colaborador> builder)
    {
        builder.ToTable("Colaboradores", "CoreRH");
        builder.HasIndex(c => c.NumeroEmpleado).IsUnique();
        builder.HasIndex(c => c.Email).IsUnique();
        builder.HasIndex(c => new { c.ExternalSource, c.ExternalEmployeeId })
            .IsUnique()
            .HasFilter("[ExternalSource] IS NOT NULL AND [ExternalEmployeeId] IS NOT NULL");
        builder.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(c => c.ApellidoPaterno).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(200).IsRequired();
        builder.Property(c => c.ExternalSource).HasMaxLength(50);
        builder.Property(c => c.ExternalEmployeeId).HasMaxLength(100);
        builder.Property(c => c.ExternalSystemCode).HasMaxLength(50);
        builder.Property(c => c.SyncStatus).HasMaxLength(30).HasDefaultValue(EmployeeSyncStatus.Pending);
        builder.Property(c => c.SyncHash).HasMaxLength(128);
        builder.Property(c => c.NombrePreferido).HasMaxLength(100);

        builder.HasOne(c => c.DatosSensibles)
            .WithOne(d => d.Colaborador)
            .HasForeignKey<ColaboradorDatosSensibles>(d => d.ColaboradorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.DatosLaborales)
            .WithOne(d => d.Colaborador)
            .HasForeignKey<ColaboradorDatosLaborales>(d => d.ColaboradorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Movimientos)
            .WithOne(m => m.Colaborador)
            .HasForeignKey(m => m.ColaboradorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.JefeDirecto)
            .WithMany(c => c.Subordinados)
            .HasForeignKey(c => c.JefeDirectoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PuestoConfiguration : IEntityTypeConfiguration<Puesto>
{
    public void Configure(EntityTypeBuilder<Puesto> builder)
    {
        builder.ToTable("Puestos", "CoreRH");
        builder.HasIndex(p => p.Codigo).IsUnique();
        builder.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(p => p.PresupuestoAnual).HasPrecision(18, 2);
        builder.Property(p => p.Impacto).HasMaxLength(100);
        builder.Property(p => p.Comunicacion).HasMaxLength(100);
        builder.Property(p => p.Innovacion).HasMaxLength(100);
        builder.Property(p => p.EducacionRequerida).HasMaxLength(100);
    }
}

public class AreaConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.ToTable("Areas", "CoreRH");
        builder.HasIndex(a => a.Codigo).IsUnique();
    }
}

public class SubareaConfiguration : IEntityTypeConfiguration<Subarea>
{
    public void Configure(EntityTypeBuilder<Subarea> builder)
    {
        builder.ToTable("Subareas", "CoreRH");
        builder.HasIndex(s => s.Codigo).IsUnique();
        builder.HasOne(s => s.Area)
            .WithMany(a => a.Subareas)
            .HasForeignKey(s => s.AreaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
{
    public void Configure(EntityTypeBuilder<Departamento> builder)
    {
        builder.ToTable("Departamentos", "CoreRH");
        builder.HasIndex(d => d.Codigo).IsUnique();
        builder.HasOne(d => d.Subarea)
            .WithMany(s => s.Departamentos)
            .HasForeignKey(d => d.SubareaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SedeConfiguration : IEntityTypeConfiguration<Sede>
{
    public void Configure(EntityTypeBuilder<Sede> builder)
    {
        builder.ToTable("Sedes", "CoreRH");
        builder.HasIndex(s => s.Codigo).IsUnique();
    }
}

public class CentroCostoConfiguration : IEntityTypeConfiguration<CentroCosto>
{
    public void Configure(EntityTypeBuilder<CentroCosto> builder)
    {
        builder.ToTable("CentrosCosto", "CoreRH");
        builder.HasIndex(c => c.Codigo).IsUnique();
    }
}

public class RelacionLaboralConfiguration : IEntityTypeConfiguration<RelacionLaboral>
{
    public void Configure(EntityTypeBuilder<RelacionLaboral> builder)
    {
        builder.ToTable("RelacionesLaborales", "CoreRH");
        builder.HasIndex(r => r.Codigo).IsUnique();
    }
}

public class ColaboradorDatosSensiblesConfiguration : IEntityTypeConfiguration<ColaboradorDatosSensibles>
{
    public void Configure(EntityTypeBuilder<ColaboradorDatosSensibles> builder)
    {
        builder.ToTable("ColaboradoresDatosSensibles", "CoreRH");
        builder.HasIndex(d => d.ColaboradorId).IsUnique();
        builder.HasIndex(d => d.Rfc).IsUnique().HasFilter("[Rfc] IS NOT NULL");
        builder.Property(d => d.Rfc).HasMaxLength(20);
        builder.Property(d => d.Curp).HasMaxLength(30);
        builder.Property(d => d.Nss).HasMaxLength(30);
    }
}

public class ColaboradorDatosLaboralesConfiguration : IEntityTypeConfiguration<ColaboradorDatosLaborales>
{
    public void Configure(EntityTypeBuilder<ColaboradorDatosLaborales> builder)
    {
        builder.ToTable("ColaboradoresDatosLaborales", "CoreRH");
        builder.HasIndex(d => d.ColaboradorId).IsUnique();
    }
}

public class MovimientoColaboradorConfiguration : IEntityTypeConfiguration<MovimientoColaborador>
{
    public void Configure(EntityTypeBuilder<MovimientoColaborador> builder)
    {
        builder.ToTable("MovimientosColaborador", "CoreRH");
        builder.Property(m => m.TipoMovimiento).HasMaxLength(50).IsRequired();
        builder.Property(m => m.Origen).HasMaxLength(50).IsRequired();
        builder.Property(m => m.CreadoPor).HasMaxLength(150).IsRequired();
    }
}
