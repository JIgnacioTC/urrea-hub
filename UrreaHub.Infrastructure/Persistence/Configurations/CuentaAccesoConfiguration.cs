using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Infrastructure.Persistence.Configurations;

public class CuentaAccesoConfiguration : IEntityTypeConfiguration<CuentaAcceso>
{
    public void Configure(EntityTypeBuilder<CuentaAcceso> builder)
    {
        builder.ToTable("CuentasAcceso", "CoreRH");
        builder.HasIndex(c => c.ColaboradorId).IsUnique();
        builder.Property(c => c.PasswordHash).HasMaxLength(200).IsRequired();

        builder.HasOne(c => c.Colaborador)
            .WithOne(c => c.CuentaAcceso)
            .HasForeignKey<CuentaAcceso>(c => c.ColaboradorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
