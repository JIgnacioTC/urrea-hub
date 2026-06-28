using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Asistencia;
using UrreaHub.Application.Common;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Domain.Asistencia;
using UrreaHub.Domain.Common;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class AttendanceAdminService : IAttendanceAdminService
{
    private readonly UrreaHubDbContext _context;
    private readonly IAuditService _audit;

    public AttendanceAdminService(UrreaHubDbContext context, IAuditService audit)
    {
        _context = context;
        _audit = audit;
    }

    public async Task<AttendanceDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var registros = await _context.RegistrosAsistencia.AsNoTracking()
            .Where(r => r.Fecha == today && r.IsActive)
            .ToListAsync(cancellationToken);

        var activos = await _context.Colaboradores.CountAsync(c => c.IsActive, cancellationToken);
        var presentes = registros.Count(r => r.HoraEntrada != null);

        var retardos = await _context.IncidenciasAsistencia.CountAsync(i =>
            i.Fecha == today && i.Tipo == TipoIncidenciaAsistencia.Retardo && i.IsActive, cancellationToken);

        var salidasTempranas = await _context.IncidenciasAsistencia.CountAsync(i =>
            i.Fecha == today && i.Tipo == TipoIncidenciaAsistencia.SalidaTemprana && i.IsActive, cancellationToken);

        var horasExtra = await _context.IncidenciasAsistencia.CountAsync(i =>
            i.Fecha == today && i.Tipo == TipoIncidenciaAsistencia.HoraExtra && i.IsActive, cancellationToken);

        var correcciones = await _context.CorreccionesAsistencia.CountAsync(c =>
            c.Estado == EstadoCorreccionAsistencia.Solicitada && c.IsActive, cancellationToken);

        var nomina = await _context.IncidenciasNominaAsistencia.CountAsync(n =>
            n.Estado == EstadoIncidenciaNominaAsistencia.Pendiente && n.IsActive, cancellationToken);

        var comercialIds = await _context.Colaboradores.AsNoTracking()
            .Where(c => c.IsActive && c.Departamento.Nombre.Contains("Comercial"))
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var comercialConRegistro = await _context.RegistrosAsistencia.AsNoTracking()
            .Where(r => r.Fecha == today && r.TipoRegistro == "Comercial" && r.IsActive && comercialIds.Contains(r.ColaboradorId))
            .Select(r => r.ColaboradorId)
            .Distinct()
            .CountAsync(cancellationToken);

        var comercialSinReporte = comercialIds.Count - comercialConRegistro;

        var ultimoCorte = await _context.IncidenciasNominaAsistencia.AsNoTracking()
            .OrderByDescending(n => n.FechaGeneracion)
            .Select(n => (DateTime?)n.FechaGeneracion)
            .FirstOrDefaultAsync(cancellationToken);

        var incidenciasRecientes = await ListIncidentsAsync(null, today.AddDays(-7), today, cancellationToken);

        return new AttendanceDashboardDto(
            presentes, activos - presentes, retardos, salidasTempranas, horasExtra,
            correcciones, nomina, comercialSinReporte, ultimoCorte,
            incidenciasRecientes.Take(10).ToList());
    }

    public async Task<IReadOnlyList<RegistroAsistenciaDto>> ListRecordsAsync(DateTime? fecha, Guid? sedeId, Guid? departamentoId, CancellationToken cancellationToken = default)
    {
        var day = fecha?.Date ?? DateTime.UtcNow.Date;
        var query = _context.RegistrosAsistencia.AsNoTracking()
            .Include(r => r.Colaborador)
            .Where(r => r.Fecha == day && r.IsActive);

        if (sedeId.HasValue) query = query.Where(r => r.Colaborador.SedeId == sedeId);
        if (departamentoId.HasValue) query = query.Where(r => r.Colaborador.DepartamentoId == departamentoId);

        var items = await query.OrderBy(r => r.Colaborador.ApellidoPaterno).ToListAsync(cancellationToken);
        return items.Select(AttendanceService.MapRegistro).ToList();
    }

    public async Task<IReadOnlyList<IncidenciaDto>> ListIncidentsAsync(EstadoIncidenciaAsistencia? estado, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default)
    {
        var query = _context.IncidenciasAsistencia.AsNoTracking()
            .Include(i => i.Colaborador).ThenInclude(c => c.Departamento)
            .Where(i => i.IsActive);

        if (estado.HasValue) query = query.Where(i => i.Estado == estado);
        if (desde.HasValue) query = query.Where(i => i.Fecha >= desde.Value.Date);
        if (hasta.HasValue) query = query.Where(i => i.Fecha <= hasta.Value.Date);

        return await query.OrderByDescending(i => i.Fecha).Select(i => new IncidenciaDto(
            i.Id, i.ColaboradorId,
            i.Colaborador.Nombre + " " + i.Colaborador.ApellidoPaterno,
            i.Colaborador.Departamento.Nombre,
            i.Fecha, i.Tipo.ToString(), i.Estado.ToString(), i.Descripcion, i.GeneraPrenomina, null
        )).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CorreccionDto>> ListCorrectionsAsync(EstadoCorreccionAsistencia? estado, CancellationToken cancellationToken = default)
    {
        var query = _context.CorreccionesAsistencia.AsNoTracking()
            .Include(c => c.Solicitante)
            .Include(c => c.Incidencia).ThenInclude(i => i.Registro)
            .Where(c => c.IsActive);

        if (estado.HasValue) query = query.Where(c => c.Estado == estado);

        return await query.OrderByDescending(c => c.FechaSolicitud).Select(c => new CorreccionDto(
            c.Id, c.IncidenciaId,
            c.Solicitante.Nombre + " " + c.Solicitante.ApellidoPaterno,
            c.Incidencia.Fecha,
            c.TipoCorreccion.ToString(),
            c.HoraEntradaSolicitada,
            c.HoraSalidaSolicitada,
            c.Motivo,
            c.Estado.ToString(),
            c.Incidencia.Registro != null && c.Incidencia.Registro.HoraEntrada != null ? c.Incidencia.Registro.HoraEntrada.Value.ToString("HH:mm") : null,
            c.Incidencia.Registro != null && c.Incidencia.Registro.HoraSalida != null ? c.Incidencia.Registro.HoraSalida.Value.ToString("HH:mm") : null
        )).ToListAsync(cancellationToken);
    }

    public async Task<int> GenerateDailyIncidentsAsync(DateTime? fecha, string performedBy, CancellationToken cancellationToken = default)
    {
        var day = fecha?.Date ?? DateTime.UtcNow.Date;
        if (day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) return 0;

        var colaboradores = await _context.Colaboradores.AsNoTracking()
            .Where(c => c.IsActive)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var count = 0;
        foreach (var id in colaboradores)
        {
            var hasRecord = await _context.RegistrosAsistencia.AnyAsync(r =>
                r.ColaboradorId == id && r.Fecha == day && r.IsActive, cancellationToken);
            if (hasRecord) continue;

            if (await HasApprovedAbsenceAsync(id, day, cancellationToken))
            {
                await EnsureIncidenciaAsync(id, null, day, TipoIncidenciaAsistencia.PermisoRelacionado,
                    "Ausencia justificada por permiso/vacación aprobada", false, EstadoIncidenciaAsistencia.Justificada, cancellationToken);
                count++;
                continue;
            }

            await EnsureIncidenciaAsync(id, null, day, TipoIncidenciaAsistencia.AusenciaInjustificada,
                "Sin registro de asistencia", true, EstadoIncidenciaAsistencia.NoJustificada, cancellationToken);
            count++;
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Asistencia", "GenerarIncidencias", "IncidenciaAsistencia", Guid.Empty, performedBy, $"Fecha {day:yyyy-MM-dd}", cancellationToken);
        return count;
    }

    public async Task<IReadOnlyList<IncidenciaNominaAsistenciaDto>> ListPayrollIncidentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.IncidenciasNominaAsistencia.AsNoTracking()
            .Include(n => n.Colaborador)
            .Where(n => n.IsActive)
            .OrderByDescending(n => n.FechaGeneracion)
            .Select(n => new IncidenciaNominaAsistenciaDto(
                n.Id, n.Colaborador.NumeroEmpleado,
                n.Colaborador.Nombre + " " + n.Colaborador.ApellidoPaterno,
                n.Periodo, n.TipoConcepto, n.Cantidad, n.Unidad, n.Estado.ToString(),
                n.FechaGeneracion, n.ValidadoPor, n.NominaSyncAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GeneratePayrollCutAsync(string periodo, string performedBy, CancellationToken cancellationToken = default)
    {
        var incidencias = await _context.IncidenciasAsistencia
            .Where(i => i.GeneraPrenomina && i.IsActive &&
                        i.Estado != EstadoIncidenciaAsistencia.Cancelada &&
                        i.Estado != EstadoIncidenciaAsistencia.AplicadaNomina)
            .ToListAsync(cancellationToken);

        var count = 0;
        foreach (var inc in incidencias)
        {
            var exists = await _context.IncidenciasNominaAsistencia.AnyAsync(n =>
                n.IncidenciaId == inc.Id && n.IsActive, cancellationToken);
            if (exists) continue;

            _context.IncidenciasNominaAsistencia.Add(new IncidenciaNominaAsistencia
            {
                Id = Guid.NewGuid(),
                IncidenciaId = inc.Id,
                ColaboradorId = inc.ColaboradorId,
                Periodo = periodo,
                TipoConcepto = inc.Tipo.ToString(),
                Cantidad = inc.Tipo == TipoIncidenciaAsistencia.AusenciaInjustificada ? 1 : 0.5m,
                Unidad = inc.Tipo == TipoIncidenciaAsistencia.AusenciaInjustificada ? "dias" : "horas",
                Estado = EstadoIncidenciaNominaAsistencia.ListaParaEnvio,
                ValidadoPor = performedBy,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            });
            inc.Estado = EstadoIncidenciaAsistencia.AplicadaNomina;
            count++;
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Asistencia", "GenerarCorteNomina", "IncidenciaNominaAsistencia", Guid.Empty, performedBy, periodo, cancellationToken);
        return count;
    }

    public async Task SendPayrollCutAsync(string performedBy, CancellationToken cancellationToken = default)
    {
        var pendientes = await _context.IncidenciasNominaAsistencia
            .Where(n => n.Estado == EstadoIncidenciaNominaAsistencia.ListaParaEnvio && n.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var n in pendientes)
        {
            n.Estado = EstadoIncidenciaNominaAsistencia.Enviada;
            n.FechaEnvioNomina = DateTime.UtcNow;
            n.NominaSyncAt = DateTime.UtcNow;
            n.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Asistencia", "EnviarNomina", "IncidenciaNominaAsistencia", Guid.Empty, performedBy, $"{pendientes.Count} registros", cancellationToken);
    }

    public async Task<IReadOnlyList<TurnoDto>> ListShiftsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Turnos.AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.Nombre)
            .Select(t => new TurnoDto(t.Id, t.Codigo, t.Nombre, t.HoraEntrada, t.HoraSalida, t.MinutosToleranciaEntrada, t.MinutosComida, t.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<TurnoDto> UpsertShiftAsync(Guid? id, UpsertTurnoDto dto, CancellationToken cancellationToken = default)
    {
        Turno entity;
        if (id.HasValue)
        {
            entity = await _context.Turnos.FirstAsync(t => t.Id == id.Value, cancellationToken);
        }
        else
        {
            entity = new Turno { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
            _context.Turnos.Add(entity);
        }

        entity.Codigo = dto.Codigo.Trim();
        entity.Nombre = dto.Nombre.Trim();
        entity.HoraEntrada = dto.HoraEntrada;
        entity.HoraSalida = dto.HoraSalida;
        entity.MinutosToleranciaEntrada = dto.MinutosToleranciaEntrada;
        entity.MinutosToleranciaSalida = dto.MinutosToleranciaSalida;
        entity.MinutosComida = dto.MinutosComida;
        entity.AplicaLunes = dto.AplicaLunes;
        entity.AplicaMartes = dto.AplicaMartes;
        entity.AplicaMiercoles = dto.AplicaMiercoles;
        entity.AplicaJueves = dto.AplicaJueves;
        entity.AplicaViernes = dto.AplicaViernes;
        entity.AplicaSabado = dto.AplicaSabado;
        entity.AplicaDomingo = dto.AplicaDomingo;
        entity.SedeId = dto.SedeId;
        entity.AreaId = dto.AreaId;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return new TurnoDto(entity.Id, entity.Codigo, entity.Nombre, entity.HoraEntrada, entity.HoraSalida, entity.MinutosToleranciaEntrada, entity.MinutosComida, entity.IsActive);
    }

    public async Task<IReadOnlyList<AsignacionTurnoDto>> ListShiftAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AsignacionesTurno.AsNoTracking()
            .Include(a => a.Colaborador)
            .Include(a => a.Turno)
            .Where(a => a.IsActive)
            .Select(a => new AsignacionTurnoDto(
                a.Id, a.ColaboradorId,
                a.Colaborador.Nombre + " " + a.Colaborador.ApellidoPaterno,
                a.TurnoId, a.Turno.Nombre, a.FechaInicio, a.FechaFin, a.Origen))
            .ToListAsync(cancellationToken);
    }

    public async Task<AsignacionTurnoDto> AssignShiftAsync(UpsertAsignacionTurnoDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new AsignacionTurno
        {
            Id = Guid.NewGuid(),
            ColaboradorId = dto.ColaboradorId,
            TurnoId = dto.TurnoId,
            FechaInicio = dto.FechaInicio.Date,
            FechaFin = dto.FechaFin?.Date,
            Origen = dto.Origen,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };
        _context.AsignacionesTurno.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        var col = await _context.Colaboradores.AsNoTracking().FirstAsync(c => c.Id == dto.ColaboradorId, cancellationToken);
        var turno = await _context.Turnos.AsNoTracking().FirstAsync(t => t.Id == dto.TurnoId, cancellationToken);
        return new AsignacionTurnoDto(entity.Id, entity.ColaboradorId, $"{col.Nombre} {col.ApellidoPaterno}", entity.TurnoId, turno.Nombre, entity.FechaInicio, entity.FechaFin, entity.Origen);
    }

    public async Task<ReglasAsistenciaDto> GetRulesAsync(Guid? sedeId, CancellationToken cancellationToken = default)
    {
        var r = await _context.ReglasAsistencia.AsNoTracking()
            .FirstOrDefaultAsync(x => x.SedeId == sedeId && x.IsActive, cancellationToken)
            ?? await _context.ReglasAsistencia.AsNoTracking()
                .FirstOrDefaultAsync(x => x.SedeId == null && x.IsActive, cancellationToken);

        if (r is null)
            return new ReglasAsistenciaDto(Guid.Empty, sedeId, 10, 60, true, true, true, false, 500);

        return MapReglas(r);
    }

    public async Task<ReglasAsistenciaDto> UpdateRulesAsync(ReglasAsistenciaDto dto, CancellationToken cancellationToken = default)
    {
        var entity = dto.Id != Guid.Empty
            ? await _context.ReglasAsistencia.FirstAsync(r => r.Id == dto.Id, cancellationToken)
            : await _context.ReglasAsistencia.FirstOrDefaultAsync(r => r.SedeId == dto.SedeId, cancellationToken);

        if (entity is null)
        {
            entity = new ReglasAsistencia { Id = Guid.NewGuid(), SedeId = dto.SedeId, CreatedAt = DateTime.UtcNow, IsActive = true };
            _context.ReglasAsistencia.Add(entity);
        }

        entity.MinutosToleranciaRetardo = dto.MinutosToleranciaRetardo;
        entity.MinutosParaFalta = dto.MinutosParaFalta;
        entity.GeneraIncidenciaNominaRetardo = dto.GeneraIncidenciaNominaRetardo;
        entity.RequiereValidacionLider = dto.RequiereValidacionLider;
        entity.PermitirRegistroMovil = dto.PermitirRegistroMovil;
        entity.RequiereGeolocalizacion = dto.RequiereGeolocalizacion;
        entity.RadioMetrosSede = dto.RadioMetrosSede;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return MapReglas(entity);
    }

    public async Task<Result<RegistroAsistenciaDto>> CreateManualRecordAsync(Guid colaboradorId, DateTime fecha, DateTime? entrada, DateTime? salida, string performedBy, CancellationToken cancellationToken = default)
    {
        var day = fecha.Date;
        var existing = await _context.RegistrosAsistencia
            .FirstOrDefaultAsync(r => r.ColaboradorId == colaboradorId && r.Fecha == day && r.IsActive, cancellationToken);

        if (existing != null)
            return Result<RegistroAsistenciaDto>.Fail("Ya existe registro para esa fecha.");

        var registro = new RegistroAsistencia
        {
            Id = Guid.NewGuid(),
            ColaboradorId = colaboradorId,
            Fecha = day,
            HoraEntrada = entrada,
            HoraSalida = salida,
            Fuente = FuenteRegistroAsistencia.Manual,
            TipoRegistro = "Oficina",
            Estado = salida.HasValue ? EstadoRegistroAsistencia.Completo : EstadoRegistroAsistencia.Manual,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };
        _context.RegistrosAsistencia.Add(registro);
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Asistencia", "RegistroManual", "RegistroAsistencia", registro.Id, performedBy, null, cancellationToken);

        return Result<RegistroAsistenciaDto>.Ok(AttendanceService.MapRegistro(registro));
    }

    public async Task<IReadOnlyList<RegistroAsistenciaDto>> ListCommercialRecordsAsync(DateTime? fecha, CancellationToken cancellationToken = default)
    {
        var day = fecha?.Date ?? DateTime.UtcNow.Date;
        var items = await _context.RegistrosAsistencia.AsNoTracking()
            .Where(r => r.Fecha == day && r.TipoRegistro == "Comercial" && r.IsActive)
            .ToListAsync(cancellationToken);
        return items.Select(AttendanceService.MapRegistro).ToList();
    }

    private async Task<bool> HasApprovedAbsenceAsync(Guid colaboradorId, DateTime date, CancellationToken cancellationToken)
        => await _context.SolicitudesAusencia.AsNoTracking().AnyAsync(s =>
            s.ColaboradorId == colaboradorId && s.IsActive &&
            s.Estado == EstadoSolicitud.Aprobada &&
            s.FechaInicio.Date <= date && s.FechaFin.Date >= date, cancellationToken);

    private async Task EnsureIncidenciaAsync(Guid colaboradorId, Guid? registroId, DateTime fecha, TipoIncidenciaAsistencia tipo, string desc, bool prenomina, EstadoIncidenciaAsistencia estado, CancellationToken cancellationToken)
    {
        var exists = await _context.IncidenciasAsistencia.AnyAsync(i =>
            i.ColaboradorId == colaboradorId && i.Fecha == fecha && i.Tipo == tipo && i.IsActive, cancellationToken);
        if (exists) return;

        _context.IncidenciasAsistencia.Add(new IncidenciaAsistencia
        {
            Id = Guid.NewGuid(),
            ColaboradorId = colaboradorId,
            RegistroId = registroId,
            Fecha = fecha,
            Tipo = tipo,
            Descripcion = desc,
            Estado = estado,
            GeneraPrenomina = prenomina,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        });
    }

    private static ReglasAsistenciaDto MapReglas(ReglasAsistencia r) => new(
        r.Id, r.SedeId, r.MinutosToleranciaRetardo, r.MinutosParaFalta,
        r.GeneraIncidenciaNominaRetardo, r.RequiereValidacionLider,
        r.PermitirRegistroMovil, r.RequiereGeolocalizacion, r.RadioMetrosSede);
}
