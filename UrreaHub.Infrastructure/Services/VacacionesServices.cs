using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Common;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Application.Vacaciones;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Vacaciones;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class CalendarioLaboralService : ICalendarioLaboralService
{
    private readonly UrreaHubDbContext _context;

    public CalendarioLaboralService(UrreaHubDbContext context) => _context = context;

    public async Task<IReadOnlySet<DateTime>> GetDiasInhabilesAsync(Guid? sedeId, int anio, CancellationToken cancellationToken = default)
    {
        var calendarios = await _context.CalendariosLaborales.AsNoTracking()
            .Include(c => c.DiasInhabiles)
            .Where(c => c.IsActive && c.Anio == anio)
            .Where(c => c.SedeId == null || c.SedeId == sedeId)
            .ToListAsync(cancellationToken);

        var dias = new HashSet<DateTime>();
        foreach (var cal in calendarios)
        {
            foreach (var d in cal.DiasInhabiles.Where(x => x.IsActive))
                dias.Add(d.Fecha.Date);
        }
        return dias;
    }
}

public class SaldoVacacionesService : ISaldoVacacionesService
{
    private readonly UrreaHubDbContext _context;

    public SaldoVacacionesService(UrreaHubDbContext context) => _context = context;

    public async Task<SaldoVacacionesDto?> GetMiSaldoAsync(Guid colaboradorId, int? anio, CancellationToken cancellationToken = default)
    {
        var year = anio ?? DateTime.UtcNow.Year;
        await EnsureSaldoAnualAsync(colaboradorId, year, cancellationToken);

        var saldo = await _context.SaldosVacaciones.AsNoTracking()
            .FirstOrDefaultAsync(s => s.ColaboradorId == colaboradorId && s.Anio == year && s.IsActive, cancellationToken);

        if (saldo is null) return null;

        var comprometidos = await GetDiasComprometidosAsync(colaboradorId, year, null, cancellationToken);
        var disponibles = saldo.DiasAsignados - saldo.DiasUsados - comprometidos;
        return new SaldoVacacionesDto(saldo.Anio, saldo.DiasAsignados, saldo.DiasUsados, comprometidos, disponibles);
    }

    public async Task<decimal> GetDiasComprometidosAsync(Guid colaboradorId, int anio, Guid? excludeSolicitudId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.SolicitudesAusencia.AsNoTracking()
            .Include(s => s.TipoAusencia)
            .Where(s => s.ColaboradorId == colaboradorId && s.IsActive &&
                        s.Estado == EstadoSolicitud.Pendiente &&
                        s.TipoAusencia.DescuentaSaldo &&
                        s.FechaInicio.Year == anio);

        if (excludeSolicitudId.HasValue)
            query = query.Where(s => s.Id != excludeSolicitudId.Value);

        return await query.SumAsync(s => s.DiasSolicitados, cancellationToken);
    }

    public async Task EnsureSaldoAnualAsync(Guid colaboradorId, int anio, CancellationToken cancellationToken = default)
    {
        var exists = await _context.SaldosVacaciones
            .AnyAsync(s => s.ColaboradorId == colaboradorId && s.Anio == anio, cancellationToken);
        if (exists) return;

        var politica = await _context.PoliticasVacaciones.AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.AntiguedadMinimaMeses)
            .FirstOrDefaultAsync(cancellationToken);

        if (politica is null) return;

        _context.SaldosVacaciones.Add(new SaldoVacaciones
        {
            Id = Guid.NewGuid(),
            ColaboradorId = colaboradorId,
            PoliticaId = politica.Id,
            Anio = anio,
            DiasAsignados = politica.DiasAnuales,
            DiasUsados = 0,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class SolicitudAusenciaService : ISolicitudAusenciaService
{
    private readonly UrreaHubDbContext _context;
    private readonly ICalendarioLaboralService _calendario;
    private readonly ISaldoVacacionesService _saldoService;
    private readonly INotificationSender _notifications;
    private readonly IAuditService _audit;

    public SolicitudAusenciaService(
        UrreaHubDbContext context,
        ICalendarioLaboralService calendario,
        ISaldoVacacionesService saldoService,
        INotificationSender notifications,
        IAuditService audit)
    {
        _context = context;
        _calendario = calendario;
        _saldoService = saldoService;
        _notifications = notifications;
        _audit = audit;
    }

    public async Task<decimal> CalcularDiasHabilesAsync(Guid colaboradorId, DateTime inicio, DateTime fin, CancellationToken cancellationToken = default)
    {
        var colaborador = await _context.Colaboradores.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == colaboradorId, cancellationToken);
        if (colaborador is null) return 0;

        var inhabiles = await _calendario.GetDiasInhabilesAsync(colaborador.SedeId, inicio.Year, cancellationToken);
        decimal dias = 0;
        for (var d = inicio.Date; d <= fin.Date; d = d.AddDays(1))
        {
            if (d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;
            if (inhabiles.Contains(d)) continue;
            dias++;
        }
        return dias;
    }

    public async Task<IReadOnlyList<TipoAusenciaDto>> GetTiposPermisoAsync(CancellationToken cancellationToken = default)
    {
        var tipos = await _context.TiposAusencia.AsNoTracking()
            .Where(t => t.IsActive && t.Categoria != CategoriaPermiso.Vacacion)
            .OrderBy(t => t.Orden)
            .ToListAsync(cancellationToken);
        return tipos.Select(MapTipoDto).ToList();
    }

    public async Task<IReadOnlyList<ResumenTipoPermisoDto>> GetResumenPermisosAsync(Guid colaboradorId, int? anio, CancellationToken cancellationToken = default)
    {
        var year = anio ?? DateTime.UtcNow.Year;
        var tipos = await _context.TiposAusencia.AsNoTracking()
            .Where(t => t.IsActive && t.Categoria != CategoriaPermiso.Vacacion)
            .OrderBy(t => t.Orden)
            .ToListAsync(cancellationToken);

        var usados = await _context.SolicitudesAusencia.AsNoTracking()
            .Where(s => s.ColaboradorId == colaboradorId && s.IsActive &&
                        s.FechaInicio.Year == year &&
                        s.Estado != EstadoSolicitud.Cancelada && s.Estado != EstadoSolicitud.Rechazada)
            .GroupBy(s => s.TipoAusenciaId)
            .Select(g => new { TipoId = g.Key, Total = g.Sum(x => x.DiasSolicitados) })
            .ToDictionaryAsync(x => x.TipoId, x => x.Total, cancellationToken);

        return tipos.Select(t =>
        {
            var usado = usados.GetValueOrDefault(t.Id, 0);
            var limite = t.DiasMaximosAnuales ?? t.DiasMaximosEvento;
            return new ResumenTipoPermisoDto(
                t.Id, t.Codigo, t.Nombre, usado,
                t.DiasMaximosAnuales, t.DiasMaximosEvento,
                limite.HasValue ? Math.Max(0, limite.Value - usado) : null);
        }).ToList();
    }

    public async Task<Result<SolicitudAusenciaDto>> CrearAsync(Guid colaboradorId, CrearSolicitudDto dto, CancellationToken cancellationToken = default)
    {
        var tipo = await _context.TiposAusencia.FirstOrDefaultAsync(t => t.Id == dto.TipoAusenciaId && t.IsActive, cancellationToken);
        if (tipo is null) return Result<SolicitudAusenciaDto>.Fail("Tipo de ausencia no válido.");

        var fechaInicio = dto.FechaInicio.Date;
        var fechaFin = dto.FechaFin.Date;
        if (fechaFin < fechaInicio)
            return Result<SolicitudAusenciaDto>.Fail("La fecha fin no puede ser anterior a la fecha inicio.");

        var parseInicio = ParseHora(dto.HoraInicio);
        var parseFin = ParseHora(dto.HoraFin);
        if (dto.HoraInicio is not null && parseInicio is null)
            return Result<SolicitudAusenciaDto>.Fail("Hora de inicio inválida. Use formato HH:mm.");
        if (dto.HoraFin is not null && parseFin is null)
            return Result<SolicitudAusenciaDto>.Fail("Hora de fin inválida. Use formato HH:mm.");

        var esDiaCompleto = dto.EsDiaCompleto && !tipo.EsParcial;

        if (tipo.EsParcial)
        {
            if (fechaInicio != fechaFin)
                return Result<SolicitudAusenciaDto>.Fail("Los permisos parciales aplican a un solo día.");
            if (tipo.Codigo == "ENTRADA_TARDE" && parseInicio is null)
                return Result<SolicitudAusenciaDto>.Fail("Indique la hora estimada de entrada.");
            if (tipo.Codigo == "SALIDA_TEMPRANO" && parseFin is null)
                return Result<SolicitudAusenciaDto>.Fail("Indique la hora de salida.");
            if (tipo.Codigo is not ("ENTRADA_TARDE" or "SALIDA_TEMPRANO") && parseInicio is null && parseFin is null)
                return Result<SolicitudAusenciaDto>.Fail("Indique el horario del permiso parcial.");
        }
        else if (!tipo.PermiteMultiDia && fechaInicio != fechaFin)
        {
            return Result<SolicitudAusenciaDto>.Fail("Este tipo de permiso solo permite un día.");
        }

        decimal dias;
        if (tipo.EsParcial)
            dias = 0.5m;
        else
        {
            dias = await CalcularDiasHabilesAsync(colaboradorId, fechaInicio, fechaFin, cancellationToken);
            if (dias <= 0) return Result<SolicitudAusenciaDto>.Fail("El rango de fechas no incluye días hábiles.");
        }

        var solicitud = new SolicitudAusencia
        {
            Id = Guid.NewGuid(),
            Folio = "PERM-" + DateTime.UtcNow.ToString("yyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper(),
            ColaboradorId = colaboradorId,
            TipoAusenciaId = dto.TipoAusenciaId,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            DiasSolicitados = dias,
            Comentario = dto.Comentario,
            Estado = dto.Enviar ? EstadoSolicitud.Pendiente : EstadoSolicitud.Borrador,
            EsDiaCompleto = esDiaCompleto,
            HoraInicio = parseInicio,
            HoraFin = parseFin,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        if (dto.Enviar)
        {
            var validation = await ValidarEnvioAsync(colaboradorId, solicitud, tipo, cancellationToken);
            if (!validation.Success) return validation;
        }

        _context.SolicitudesAusencia.Add(solicitud);
        await _context.SaveChangesAsync(cancellationToken);

        if (dto.Enviar)
        {
            await NotificarNuevaSolicitudAsync(solicitud.Id, cancellationToken);
            if (!string.IsNullOrEmpty(tipo.WebhookUrl))
                await EjecutarWebhookAsync(solicitud, tipo.WebhookUrl, cancellationToken);
        }

        return Result<SolicitudAusenciaDto>.Ok(await MapDtoAsync(solicitud.Id, cancellationToken));
    }

    public async Task<Result<SolicitudAusenciaDto>> EnviarAsync(Guid colaboradorId, Guid solicitudId, CancellationToken cancellationToken = default)
    {
        var solicitud = await GetOwnedSolicitudAsync(colaboradorId, solicitudId, cancellationToken);
        if (solicitud is null) return Result<SolicitudAusenciaDto>.Fail("Solicitud no encontrada.");
        if (solicitud.Estado != EstadoSolicitud.Borrador)
            return Result<SolicitudAusenciaDto>.Fail("Solo se pueden enviar solicitudes en borrador.");

        var tipo = await _context.TiposAusencia.FirstAsync(t => t.Id == solicitud.TipoAusenciaId, cancellationToken);
        var validation = await ValidarEnvioAsync(colaboradorId, solicitud, tipo, cancellationToken);
        if (!validation.Success) return validation;

        solicitud.Estado = EstadoSolicitud.Pendiente;
        solicitud.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogCambioEstadoAsync("SolicitudAusencia", solicitud.Id, "Borrador", "Pendiente", colaboradorId.ToString(), null, cancellationToken);
        await NotificarNuevaSolicitudAsync(solicitud.Id, cancellationToken);

        if (!string.IsNullOrEmpty(tipo.WebhookUrl))
            await EjecutarWebhookAsync(solicitud, tipo.WebhookUrl, cancellationToken);

        return Result<SolicitudAusenciaDto>.Ok(await MapDtoAsync(solicitud.Id, cancellationToken));
    }

    public async Task<Result<SolicitudAusenciaDto>> CancelarAsync(Guid colaboradorId, Guid solicitudId, CancellationToken cancellationToken = default)
    {
        var solicitud = await GetOwnedSolicitudAsync(colaboradorId, solicitudId, cancellationToken);
        if (solicitud is null) return Result<SolicitudAusenciaDto>.Fail("Solicitud no encontrada.");
        if (solicitud.Estado != EstadoSolicitud.Pendiente)
            return Result<SolicitudAusenciaDto>.Fail("Solo se pueden cancelar solicitudes pendientes.");

        var anterior = solicitud.Estado.ToString();
        solicitud.Estado = EstadoSolicitud.Cancelada;
        solicitud.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogCambioEstadoAsync("SolicitudAusencia", solicitud.Id, anterior, "Cancelada", colaboradorId.ToString(), null, cancellationToken);

        return Result<SolicitudAusenciaDto>.Ok(await MapDtoAsync(solicitud.Id, cancellationToken));
    }

    public async Task<Result<SolicitudAusenciaDto>> AprobarAsync(Guid aprobadorId, Guid solicitudId, AprobacionRequestDto dto, bool isRhAdmin, CancellationToken cancellationToken = default)
    {
        var solicitud = await _context.SolicitudesAusencia
            .Include(s => s.Colaborador)
            .Include(s => s.TipoAusencia)
            .FirstOrDefaultAsync(s => s.Id == solicitudId && s.IsActive, cancellationToken);

        if (solicitud is null) return Result<SolicitudAusenciaDto>.Fail("Solicitud no encontrada.");
        if (solicitud.Estado != EstadoSolicitud.Pendiente)
            return Result<SolicitudAusenciaDto>.Fail("La solicitud no está pendiente.");

        if (!await PuedeAprobarAsync(aprobadorId, solicitud, isRhAdmin, cancellationToken))
            return Result<SolicitudAusenciaDto>.Fail("No autorizado para aprobar esta solicitud.");

        solicitud.Estado = EstadoSolicitud.Aprobada;
        solicitud.UpdatedAt = DateTime.UtcNow;

        _context.AprobacionesAusencia.Add(new AprobacionAusencia
        {
            Id = Guid.NewGuid(),
            SolicitudId = solicitud.Id,
            AprobadorId = aprobadorId,
            Decision = EstadoSolicitud.Aprobada,
            Comentario = dto.Comentario,
            FechaDecision = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });

        if (solicitud.TipoAusencia.DescuentaSaldo)
            await AplicarSaldoAsync(solicitud, cancellationToken);

        await CrearIncidenciaNominaAsync(solicitud, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogCambioEstadoAsync("SolicitudAusencia", solicitud.Id, "Pendiente", "Aprobada", aprobadorId.ToString(), dto.Comentario, cancellationToken);
        await NotificarDecisionAsync(solicitud, "aprobada", cancellationToken);

        return Result<SolicitudAusenciaDto>.Ok(await MapDtoAsync(solicitud.Id, cancellationToken));
    }

    public async Task<Result<SolicitudAusenciaDto>> RechazarAsync(Guid aprobadorId, Guid solicitudId, AprobacionRequestDto dto, bool isRhAdmin, CancellationToken cancellationToken = default)
    {
        var solicitud = await _context.SolicitudesAusencia
            .Include(s => s.Colaborador)
            .Include(s => s.TipoAusencia)
            .FirstOrDefaultAsync(s => s.Id == solicitudId && s.IsActive, cancellationToken);

        if (solicitud is null) return Result<SolicitudAusenciaDto>.Fail("Solicitud no encontrada.");
        if (solicitud.Estado != EstadoSolicitud.Pendiente)
            return Result<SolicitudAusenciaDto>.Fail("La solicitud no está pendiente.");

        if (!await PuedeAprobarAsync(aprobadorId, solicitud, isRhAdmin, cancellationToken))
            return Result<SolicitudAusenciaDto>.Fail("No autorizado para rechazar esta solicitud.");

        solicitud.Estado = EstadoSolicitud.Rechazada;
        solicitud.UpdatedAt = DateTime.UtcNow;

        _context.AprobacionesAusencia.Add(new AprobacionAusencia
        {
            Id = Guid.NewGuid(),
            SolicitudId = solicitud.Id,
            AprobadorId = aprobadorId,
            Decision = EstadoSolicitud.Rechazada,
            Comentario = dto.Comentario,
            FechaDecision = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogCambioEstadoAsync("SolicitudAusencia", solicitud.Id, "Pendiente", "Rechazada", aprobadorId.ToString(), dto.Comentario, cancellationToken);
        await NotificarDecisionAsync(solicitud, "rechazada", cancellationToken);

        return Result<SolicitudAusenciaDto>.Ok(await MapDtoAsync(solicitud.Id, cancellationToken));
    }

    public async Task<SolicitudAusenciaDto?> GetByIdAsync(Guid solicitudId, Guid? viewerColaboradorId, bool isRhAdmin, CancellationToken cancellationToken = default)
    {
        var s = await _context.SolicitudesAusencia.AsNoTracking()
            .Include(x => x.Colaborador)
            .FirstOrDefaultAsync(x => x.Id == solicitudId && x.IsActive, cancellationToken);
        if (s is null) return null;

        if (!isRhAdmin && viewerColaboradorId.HasValue)
        {
            var canView = s.ColaboradorId == viewerColaboradorId.Value ||
                          s.Colaborador.JefeDirectoId == viewerColaboradorId.Value;
            if (!canView) return null;
        }

        return await MapDtoAsync(solicitudId, cancellationToken);
    }

    public async Task<IReadOnlyList<SolicitudAusenciaDto>> GetMisSolicitudesAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var ids = await _context.SolicitudesAusencia.AsNoTracking()
            .Where(s => s.ColaboradorId == colaboradorId && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        var result = new List<SolicitudAusenciaDto>();
        foreach (var id in ids)
            result.Add(await MapDtoAsync(id, cancellationToken));
        return result;
    }

    public async Task<IReadOnlyList<PendingApprovalDto>> GetPendientesAprobacionAsync(Guid jefeId, CancellationToken cancellationToken = default)
    {
        var subordinados = await _context.Colaboradores.AsNoTracking()
            .Where(c => c.JefeDirectoId == jefeId && c.IsActive)
            .Select(c => new { c.Id, Puesto = c.Puesto.Nombre, Departamento = c.Departamento.Nombre })
            .ToListAsync(cancellationToken);

        var subIds = subordinados.Select(s => s.Id).ToList();
        var meta = subordinados.ToDictionary(s => s.Id);

        var solicitudes = await _context.SolicitudesAusencia.AsNoTracking()
            .Include(s => s.Colaborador)
            .Include(s => s.TipoAusencia)
            .Where(s => subIds.Contains(s.ColaboradorId) && s.Estado == EstadoSolicitud.Pendiente && s.IsActive)
            .OrderBy(s => s.FechaInicio)
            .ToListAsync(cancellationToken);

        var result = new List<PendingApprovalDto>();
        foreach (var s in solicitudes)
        {
            decimal? saldoDisponible = null;
            decimal? saldoPosterior = null;
            if (s.TipoAusencia.DescuentaSaldo)
            {
                var saldo = await _saldoService.GetMiSaldoAsync(s.ColaboradorId, s.FechaInicio.Year, cancellationToken);
                saldoDisponible = saldo?.DiasDisponibles;
                saldoPosterior = saldoDisponible.HasValue ? saldoDisponible - s.DiasSolicitados : null;
            }

            var traslapes = await GetTraslapesEquipoAsync(jefeId, s, cancellationToken);
            var info = meta.GetValueOrDefault(s.ColaboradorId);

            result.Add(new PendingApprovalDto(
                s.Id, s.ColaboradorId,
                $"{s.Colaborador.Nombre} {s.Colaborador.ApellidoPaterno}",
                info?.Puesto ?? "",
                info?.Departamento ?? "",
                s.TipoAusenciaId, s.TipoAusencia.Nombre, s.TipoAusencia.Codigo,
                s.FechaInicio, s.FechaFin, s.DiasSolicitados, s.Comentario, s.Estado, s.CreatedAt,
                saldoDisponible, saldoPosterior, traslapes));
        }
        return result;
    }

    public async Task<IReadOnlyList<TipoAusenciaDto>> GetAllTiposAsync(CancellationToken cancellationToken = default)
    {
        var tipos = await _context.TiposAusencia.AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.Orden)
            .ToListAsync(cancellationToken);
        return tipos.Select(MapTipoDto).ToList();
    }

    public async Task<TeamCalendarDto> GetTeamCalendarAsync(Guid colaboradorId, DateTime desde, DateTime hasta, CancellationToken cancellationToken = default)
    {
        var colaborador = await _context.Colaboradores.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == colaboradorId, cancellationToken);

        var ausencias = await GetCalendarioAusenciasAsync(colaboradorId, desde, hasta, cancellationToken);
        var inhabiles = await _calendario.GetDiasInhabilesAsync(colaborador?.SedeId, desde.Year, cancellationToken);
        if (hasta.Year != desde.Year)
        {
            var extra = await _calendario.GetDiasInhabilesAsync(colaborador?.SedeId, hasta.Year, cancellationToken);
            inhabiles = new HashSet<DateTime>(inhabiles.Concat(extra));
        }

        var dias = inhabiles
            .Where(d => d >= desde.Date && d <= hasta.Date)
            .OrderBy(d => d)
            .Select(d => new DiaInhabilCalendarioDto(d, "Día inhábil", true))
            .ToList();

        return new TeamCalendarDto(desde, hasta, ausencias, dias);
    }

    public async Task<CalculateDaysResultDto> PreviewDaysAsync(Guid colaboradorId, CalculateDaysDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.FechaFin.Date < dto.FechaInicio.Date)
            return new CalculateDaysResultDto(0, null, null, false, false);

        decimal dias;
        TipoAusencia? tipo = null;
        if (dto.TipoAusenciaId.HasValue)
        {
            tipo = await _context.TiposAusencia.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == dto.TipoAusenciaId.Value, cancellationToken);
            dias = tipo?.EsParcial == true ? 0.5m : await CalcularDiasHabilesAsync(colaboradorId, dto.FechaInicio, dto.FechaFin, cancellationToken);
        }
        else
        {
            dias = await CalcularDiasHabilesAsync(colaboradorId, dto.FechaInicio, dto.FechaFin, cancellationToken);
        }

        decimal? saldoDisponible = null;
        decimal? saldoPosterior = null;
        var excedeSaldo = false;
        if (tipo?.DescuentaSaldo == true)
        {
            var saldo = await _saldoService.GetMiSaldoAsync(colaboradorId, dto.FechaInicio.Year, cancellationToken);
            saldoDisponible = saldo?.DiasDisponibles;
            saldoPosterior = saldoDisponible - dias;
            excedeSaldo = saldoPosterior < 0;
        }

        var traslape = await _context.SolicitudesAusencia.AnyAsync(s =>
            s.ColaboradorId == colaboradorId && s.IsActive &&
            s.Estado != EstadoSolicitud.Cancelada && s.Estado != EstadoSolicitud.Rechazada &&
            s.FechaInicio <= dto.FechaFin && s.FechaFin >= dto.FechaInicio, cancellationToken);

        return new CalculateDaysResultDto(dias, saldoDisponible, saldoPosterior, excedeSaldo, traslape);
    }

    public async Task<IReadOnlyList<CalendarioAusenciaDto>> GetCalendarioAusenciasAsync(Guid colaboradorId, DateTime desde, DateTime hasta, CancellationToken cancellationToken = default)
    {
        var colaborador = await _context.Colaboradores.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == colaboradorId, cancellationToken);
        if (colaborador is null) return Array.Empty<CalendarioAusenciaDto>();

        var equipoIds = await _context.Colaboradores.AsNoTracking()
            .Where(c => c.JefeDirectoId == colaboradorId || c.Id == colaboradorId ||
                        (colaborador.JefeDirectoId != null && c.JefeDirectoId == colaborador.JefeDirectoId))
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        return await _context.SolicitudesAusencia.AsNoTracking()
            .Include(s => s.Colaborador)
            .Include(s => s.TipoAusencia)
            .Where(s => equipoIds.Contains(s.ColaboradorId))
            .Where(s => s.IsActive && s.Estado != EstadoSolicitud.Cancelada && s.Estado != EstadoSolicitud.Rechazada)
            .Where(s => s.FechaFin >= desde && s.FechaInicio <= hasta)
            .Select(s => new CalendarioAusenciaDto(
                s.Id, s.ColaboradorId,
                s.Colaborador.Nombre + " " + s.Colaborador.ApellidoPaterno,
                s.TipoAusencia.Nombre, s.TipoAusencia.Color ?? "#10b981",
                s.FechaInicio, s.FechaFin, s.Estado))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReporteAusenciaDto>> GetReporteAusenciasAsync(DateTime? desde, DateTime? hasta, Guid? sedeId, Guid? tipoId, EstadoSolicitud? estado, CancellationToken cancellationToken = default)
    {
        var query = _context.SolicitudesAusencia.AsNoTracking()
            .Include(s => s.Colaborador).ThenInclude(c => c.Sede)
            .Include(s => s.TipoAusencia)
            .Include(s => s.Aprobaciones).ThenInclude(a => a.Aprobador)
            .Where(s => s.IsActive);

        if (desde.HasValue) query = query.Where(s => s.FechaFin >= desde.Value);
        if (hasta.HasValue) query = query.Where(s => s.FechaInicio <= hasta.Value);
        if (sedeId.HasValue) query = query.Where(s => s.Colaborador.SedeId == sedeId);
        if (tipoId.HasValue) query = query.Where(s => s.TipoAusenciaId == tipoId);
        if (estado.HasValue) query = query.Where(s => s.Estado == estado);

        return await query.OrderByDescending(s => s.FechaInicio).Select(s => new ReporteAusenciaDto(
            s.Colaborador.NumeroEmpleado,
            s.Colaborador.Nombre + " " + s.Colaborador.ApellidoPaterno,
            s.TipoAusencia.Nombre,
            s.FechaInicio, s.FechaFin, s.DiasSolicitados,
            s.Estado.ToString(),
            s.Aprobaciones.OrderByDescending(a => a.FechaDecision).Select(a => a.Aprobador.Nombre).FirstOrDefault()
        )).ToListAsync(cancellationToken);
    }

    public async Task<RhDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var inicioMes = new DateTime(now.Year, now.Month, 1);

        return new RhDashboardDto(
            await _context.SolicitudesAusencia.CountAsync(s => s.Estado == EstadoSolicitud.Pendiente && s.IsActive, cancellationToken),
            await _context.SolicitudesAusencia.CountAsync(s => s.Estado == EstadoSolicitud.Aprobada && s.UpdatedAt >= inicioMes && s.IsActive, cancellationToken),
            await _context.Colaboradores.CountAsync(c => c.IsActive, cancellationToken),
            await _context.SolicitudesAusencia.CountAsync(s => s.Estado == EstadoSolicitud.Rechazada && s.UpdatedAt >= inicioMes && s.IsActive, cancellationToken));
    }

    private async Task<Result<SolicitudAusenciaDto>> ValidarEnvioAsync(Guid colaboradorId, SolicitudAusencia solicitud, TipoAusencia tipo, CancellationToken cancellationToken)
    {
        if (tipo.DescuentaSaldo)
        {
            var saldo = await _saldoService.GetMiSaldoAsync(colaboradorId, solicitud.FechaInicio.Year, cancellationToken);
            if (saldo is null || saldo.DiasPendientes < solicitud.DiasSolicitados)
                return Result<SolicitudAusenciaDto>.Fail($"Saldo insuficiente. Disponible: {saldo?.DiasPendientes ?? 0} días.");
        }

        var limiteAnual = tipo.DiasMaximosAnuales ?? tipo.DiasMaximosEvento;
        if (limiteAnual.HasValue)
        {
            var usados = await _context.SolicitudesAusencia.AsNoTracking()
                .Where(s => s.ColaboradorId == colaboradorId && s.TipoAusenciaId == tipo.Id && s.IsActive &&
                            s.FechaInicio.Year == solicitud.FechaInicio.Year &&
                            s.Estado != EstadoSolicitud.Cancelada && s.Estado != EstadoSolicitud.Rechazada &&
                            s.Id != solicitud.Id)
                .SumAsync(s => s.DiasSolicitados, cancellationToken);

            if (usados + solicitud.DiasSolicitados > limiteAnual.Value)
            {
                var restantes = Math.Max(0, limiteAnual.Value - usados);
                return Result<SolicitudAusenciaDto>.Fail(
                    $"Límite LFT/política excedido para {tipo.Nombre}. Disponible: {restantes} día(s).");
            }
        }

        var solapamiento = await _context.SolicitudesAusencia.AnyAsync(s =>
            s.ColaboradorId == colaboradorId && s.IsActive &&
            s.Estado != EstadoSolicitud.Cancelada && s.Estado != EstadoSolicitud.Rechazada &&
            s.Id != solicitud.Id &&
            s.FechaInicio <= solicitud.FechaFin && s.FechaFin >= solicitud.FechaInicio, cancellationToken);

        if (solapamiento)
            return Result<SolicitudAusenciaDto>.Fail("Ya existe una solicitud activa en ese rango de fechas.");

        if (tipo.RequiereComprobante && string.IsNullOrWhiteSpace(solicitud.Comentario))
            return Result<SolicitudAusenciaDto>.Fail("Este permiso requiere indicar el motivo o adjuntar referencia en comentarios.");

        return Result<SolicitudAusenciaDto>.Ok(null!);
    }

    private async Task AplicarSaldoAsync(SolicitudAusencia solicitud, CancellationToken cancellationToken)
    {
        await _saldoService.EnsureSaldoAnualAsync(solicitud.ColaboradorId, solicitud.FechaInicio.Year, cancellationToken);
        var saldo = await _context.SaldosVacaciones
            .FirstAsync(s => s.ColaboradorId == solicitud.ColaboradorId && s.Anio == solicitud.FechaInicio.Year, cancellationToken);
        saldo.DiasUsados += solicitud.DiasSolicitados;
        saldo.UpdatedAt = DateTime.UtcNow;
    }

    private async Task CrearIncidenciaNominaAsync(SolicitudAusencia solicitud, CancellationToken cancellationToken)
    {
        var colaborador = await _context.Colaboradores.AsNoTracking()
            .FirstAsync(c => c.Id == solicitud.ColaboradorId, cancellationToken);

        _context.IncidenciasNomina.Add(new IncidenciaNomina
        {
            Id = Guid.NewGuid(),
            SolicitudId = solicitud.Id,
            ColaboradorId = solicitud.ColaboradorId,
            NumeroEmpleado = colaborador.NumeroEmpleado,
            TipoIncidencia = solicitud.TipoAusencia.Codigo,
            FechaInicio = solicitud.FechaInicio,
            FechaFin = solicitud.FechaFin,
            Dias = solicitud.DiasSolicitados,
            Estado = "Pendiente",
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                solicitud.Id,
                solicitud.TipoAusencia.Codigo,
                solicitud.TipoAusencia.Nombre,
                solicitud.DiasSolicitados,
                solicitud.FechaInicio,
                solicitud.FechaFin,
            }),
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        });
    }

    private async Task<IReadOnlyList<string>> GetTraslapesEquipoAsync(Guid jefeId, SolicitudAusencia solicitud, CancellationToken cancellationToken)
    {
        var equipoIds = await _context.Colaboradores.AsNoTracking()
            .Where(c => c.JefeDirectoId == jefeId && c.IsActive && c.Id != solicitud.ColaboradorId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var traslapes = await _context.SolicitudesAusencia.AsNoTracking()
            .Include(s => s.Colaborador)
            .Where(s => equipoIds.Contains(s.ColaboradorId) && s.IsActive &&
                        s.Estado != EstadoSolicitud.Cancelada && s.Estado != EstadoSolicitud.Rechazada &&
                        s.Id != solicitud.Id &&
                        s.FechaInicio <= solicitud.FechaFin && s.FechaFin >= solicitud.FechaInicio)
            .Select(s => s.Colaborador.Nombre + " " + s.Colaborador.ApellidoPaterno + " (" + s.FechaInicio.ToString("dd/MM") + ")")
            .ToListAsync(cancellationToken);

        return traslapes;
    }

    private async Task<bool> PuedeAprobarAsync(Guid aprobadorId, SolicitudAusencia solicitud, bool isRhAdmin, CancellationToken cancellationToken)
    {
        if (isRhAdmin) return true;
        return solicitud.Colaborador.JefeDirectoId == aprobadorId;
    }

    private async Task<SolicitudAusencia?> GetOwnedSolicitudAsync(Guid colaboradorId, Guid solicitudId, CancellationToken cancellationToken)
        => await _context.SolicitudesAusencia.FirstOrDefaultAsync(s => s.Id == solicitudId && s.ColaboradorId == colaboradorId && s.IsActive, cancellationToken);

    private async Task<SolicitudAusenciaDto> MapDtoAsync(Guid id, CancellationToken cancellationToken)
    {
        var s = await _context.SolicitudesAusencia.AsNoTracking()
            .Include(x => x.Colaborador)
            .Include(x => x.TipoAusencia)
            .FirstAsync(x => x.Id == id, cancellationToken);

        return new SolicitudAusenciaDto(
            s.Id, s.ColaboradorId,
            $"{s.Colaborador.Nombre} {s.Colaborador.ApellidoPaterno}",
            s.TipoAusenciaId, s.TipoAusencia.Nombre, s.TipoAusencia.Codigo,
            s.FechaInicio, s.FechaFin, s.DiasSolicitados, s.Comentario, s.Estado, s.CreatedAt,
            s.EsDiaCompleto,
            FormatHora(s.HoraInicio),
            FormatHora(s.HoraFin));
    }

    private static TipoAusenciaDto MapTipoDto(TipoAusencia t) => new(
        t.Id, t.Codigo, t.Nombre, t.DescuentaSaldo, t.RequiereAprobacion, t.Color,
        t.Categoria, t.EsParcial, t.PermiteMultiDia, t.DiasMaximosAnuales, t.DiasMaximosEvento,
        t.RequiereComprobante, t.Remunerado, t.BaseLegalLft, t.Descripcion, t.Icono, t.Orden);

    private static TimeSpan? ParseHora(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return TimeSpan.TryParse(value, out var ts) ? ts : null;
    }

    private static string? FormatHora(TimeSpan? value)
        => value is null ? null : $"{(int)value.Value.TotalHours:D2}:{value.Value.Minutes:D2}";

    private async Task NotificarNuevaSolicitudAsync(Guid solicitudId, CancellationToken cancellationToken)
    {
        var s = await _context.SolicitudesAusencia.AsNoTracking()
            .Include(x => x.Colaborador).ThenInclude(c => c.JefeDirecto)
            .Include(x => x.TipoAusencia)
            .FirstAsync(x => x.Id == solicitudId, cancellationToken);

        var destinatario = s.Colaborador.JefeDirecto?.Email;
        if (string.IsNullOrEmpty(destinatario)) return;

        await _notifications.SendAsync(new NotificationRequest(
            "solicitud_pendiente",
            destinatario,
            $"Solicitud de {s.TipoAusencia.Nombre} pendiente",
            $"{s.Colaborador.Nombre} {s.Colaborador.ApellidoPaterno} solicitó {s.DiasSolicitados} día(s) del {s.FechaInicio:dd/MM/yyyy} al {s.FechaFin:dd/MM/yyyy}.",
            s.Id), cancellationToken);
    }

    private async Task NotificarDecisionAsync(SolicitudAusencia solicitud, string decision, CancellationToken cancellationToken)
    {
        await _notifications.SendAsync(new NotificationRequest(
            $"solicitud_{decision}",
            solicitud.Colaborador.Email,
            $"Tu solicitud fue {decision}",
            $"Tu solicitud de {solicitud.TipoAusencia.Nombre} del {solicitud.FechaInicio:dd/MM/yyyy} al {solicitud.FechaFin:dd/MM/yyyy} fue {decision}.",
            solicitud.Id), cancellationToken);
    }

    private async Task EjecutarWebhookAsync(SolicitudAusencia solicitud, string webhookUrl, CancellationToken cancellationToken)
    {
        try
        {
            using var client = new HttpClient();
            var payload = new
            {
                solicitud.Id,
                solicitud.Folio,
                solicitud.ColaboradorId,
                solicitud.TipoAusenciaId,
                solicitud.FechaInicio,
                solicitud.FechaFin,
                solicitud.DiasSolicitados,
                solicitud.Estado
            };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            await client.PostAsync(webhookUrl, content, cancellationToken);
        }
        catch
        {
            // Fallback silencioso en caso de error de red
        }
    }
}
