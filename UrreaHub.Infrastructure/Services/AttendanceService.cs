using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Asistencia;
using UrreaHub.Application.Common;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Domain.Asistencia;
using UrreaHub.Domain.Common;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class AttendanceService : IAttendanceService
{
    private readonly UrreaHubDbContext _context;
    private readonly IAuditService _audit;

    public AttendanceService(UrreaHubDbContext context, IAuditService audit)
    {
        _context = context;
        _audit = audit;
    }

    public async Task<AttendanceSummaryDto> GetMySummaryAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var inicioMes = new DateTime(today.Year, today.Month, 1);

        var registroHoy = await _context.RegistrosAsistencia.AsNoTracking()
            .FirstOrDefaultAsync(r => r.ColaboradorId == colaboradorId && r.Fecha == today && r.IsActive, cancellationToken);

        var retardos = await _context.IncidenciasAsistencia.CountAsync(i =>
            i.ColaboradorId == colaboradorId && i.Tipo == TipoIncidenciaAsistencia.Retardo &&
            i.Fecha >= inicioMes && i.IsActive, cancellationToken);

        var ausencias = await _context.IncidenciasAsistencia.CountAsync(i =>
            i.ColaboradorId == colaboradorId &&
            i.Tipo == TipoIncidenciaAsistencia.AusenciaInjustificada &&
            i.Fecha >= inicioMes && i.IsActive, cancellationToken);

        var correccionesPendientes = await _context.CorreccionesAsistencia.CountAsync(c =>
            c.SolicitanteId == colaboradorId &&
            c.Estado == EstadoCorreccionAsistencia.Solicitada && c.IsActive, cancellationToken);

        var historial = await _context.RegistrosAsistencia.AsNoTracking()
            .Where(r => r.ColaboradorId == colaboradorId && r.IsActive)
            .OrderByDescending(r => r.Fecha)
            .Take(14)
            .ToListAsync(cancellationToken);

        var colaborador = await _context.Colaboradores.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == colaboradorId, cancellationToken);
        var puedenChecarRemotamente = colaborador?.PuedenChecarRemotamente ?? false;

        return new AttendanceSummaryDto(
            registroHoy is null ? null : MapRegistro(registroHoy),
            retardos,
            ausencias,
            correccionesPendientes,
            historial.Select(MapRegistro).ToList(),
            puedenChecarRemotamente);
    }

    public async Task<IReadOnlyList<RegistroAsistenciaDto>> GetMyRecordsAsync(Guid colaboradorId, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default)
    {
        var query = _context.RegistrosAsistencia.AsNoTracking()
            .Where(r => r.ColaboradorId == colaboradorId && r.IsActive);
        if (desde.HasValue) query = query.Where(r => r.Fecha >= desde.Value.Date);
        if (hasta.HasValue) query = query.Where(r => r.Fecha <= hasta.Value.Date);
        var items = await query.OrderByDescending(r => r.Fecha).ToListAsync(cancellationToken);
        return items.Select(MapRegistro).ToList();
    }

    public async Task<Result<RegistroAsistenciaDto>> CheckInAsync(Guid colaboradorId, CheckInOutDto dto, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var now = DateTime.UtcNow;

        var existing = await _context.RegistrosAsistencia
            .FirstOrDefaultAsync(r => r.ColaboradorId == colaboradorId && r.Fecha == today && r.IsActive, cancellationToken);

        if (existing?.HoraEntrada != null && existing.HoraSalida == null)
            return Result<RegistroAsistenciaDto>.Fail("Ya tienes una entrada activa hoy.");

        if (existing?.HoraEntrada != null && existing.HoraSalida != null)
            return Result<RegistroAsistenciaDto>.Fail("Ya completaste tu registro de hoy.");

        var tipoRegistro = dto.ClienteComercial != null ? "Comercial" : "Oficina";
        if (existing is null)
        {
            existing = new RegistroAsistencia
            {
                Id = Guid.NewGuid(),
                ColaboradorId = colaboradorId,
                Fecha = today,
                HoraEntrada = now,
                Fuente = dto.Fuente,
                TipoRegistro = tipoRegistro,
                Estado = EstadoRegistroAsistencia.EntradaSinSalida,
                LatitudEntrada = dto.Latitud,
                LongitudEntrada = dto.Longitud,
                ClienteComercial = dto.ClienteComercial,
                UbicacionComercial = dto.UbicacionComercial,
                Observaciones = dto.Observaciones,
                CreatedAt = now,
                IsActive = true,
            };
            _context.RegistrosAsistencia.Add(existing);
        }
        else
        {
            existing.HoraEntrada = now;
            existing.Fuente = dto.Fuente;
            existing.LatitudEntrada = dto.Latitud;
            existing.LongitudEntrada = dto.Longitud;
            existing.UpdatedAt = now;
        }

        await EvaluateTardinessAsync(existing, colaboradorId, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Asistencia", "CheckIn", "RegistroAsistencia", existing.Id, colaboradorId.ToString(), null, cancellationToken);

        return Result<RegistroAsistenciaDto>.Ok(MapRegistro(existing));
    }

    public async Task<Result<RegistroAsistenciaDto>> CheckOutAsync(Guid colaboradorId, CheckInOutDto dto, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var now = DateTime.UtcNow;

        var registro = await _context.RegistrosAsistencia
            .FirstOrDefaultAsync(r => r.ColaboradorId == colaboradorId && r.Fecha == today && r.IsActive, cancellationToken);

        if (registro?.HoraEntrada is null)
            return Result<RegistroAsistenciaDto>.Fail("Debes registrar entrada antes de la salida.");

        if (registro.HoraSalida != null)
            return Result<RegistroAsistenciaDto>.Fail("Ya registraste salida hoy.");

        registro.HoraSalida = now;
        registro.LatitudSalida = dto.Latitud;
        registro.LongitudSalida = dto.Longitud;
        registro.Estado = EstadoRegistroAsistencia.Completo;
        registro.UpdatedAt = now;

        await EvaluateEarlyDepartureAndOvertimeAsync(registro, colaboradorId, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Asistencia", "CheckOut", "RegistroAsistencia", registro.Id, colaboradorId.ToString(), null, cancellationToken);

        return Result<RegistroAsistenciaDto>.Ok(MapRegistro(registro));
    }

    public async Task<Result<CorreccionDto>> CreateCorrectionAsync(Guid colaboradorId, CrearCorreccionDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Motivo))
            return Result<CorreccionDto>.Fail("El motivo es obligatorio.");

        var fecha = dto.Fecha.Date;
        var registro = dto.RegistroId.HasValue
            ? await _context.RegistrosAsistencia.FirstOrDefaultAsync(r => r.Id == dto.RegistroId.Value, cancellationToken)
            : await _context.RegistrosAsistencia.FirstOrDefaultAsync(r => r.ColaboradorId == colaboradorId && r.Fecha == fecha && r.IsActive, cancellationToken);

        var incidencia = new IncidenciaAsistencia
        {
            Id = Guid.NewGuid(),
            ColaboradorId = colaboradorId,
            RegistroId = registro?.Id,
            Fecha = fecha,
            Tipo = MapCorreccionToIncidencia(dto.TipoCorreccion),
            Estado = EstadoIncidenciaAsistencia.PendienteValidacion,
            Descripcion = $"Corrección solicitada: {dto.TipoCorreccion}",
            RequiereValidacion = true,
            GeneraPrenomina = false,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };
        _context.IncidenciasAsistencia.Add(incidencia);

        var correccion = new CorreccionAsistencia
        {
            Id = Guid.NewGuid(),
            IncidenciaId = incidencia.Id,
            SolicitanteId = colaboradorId,
            TipoCorreccion = dto.TipoCorreccion,
            HoraEntradaSolicitada = dto.HoraEntradaSolicitada,
            HoraSalidaSolicitada = dto.HoraSalidaSolicitada,
            Motivo = dto.Motivo.Trim(),
            EvidenciaRef = dto.EvidenciaRef,
            Estado = EstadoCorreccionAsistencia.Solicitada,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };
        _context.CorreccionesAsistencia.Add(correccion);

        if (registro != null)
            registro.Estado = EstadoRegistroAsistencia.ConIncidencia;

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Asistencia", "CorreccionSolicitada", "CorreccionAsistencia", correccion.Id, colaboradorId.ToString(), dto.Motivo, cancellationToken);

        return Result<CorreccionDto>.Ok(await MapCorreccionAsync(correccion.Id, cancellationToken));
    }

    public async Task<IReadOnlyList<CorreccionDto>> GetMyCorrectionsAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var ids = await _context.CorreccionesAsistencia.AsNoTracking()
            .Where(c => c.SolicitanteId == colaboradorId && c.IsActive)
            .OrderByDescending(c => c.FechaSolicitud)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var result = new List<CorreccionDto>();
        foreach (var id in ids)
            result.Add(await MapCorreccionAsync(id, cancellationToken));
        return result;
    }

    public async Task<IReadOnlyList<CorreccionDto>> GetPendingCorrectionsAsync(Guid jefeId, CancellationToken cancellationToken = default)
    {
        var subIds = await _context.Colaboradores.AsNoTracking()
            .Where(c => c.JefeDirectoId == jefeId && c.IsActive)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var ids = await _context.CorreccionesAsistencia.AsNoTracking()
            .Where(c => subIds.Contains(c.SolicitanteId) && c.Estado == EstadoCorreccionAsistencia.Solicitada && c.IsActive)
            .OrderBy(c => c.FechaSolicitud)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var result = new List<CorreccionDto>();
        foreach (var id in ids)
            result.Add(await MapCorreccionAsync(id, cancellationToken));
        return result;
    }

    public async Task<Result<CorreccionDto>> ApproveCorrectionAsync(Guid aprobadorId, Guid correctionId, DecisionCorreccionDto dto, bool isRhAdmin, CancellationToken cancellationToken = default)
    {
        var correccion = await LoadCorreccionAsync(correctionId, cancellationToken);
        if (correccion is null) return Result<CorreccionDto>.Fail("Corrección no encontrada.");
        if (correccion.Estado != EstadoCorreccionAsistencia.Solicitada)
            return Result<CorreccionDto>.Fail("La corrección no está pendiente.");

        if (!await CanApproveAsync(aprobadorId, correccion.SolicitanteId, isRhAdmin, cancellationToken))
            return Result<CorreccionDto>.Fail("No autorizado para aprobar esta corrección.");

        correccion.Estado = EstadoCorreccionAsistencia.Aprobada;
        correccion.AprobadorId = aprobadorId;
        correccion.FechaDecision = DateTime.UtcNow;
        correccion.ComentarioDecision = dto.Comentario;
        correccion.UpdatedAt = DateTime.UtcNow;

        correccion.Incidencia.Estado = EstadoIncidenciaAsistencia.Justificada;
        correccion.Incidencia.UpdatedAt = DateTime.UtcNow;

        await ApplyCorrectionToRecordAsync(correccion, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogCambioEstadoAsync("CorreccionAsistencia", correccion.Id, "Solicitada", "Aprobada", aprobadorId.ToString(), dto.Comentario, cancellationToken);

        return Result<CorreccionDto>.Ok(await MapCorreccionAsync(correctionId, cancellationToken));
    }

    public async Task<Result<CorreccionDto>> RejectCorrectionAsync(Guid aprobadorId, Guid correctionId, DecisionCorreccionDto dto, bool isRhAdmin, CancellationToken cancellationToken = default)
    {
        var correccion = await LoadCorreccionAsync(correctionId, cancellationToken);
        if (correccion is null) return Result<CorreccionDto>.Fail("Corrección no encontrada.");

        if (!await CanApproveAsync(aprobadorId, correccion.SolicitanteId, isRhAdmin, cancellationToken))
            return Result<CorreccionDto>.Fail("No autorizado.");

        correccion.Estado = EstadoCorreccionAsistencia.Rechazada;
        correccion.AprobadorId = aprobadorId;
        correccion.FechaDecision = DateTime.UtcNow;
        correccion.ComentarioDecision = dto.Comentario;
        correccion.Incidencia.Estado = EstadoIncidenciaAsistencia.Rechazada;

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogCambioEstadoAsync("CorreccionAsistencia", correccion.Id, "Solicitada", "Rechazada", aprobadorId.ToString(), dto.Comentario, cancellationToken);

        return Result<CorreccionDto>.Ok(await MapCorreccionAsync(correctionId, cancellationToken));
    }

    public async Task<TeamAttendanceSummaryDto> GetTeamSummaryAsync(Guid jefeId, DateTime fecha, CancellationToken cancellationToken = default)
    {
        var subIds = await _context.Colaboradores.AsNoTracking()
            .Where(c => c.JefeDirectoId == jefeId && c.IsActive)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var day = fecha.Date;
        var registros = await _context.RegistrosAsistencia.AsNoTracking()
            .Where(r => subIds.Contains(r.ColaboradorId) && r.Fecha == day && r.IsActive)
            .ToListAsync(cancellationToken);

        var presentes = registros.Count(r => r.HoraEntrada != null);
        var ausentes = subIds.Count - presentes;

        var retardos = await _context.IncidenciasAsistencia.CountAsync(i =>
            subIds.Contains(i.ColaboradorId) && i.Fecha == day && i.Tipo == TipoIncidenciaAsistencia.Retardo && i.IsActive, cancellationToken);

        var salidasTempranas = await _context.IncidenciasAsistencia.CountAsync(i =>
            subIds.Contains(i.ColaboradorId) && i.Fecha == day && i.Tipo == TipoIncidenciaAsistencia.SalidaTemprana && i.IsActive, cancellationToken);

        var correcciones = await _context.CorreccionesAsistencia.CountAsync(c =>
            subIds.Contains(c.SolicitanteId) && c.Estado == EstadoCorreccionAsistencia.Solicitada && c.IsActive, cancellationToken);

        return new TeamAttendanceSummaryDto(
            presentes, ausentes, retardos, salidasTempranas, correcciones,
            registros.Select(MapRegistro).ToList());
    }

    public async Task<IReadOnlyList<IncidenciaDto>> GetTeamIncidentsAsync(Guid jefeId, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default)
    {
        var subIds = await _context.Colaboradores.AsNoTracking()
            .Where(c => c.JefeDirectoId == jefeId && c.IsActive)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var query = _context.IncidenciasAsistencia.AsNoTracking()
            .Include(i => i.Colaborador).ThenInclude(c => c.Departamento)
            .Where(i => subIds.Contains(i.ColaboradorId) && i.IsActive);

        if (desde.HasValue) query = query.Where(i => i.Fecha >= desde.Value.Date);
        if (hasta.HasValue) query = query.Where(i => i.Fecha <= hasta.Value.Date);

        return await query.OrderByDescending(i => i.Fecha).Select(i => new IncidenciaDto(
            i.Id, i.ColaboradorId,
            i.Colaborador.Nombre + " " + i.Colaborador.ApellidoPaterno,
            i.Colaborador.Departamento.Nombre,
            i.Fecha, i.Tipo.ToString(), i.Estado.ToString(), i.Descripcion, i.GeneraPrenomina, null
        )).ToListAsync(cancellationToken);
    }

    private async Task EvaluateTardinessAsync(RegistroAsistencia registro, Guid colaboradorId, CancellationToken cancellationToken)
    {
        if (registro.HoraEntrada is null) return;
        if (await HasApprovedAbsenceAsync(colaboradorId, registro.Fecha, cancellationToken)) return;

        var turno = await GetTurnoAsync(colaboradorId, registro.Fecha, cancellationToken);
        if (turno is null) return;

        var reglas = await GetReglasAsync(null, cancellationToken);
        var limite = turno.HoraEntrada.Add(TimeSpan.FromMinutes(turno.MinutosToleranciaEntrada + reglas.MinutosToleranciaRetardo));
        var horaLocal = registro.HoraEntrada.Value.TimeOfDay;

        if (horaLocal <= limite) return;

        await CreateIncidenciaAsync(colaboradorId, registro, TipoIncidenciaAsistencia.Retardo,
            $"Retardo: entrada {horaLocal:hh\\:mm}, esperada {turno.HoraEntrada:hh\\:mm}", reglas.GeneraIncidenciaNominaRetardo, cancellationToken);
    }

    private async Task EvaluateEarlyDepartureAndOvertimeAsync(RegistroAsistencia registro, Guid colaboradorId, CancellationToken cancellationToken)
    {
        if (registro.HoraEntrada is null || registro.HoraSalida is null) return;

        var turno = await GetTurnoAsync(colaboradorId, registro.Fecha, cancellationToken);
        if (turno is null) return;

        var salida = registro.HoraSalida.Value.TimeOfDay;
        var limiteSalida = turno.HoraSalida.Subtract(TimeSpan.FromMinutes(turno.MinutosToleranciaSalida));

        if (salida < limiteSalida)
        {
            await CreateIncidenciaAsync(colaboradorId, registro, TipoIncidenciaAsistencia.SalidaTemprana,
                $"Salida temprana: {salida:hh\\:mm}", false, cancellationToken);
        }
        else if (salida > turno.HoraSalida.Add(TimeSpan.FromMinutes(30)))
        {
            await CreateIncidenciaAsync(colaboradorId, registro, TipoIncidenciaAsistencia.HoraExtra,
                $"Hora extra detectada", true, cancellationToken);
        }
    }

    private async Task CreateIncidenciaAsync(Guid colaboradorId, RegistroAsistencia registro, TipoIncidenciaAsistencia tipo, string desc, bool prenomina, CancellationToken cancellationToken)
    {
        var exists = await _context.IncidenciasAsistencia.AnyAsync(i =>
            i.ColaboradorId == colaboradorId && i.RegistroId == registro.Id && i.Tipo == tipo && i.IsActive, cancellationToken);
        if (exists) return;

        _context.IncidenciasAsistencia.Add(new IncidenciaAsistencia
        {
            Id = Guid.NewGuid(),
            ColaboradorId = colaboradorId,
            RegistroId = registro.Id,
            Fecha = registro.Fecha,
            Tipo = tipo,
            Descripcion = desc,
            Estado = EstadoIncidenciaAsistencia.Detectada,
            GeneraPrenomina = prenomina,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        });
        registro.Estado = EstadoRegistroAsistencia.ConIncidencia;
    }

    private async Task ApplyCorrectionToRecordAsync(CorreccionAsistencia correccion, CancellationToken cancellationToken)
    {
        var registro = correccion.Incidencia.RegistroId.HasValue
            ? await _context.RegistrosAsistencia.FirstOrDefaultAsync(r => r.Id == correccion.Incidencia.RegistroId, cancellationToken)
            : await _context.RegistrosAsistencia.FirstOrDefaultAsync(r =>
                r.ColaboradorId == correccion.SolicitanteId && r.Fecha == correccion.Incidencia.Fecha && r.IsActive, cancellationToken);

        if (registro is null && (correccion.HoraEntradaSolicitada != null || correccion.HoraSalidaSolicitada != null))
        {
            registro = new RegistroAsistencia
            {
                Id = Guid.NewGuid(),
                ColaboradorId = correccion.SolicitanteId,
                Fecha = correccion.Incidencia.Fecha,
                Fuente = FuenteRegistroAsistencia.Manual,
                TipoRegistro = "Oficina",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };
            _context.RegistrosAsistencia.Add(registro);
            correccion.Incidencia.RegistroId = registro.Id;
        }

        if (registro is null) return;

        if (correccion.HoraEntradaSolicitada.HasValue)
            registro.HoraEntrada = correccion.HoraEntradaSolicitada;
        if (correccion.HoraSalidaSolicitada.HasValue)
            registro.HoraSalida = correccion.HoraSalidaSolicitada;

        registro.Estado = EstadoRegistroAsistencia.Validado;
        registro.UpdatedAt = DateTime.UtcNow;
    }

    private async Task<bool> HasApprovedAbsenceAsync(Guid colaboradorId, DateTime date, CancellationToken cancellationToken)
    {
        return await _context.SolicitudesAusencia.AsNoTracking().AnyAsync(s =>
            s.ColaboradorId == colaboradorId && s.IsActive &&
            s.Estado == EstadoSolicitud.Aprobada &&
            s.FechaInicio.Date <= date && s.FechaFin.Date >= date, cancellationToken);
    }

    private async Task<Turno?> GetTurnoAsync(Guid colaboradorId, DateTime date, CancellationToken cancellationToken)
    {
        var asignacion = await _context.AsignacionesTurno.AsNoTracking()
            .Include(a => a.Turno)
            .Where(a => a.ColaboradorId == colaboradorId && a.IsActive &&
                        a.FechaInicio <= date && (a.FechaFin == null || a.FechaFin >= date))
            .OrderByDescending(a => a.FechaInicio)
            .FirstOrDefaultAsync(cancellationToken);

        return asignacion?.Turno;
    }

    private async Task<ReglasAsistencia> GetReglasAsync(Guid? sedeId, CancellationToken cancellationToken)
    {
        var reglas = await _context.ReglasAsistencia.AsNoTracking()
            .FirstOrDefaultAsync(r => r.SedeId == sedeId && r.IsActive, cancellationToken)
            ?? await _context.ReglasAsistencia.AsNoTracking()
                .FirstOrDefaultAsync(r => r.SedeId == null && r.IsActive, cancellationToken);

        return reglas ?? new ReglasAsistencia { MinutosToleranciaRetardo = 10, MinutosParaFalta = 60 };
    }

    private async Task<bool> CanApproveAsync(Guid aprobadorId, Guid solicitanteId, bool isRhAdmin, CancellationToken cancellationToken)
    {
        if (isRhAdmin) return true;
        var col = await _context.Colaboradores.AsNoTracking().FirstOrDefaultAsync(c => c.Id == solicitanteId, cancellationToken);
        return col?.JefeDirectoId == aprobadorId;
    }

    private async Task<CorreccionAsistencia?> LoadCorreccionAsync(Guid id, CancellationToken cancellationToken)
        => await _context.CorreccionesAsistencia
            .Include(c => c.Incidencia).ThenInclude(i => i.Registro)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);

    private async Task<CorreccionDto> MapCorreccionAsync(Guid id, CancellationToken cancellationToken)
    {
        var c = await _context.CorreccionesAsistencia.AsNoTracking()
            .Include(x => x.Solicitante)
            .Include(x => x.Incidencia).ThenInclude(i => i.Registro)
            .FirstAsync(x => x.Id == id, cancellationToken);

        return new CorreccionDto(
            c.Id, c.IncidenciaId,
            $"{c.Solicitante.Nombre} {c.Solicitante.ApellidoPaterno}",
            c.Incidencia.Fecha,
            c.TipoCorreccion.ToString(),
            c.HoraEntradaSolicitada,
            c.HoraSalidaSolicitada,
            c.Motivo,
            c.Estado.ToString(),
            c.Incidencia.Registro?.HoraEntrada?.ToString("HH:mm"),
            c.Incidencia.Registro?.HoraSalida?.ToString("HH:mm"));
    }

    private static TipoIncidenciaAsistencia MapCorreccionToIncidencia(TipoCorreccionAsistencia t) => t switch
    {
        TipoCorreccionAsistencia.EntradaOmitida => TipoIncidenciaAsistencia.EntradaOmitida,
        TipoCorreccionAsistencia.SalidaOmitida => TipoIncidenciaAsistencia.SalidaOmitida,
        TipoCorreccionAsistencia.ErrorBiometrico => TipoIncidenciaAsistencia.ErrorBiometrico,
        TipoCorreccionAsistencia.TrabajoCampo or TipoCorreccionAsistencia.VisitaCliente => TipoIncidenciaAsistencia.TrabajoCampo,
        _ => TipoIncidenciaAsistencia.RegistroManual,
    };

    internal static RegistroAsistenciaDto MapRegistro(RegistroAsistencia r)
    {
        decimal? horas = null;
        if (r.HoraEntrada.HasValue && r.HoraSalida.HasValue)
            horas = (decimal)(r.HoraSalida.Value - r.HoraEntrada.Value).TotalHours;

        return new RegistroAsistenciaDto(
            r.Id, r.ColaboradorId, r.Fecha,
            r.HoraEntrada, r.HoraSalida,
            r.Fuente.ToString(), r.TipoRegistro, r.Estado.ToString(),
            horas, r.Observaciones);
    }

    public async Task<IReadOnlyList<TurnoDto>> GetAvailableShiftsAsync(CancellationToken cancellationToken = default)
    {
        var turnos = await _context.Turnos.AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.Nombre)
            .ToListAsync(cancellationToken);

        return turnos.Select(t => new TurnoDto(
            t.Id, t.Codigo, t.Nombre, t.HoraEntrada, t.HoraSalida, t.MinutosToleranciaEntrada, t.MinutosComida, t.IsActive
        )).ToList();
    }

    public async Task<IReadOnlyList<AsignacionTurnoDto>> GetMyShiftHistoryAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var asignaciones = await _context.AsignacionesTurno.AsNoTracking()
            .Include(a => a.Turno)
            .Include(a => a.Colaborador)
            .Where(a => a.ColaboradorId == colaboradorId && a.IsActive)
            .OrderByDescending(a => a.FechaInicio)
            .ToListAsync(cancellationToken);

        return asignaciones.Select(a => new AsignacionTurnoDto(
            a.Id,
            a.ColaboradorId,
            $"{a.Colaborador.Nombre} {a.Colaborador.ApellidoPaterno}",
            a.TurnoId,
            a.Turno.Nombre,
            a.FechaInicio,
            a.FechaFin,
            a.Origen
        )).ToList();
    }

    public async Task<Result<SolicitudCambioHorarioDto>> CreateShiftChangeRequestAsync(Guid colaboradorId, CrearSolicitudCambioHorarioDto dto, CancellationToken cancellationToken = default)
    {
        var col = await _context.Colaboradores.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == colaboradorId && c.IsActive, cancellationToken);
        if (col == null) return Result<SolicitudCambioHorarioDto>.Fail("Colaborador no encontrado.");

        var turnoSolicitado = await _context.Turnos.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == dto.TurnoSolicitadoId && t.IsActive, cancellationToken);
        if (turnoSolicitado == null) return Result<SolicitudCambioHorarioDto>.Fail("Turno solicitado no encontrado o inactivo.");

        var activeShift = await GetTurnoAsync(colaboradorId, DateTime.UtcNow.Date, cancellationToken);
        if (activeShift == null)
            return Result<SolicitudCambioHorarioDto>.Fail("No tienes un turno actualmente asignado.");

        if (activeShift.Id == turnoSolicitado.Id)
            return Result<SolicitudCambioHorarioDto>.Fail("El turno solicitado es el mismo que el actual.");

        var hasPending = await _context.SolicitudesCambioHorario.AnyAsync(s =>
            s.ColaboradorId == colaboradorId && s.Estado == "Pendiente" && s.IsActive, cancellationToken);
        if (hasPending)
            return Result<SolicitudCambioHorarioDto>.Fail("Ya tienes una solicitud de cambio de horario pendiente.");

        var request = new SolicitudCambioHorario
        {
            Id = Guid.NewGuid(),
            ColaboradorId = colaboradorId,
            TurnoActualId = activeShift.Id,
            TurnoSolicitadoId = turnoSolicitado.Id,
            Motivo = dto.Motivo.Trim(),
            Estado = "Pendiente",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.SolicitudesCambioHorario.Add(request);
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Asistencia", "SolicitudCambioHorarioCreada", "SolicitudCambioHorario", request.Id, colaboradorId.ToString(), dto.Motivo, cancellationToken);

        return Result<SolicitudCambioHorarioDto>.Ok(await MapSolicitudAsync(request.Id, cancellationToken));
    }

    public async Task<IReadOnlyList<SolicitudCambioHorarioDto>> GetMyShiftChangeRequestsAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var ids = await _context.SolicitudesCambioHorario.AsNoTracking()
            .Where(s => s.ColaboradorId == colaboradorId && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var result = new List<SolicitudCambioHorarioDto>();
        foreach (var id in ids)
            result.Add(await MapSolicitudAsync(id, cancellationToken));
        return result;
    }

    public async Task<IReadOnlyList<SolicitudCambioHorarioDto>> GetPendingShiftChangeRequestsAsync(Guid jefeId, CancellationToken cancellationToken = default)
    {
        var subIds = await _context.Colaboradores.AsNoTracking()
            .Where(c => c.JefeDirectoId == jefeId && c.IsActive)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var ids = await _context.SolicitudesCambioHorario.AsNoTracking()
            .Where(s => subIds.Contains(s.ColaboradorId) && s.Estado == "Pendiente" && s.IsActive)
            .OrderBy(s => s.CreatedAt)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var result = new List<SolicitudCambioHorarioDto>();
        foreach (var id in ids)
            result.Add(await MapSolicitudAsync(id, cancellationToken));
        return result;
    }

    public async Task<Result<SolicitudCambioHorarioDto>> ApproveShiftChangeRequestAsync(Guid aprobadorId, Guid requestId, DecisionCambioHorarioDto dto, bool isRhAdmin, CancellationToken cancellationToken = default)
    {
        var request = await _context.SolicitudesCambioHorario
            .Include(s => s.Colaborador)
            .FirstOrDefaultAsync(s => s.Id == requestId && s.IsActive, cancellationToken);

        if (request == null) return Result<SolicitudCambioHorarioDto>.Fail("Solicitud no encontrada.");
        if (request.Estado != "Pendiente") return Result<SolicitudCambioHorarioDto>.Fail("La solicitud no está pendiente.");

        if (!isRhAdmin && request.Colaborador.JefeDirectoId != aprobadorId)
            return Result<SolicitudCambioHorarioDto>.Fail("No autorizado para aprobar esta solicitud.");

        request.Estado = "Aprobado";
        request.AprobadorId = aprobadorId;
        request.FechaDecision = DateTime.UtcNow;
        request.ComentarioAprobador = dto.Comentario;
        request.UpdatedAt = DateTime.UtcNow;

        var today = DateTime.UtcNow.Date;
        
        var activeAssignments = await _context.AsignacionesTurno
            .Where(a => a.ColaboradorId == request.ColaboradorId && a.IsActive && a.FechaFin == null)
            .ToListAsync(cancellationToken);

        foreach (var a in activeAssignments)
        {
            a.FechaFin = today.AddDays(-1);
            a.UpdatedAt = DateTime.UtcNow;
        }

        var newAssignment = new AsignacionTurno
        {
            Id = Guid.NewGuid(),
            ColaboradorId = request.ColaboradorId,
            TurnoId = request.TurnoSolicitadoId,
            FechaInicio = today,
            Origen = "CambioAprobado",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        _context.AsignacionesTurno.Add(newAssignment);

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogCambioEstadoAsync("SolicitudCambioHorario", request.Id, "Pendiente", "Aprobado", aprobadorId.ToString(), dto.Comentario, cancellationToken);

        return Result<SolicitudCambioHorarioDto>.Ok(await MapSolicitudAsync(requestId, cancellationToken));
    }

    public async Task<Result<SolicitudCambioHorarioDto>> RejectShiftChangeRequestAsync(Guid aprobadorId, Guid requestId, DecisionCambioHorarioDto dto, bool isRhAdmin, CancellationToken cancellationToken = default)
    {
        var request = await _context.SolicitudesCambioHorario
            .Include(s => s.Colaborador)
            .FirstOrDefaultAsync(s => s.Id == requestId && s.IsActive, cancellationToken);

        if (request == null) return Result<SolicitudCambioHorarioDto>.Fail("Solicitud no encontrada.");
        if (request.Estado != "Pendiente") return Result<SolicitudCambioHorarioDto>.Fail("La solicitud no está pendiente.");

        if (!isRhAdmin && request.Colaborador.JefeDirectoId != aprobadorId)
            return Result<SolicitudCambioHorarioDto>.Fail("No autorizado para rechazar esta solicitud.");

        request.Estado = "Rechazado";
        request.AprobadorId = aprobadorId;
        request.FechaDecision = DateTime.UtcNow;
        request.ComentarioAprobador = dto.Comentario;
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogCambioEstadoAsync("SolicitudCambioHorario", request.Id, "Pendiente", "Rechazado", aprobadorId.ToString(), dto.Comentario, cancellationToken);

        return Result<SolicitudCambioHorarioDto>.Ok(await MapSolicitudAsync(requestId, cancellationToken));
    }

    private async Task<SolicitudCambioHorarioDto> MapSolicitudAsync(Guid id, CancellationToken cancellationToken)
    {
        var s = await _context.SolicitudesCambioHorario.AsNoTracking()
            .Include(x => x.Colaborador)
            .Include(x => x.TurnoActual)
            .Include(x => x.TurnoSolicitado)
            .Include(x => x.Aprobador)
            .FirstAsync(x => x.Id == id, cancellationToken);

        return new SolicitudCambioHorarioDto(
            s.Id,
            s.ColaboradorId,
            $"{s.Colaborador.Nombre} {s.Colaborador.ApellidoPaterno}",
            s.TurnoActualId,
            s.TurnoActual.Nombre,
            s.TurnoSolicitadoId,
            s.TurnoActual.Nombre == s.TurnoSolicitado.Nombre ? s.TurnoSolicitado.Nombre + " (Nuevo)" : s.TurnoSolicitado.Nombre,
            s.Motivo,
            s.Estado,
            s.ComentarioAprobador,
            s.AprobadorId,
            s.Aprobador != null ? $"{s.Aprobador.Nombre} {s.Aprobador.ApellidoPaterno}" : null,
            s.FechaDecision,
            s.CreatedAt);
    }

    public async Task<ChecadorResultDto> VerifyAndRegisterChecadorAsync(ChecadorVerifyDto dto, CancellationToken cancellationToken = default)
    {
        var cleanNumero = dto.NumeroEmpleado?.Trim();
        if (string.IsNullOrEmpty(cleanNumero))
        {
            return new ChecadorResultDto(false, "Número de empleado requerido.", null, null, null, null, null, null, null);
        }

        // Find active employee by EmployeeNumber
        var emp = await _context.Colaboradores
            .Include(c => c.Sede)
            .FirstOrDefaultAsync(c => c.NumeroEmpleado == cleanNumero && c.IsActive && c.FechaBaja == null, cancellationToken);

        if (emp == null)
        {
            return new ChecadorResultDto(false, "Colaborador no encontrado o inactivo.", null, null, cleanNumero, null, null, null, null);
        }

        var nombreCompleto = $"{emp.Nombre} {emp.ApellidoPaterno}".Trim();

        // 1. Check for approved leaves/absences today
        var today = DateTime.UtcNow.Date;
        var now = DateTime.UtcNow;

        var hasAbsence = await _context.SolicitudesAusencia
            .AnyAsync(s => s.ColaboradorId == emp.Id && 
                           s.IsActive && 
                           s.Estado == Domain.Common.EstadoSolicitud.Aprobada && 
                           s.FechaInicio <= today && 
                           s.FechaFin >= today, cancellationToken);

        if (hasAbsence)
        {
            return new ChecadorResultDto(false, "El colaborador cuenta con un permiso o ausencia activa el día de hoy.", null, nombreCompleto, cleanNumero, null, null, null, null);
        }

        // 2. Check shift assignment (AsignacionTurno)
        var shiftAssign = await _context.AsignacionesTurno
            .Include(a => a.Turno)
            .Where(a => a.ColaboradorId == emp.Id && a.IsActive && a.FechaInicio <= today && (a.FechaFin == null || a.FechaFin >= today))
            .OrderByDescending(a => a.FechaInicio)
            .FirstOrDefaultAsync(cancellationToken);

        if (shiftAssign == null)
        {
            return new ChecadorResultDto(false, "El colaborador no cuenta con un turno asignado.", null, nombreCompleto, cleanNumero, null, null, null, null);
        }

        var turno = shiftAssign.Turno;
        var dayOfWeek = today.DayOfWeek;
        bool appliesToday = false;
        switch (dayOfWeek)
        {
            case DayOfWeek.Monday: appliesToday = turno.AplicaLunes; break;
            case DayOfWeek.Tuesday: appliesToday = turno.AplicaMartes; break;
            case DayOfWeek.Wednesday: appliesToday = turno.AplicaMiercoles; break;
            case DayOfWeek.Thursday: appliesToday = turno.AplicaJueves; break;
            case DayOfWeek.Friday: appliesToday = turno.AplicaViernes; break;
            case DayOfWeek.Saturday: appliesToday = turno.AplicaSabado; break;
            case DayOfWeek.Sunday: appliesToday = turno.AplicaDomingo; break;
        }

        string? warning = null;
        if (!appliesToday)
        {
            return new ChecadorResultDto(false, "Hoy es día de descanso para el colaborador según su turno.", null, nombreCompleto, cleanNumero, null, null, turno.Nombre, $"{turno.HoraEntrada:hh\\:mm} - {turno.HoraSalida:hh\\:mm}");
        }

        // 3. Register Check-in or Check-out
        var existing = await _context.RegistrosAsistencia
            .FirstOrDefaultAsync(r => r.ColaboradorId == emp.Id && r.Fecha == today && r.IsActive, cancellationToken);

        string tipoReg = "Entrada";

        if (existing != null && existing.HoraEntrada != null && existing.HoraSalida != null)
        {
            return new ChecadorResultDto(false, "El colaborador ya registró su entrada y salida del día de hoy.", null, nombreCompleto, cleanNumero, null, null, turno.Nombre, $"{turno.HoraEntrada:hh\\:mm} - {turno.HoraSalida:hh\\:mm}");
        }

        if (existing == null)
        {
            existing = new RegistroAsistencia
            {
                Id = Guid.NewGuid(),
                ColaboradorId = emp.Id,
                Fecha = today,
                HoraEntrada = now,
                Fuente = FuenteRegistroAsistencia.Biometrico,
                TipoRegistro = "ChecadorSede",
                Estado = EstadoRegistroAsistencia.EntradaSinSalida,
                CreatedAt = now,
                IsActive = true
            };
            _context.RegistrosAsistencia.Add(existing);
            await EvaluateTardinessAsync(existing, emp.Id, cancellationToken);
            tipoReg = "Entrada";
        }
        else
        {
            existing.HoraSalida = now;
            existing.Estado = EstadoRegistroAsistencia.Completo;
            existing.UpdatedAt = now;
            await EvaluateEarlyDepartureAndOvertimeAsync(existing, emp.Id, cancellationToken);
            tipoReg = "Salida";
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Asistencia", "ChecadorSede", "RegistroAsistencia", existing.Id, emp.Id.ToString(), $"Tipo: {tipoReg}, SedeId: {dto.SedeId}", cancellationToken);

        return new ChecadorResultDto(
            true,
            null,
            warning,
            nombreCompleto,
            cleanNumero,
            tipoReg,
            now,
            turno.Nombre,
            $"{turno.HoraEntrada:hh\\:mm} - {turno.HoraSalida:hh\\:mm}"
        );
    }
}
