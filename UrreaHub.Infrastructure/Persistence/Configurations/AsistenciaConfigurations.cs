using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrreaHub.Domain.Asistencia;

namespace UrreaHub.Infrastructure.Persistence.Configurations;

public class AsistenciaConfigurations :
    IEntityTypeConfiguration<RegistroAsistencia>,
    IEntityTypeConfiguration<IncidenciaAsistencia>,
    IEntityTypeConfiguration<CorreccionAsistencia>,
    IEntityTypeConfiguration<Turno>,
    IEntityTypeConfiguration<AsignacionTurno>,
    IEntityTypeConfiguration<ReglasAsistencia>,
    IEntityTypeConfiguration<IncidenciaNominaAsistencia>,
    IEntityTypeConfiguration<SolicitudCambioHorario>
{
    public void Configure(EntityTypeBuilder<RegistroAsistencia> builder)
    {
        builder.ToTable("Registros", "Asistencia");
        builder.HasIndex(r => new { r.ColaboradorId, r.Fecha });
    }

    public void Configure(EntityTypeBuilder<IncidenciaAsistencia> builder) =>
        builder.ToTable("Incidencias", "Asistencia");

    public void Configure(EntityTypeBuilder<CorreccionAsistencia> builder) =>
        builder.ToTable("Correcciones", "Asistencia");

    public void Configure(EntityTypeBuilder<Turno> builder)
    {
        builder.ToTable("Turnos", "Asistencia");
        builder.HasIndex(t => t.Codigo).IsUnique();
    }

    public void Configure(EntityTypeBuilder<AsignacionTurno> builder) =>
        builder.ToTable("AsignacionesTurno", "Asistencia");

    public void Configure(EntityTypeBuilder<ReglasAsistencia> builder) =>
        builder.ToTable("Reglas", "Asistencia");

    public void Configure(EntityTypeBuilder<IncidenciaNominaAsistencia> builder) =>
        builder.ToTable("IncidenciasNomina", "Asistencia");

    public void Configure(EntityTypeBuilder<SolicitudCambioHorario> builder)
    {
        builder.ToTable("SolicitudesCambioHorario", "Asistencia");
        
        builder.HasOne(s => s.TurnoActual)
            .WithMany()
            .HasForeignKey(s => s.TurnoActualId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.TurnoSolicitado)
            .WithMany()
            .HasForeignKey(s => s.TurnoSolicitadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
