using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrreaHub.Domain.Seguridad;

namespace UrreaHub.Infrastructure.Persistence.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Roles", "Seguridad");
        builder.HasIndex(r => r.Codigo).IsUnique();
        builder.Property(r => r.Codigo).HasMaxLength(80).IsRequired();
        builder.Property(r => r.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(r => r.Descripcion).HasMaxLength(500);
    }
}

public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.ToTable("Permisos", "Seguridad");
        builder.HasIndex(p => p.Codigo).IsUnique();
        builder.Property(p => p.Codigo).HasMaxLength(120).IsRequired();
        builder.Property(p => p.Modulo).HasMaxLength(80).IsRequired();
        builder.Property(p => p.Nombre).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Descripcion).HasMaxLength(500);
    }
}

public class RolPermisoConfiguration : IEntityTypeConfiguration<RolPermiso>
{
    public void Configure(EntityTypeBuilder<RolPermiso> builder)
    {
        builder.ToTable("RolPermisos", "Seguridad");
        builder.HasIndex(rp => new { rp.RolId, rp.PermisoId }).IsUnique();

        builder.HasOne(rp => rp.Rol)
            .WithMany(r => r.RolPermisos)
            .HasForeignKey(rp => rp.RolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rp => rp.Permiso)
            .WithMany(p => p.RolPermisos)
            .HasForeignKey(rp => rp.PermisoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ColaboradorRolConfiguration : IEntityTypeConfiguration<ColaboradorRol>
{
    public void Configure(EntityTypeBuilder<ColaboradorRol> builder)
    {
        builder.ToTable("ColaboradorRoles", "Seguridad");
        builder.HasIndex(cr => new { cr.ColaboradorId, cr.RolId }).IsUnique();

        builder.HasOne(cr => cr.Colaborador)
            .WithMany()
            .HasForeignKey(cr => cr.ColaboradorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.Rol)
            .WithMany(r => r.ColaboradorRoles)
            .HasForeignKey(cr => cr.RolId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
