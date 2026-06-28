using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrreaHub.Domain.Compensaciones;

namespace UrreaHub.Infrastructure.Persistence.Configurations;

public class CompensacionConfigurations :
    IEntityTypeConfiguration<SolicitudAjusteCompensacion>,
    IEntityTypeConfiguration<AprobacionAjusteCompensacion>,
    IEntityTypeConfiguration<HistorialAjusteCompensacion>,
    IEntityTypeConfiguration<Tabulador>,
    IEntityTypeConfiguration<BandaSalarial>,
    IEntityTypeConfiguration<ConceptoCompensacion>
{
    public void Configure(EntityTypeBuilder<SolicitudAjusteCompensacion> builder) => builder.ToTable("SolicitudesAjuste", "Compensaciones");
    public void Configure(EntityTypeBuilder<AprobacionAjusteCompensacion> builder) => builder.ToTable("Aprobaciones", "Compensaciones");
    public void Configure(EntityTypeBuilder<HistorialAjusteCompensacion> builder) => builder.ToTable("Historial", "Compensaciones");
    public void Configure(EntityTypeBuilder<Tabulador> builder) => builder.ToTable("Tabuladores", "Compensaciones");
    public void Configure(EntityTypeBuilder<BandaSalarial> builder) => builder.ToTable("BandasSalariales", "Compensaciones");
    public void Configure(EntityTypeBuilder<ConceptoCompensacion> builder) => builder.ToTable("Conceptos", "Compensaciones");
}
