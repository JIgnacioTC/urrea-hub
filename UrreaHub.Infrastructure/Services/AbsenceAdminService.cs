using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Application.Vacaciones;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Vacaciones;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class AbsenceAdminService : IAbsenceAdminService
{
    private readonly UrreaHubDbContext _context;
    private readonly ISaldoVacacionesService _saldoService;
    private readonly IAuditService _audit;

    public AbsenceAdminService(UrreaHubDbContext context, ISaldoVacacionesService saldoService, IAuditService audit)
    {
        _context = context;
        _saldoService = saldoService;
        _audit = audit;
    }

    public async Task<IReadOnlyList<PoliticaVacacionesDto>> ListPoliciesAsync(CancellationToken cancellationToken = default)
    {
        var politicas = await _context.PoliticasVacaciones.AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.AntiguedadMinimaMeses)
            .ToListAsync(cancellationToken);

        var counts = await _context.SaldosVacaciones.AsNoTracking()
            .Where(s => s.IsActive)
            .GroupBy(s => s.PoliticaId)
            .Select(g => new { PoliticaId = g.Key, Count = g.Select(x => x.ColaboradorId).Distinct().Count() })
            .ToDictionaryAsync(x => x.PoliticaId, x => x.Count, cancellationToken);

        return politicas.Select(p => new PoliticaVacacionesDto(
            p.Id, p.Nombre, p.Descripcion, p.DiasAnuales, p.AntiguedadMinimaMeses, p.Acumulable, p.IsActive,
            counts.GetValueOrDefault(p.Id, 0))).ToList();
    }

    public async Task<PoliticaVacacionesDto> UpsertPolicyAsync(Guid? id, UpsertPoliticaDto dto, CancellationToken cancellationToken = default)
    {
        PoliticaVacaciones entity;
        if (id.HasValue)
        {
            entity = await _context.PoliticasVacaciones.FirstAsync(p => p.Id == id.Value, cancellationToken);
            entity.Nombre = dto.Nombre.Trim();
            entity.Descripcion = dto.Descripcion?.Trim();
            entity.DiasAnuales = dto.DiasAnuales;
            entity.AntiguedadMinimaMeses = dto.AntiguedadMinimaMeses;
            entity.Acumulable = dto.Acumulable;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            entity = new PoliticaVacaciones
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim(),
                DiasAnuales = dto.DiasAnuales,
                AntiguedadMinimaMeses = dto.AntiguedadMinimaMeses,
                Acumulable = dto.Acumulable,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
            };
            _context.PoliticasVacaciones.Add(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        var count = await _context.SaldosVacaciones.CountAsync(s => s.PoliticaId == entity.Id && s.IsActive, cancellationToken);
        return new PoliticaVacacionesDto(entity.Id, entity.Nombre, entity.Descripcion, entity.DiasAnuales,
            entity.AntiguedadMinimaMeses, entity.Acumulable, entity.IsActive, count);
    }

    public async Task<IReadOnlyList<TipoAusenciaDto>> ListTypesAsync(CancellationToken cancellationToken = default)
    {
        var tipos = await _context.TiposAusencia.AsNoTracking()
            .OrderBy(t => t.Orden)
            .ToListAsync(cancellationToken);
        return tipos.Select(MapTipo).ToList();
    }

    public async Task<TipoAusenciaDto> UpsertTypeAsync(Guid? id, UpsertTipoAusenciaDto dto, CancellationToken cancellationToken = default)
    {
        TipoAusencia entity;
        if (id.HasValue)
        {
            entity = await _context.TiposAusencia.FirstAsync(t => t.Id == id.Value, cancellationToken);
        }
        else
        {
            entity = new TipoAusencia { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
            _context.TiposAusencia.Add(entity);
        }

        entity.Codigo = dto.Codigo.Trim().ToUpperInvariant();
        entity.Nombre = dto.Nombre.Trim();
        entity.DescuentaSaldo = dto.DescuentaSaldo;
        entity.RequiereAprobacion = dto.RequiereAprobacion;
        entity.Color = dto.Color;
        entity.Categoria = dto.Categoria;
        entity.EsParcial = dto.EsParcial;
        entity.PermiteMultiDia = dto.PermiteMultiDia;
        entity.DiasMaximosAnuales = dto.DiasMaximosAnuales;
        entity.DiasMaximosEvento = dto.DiasMaximosEvento;
        entity.RequiereComprobante = dto.RequiereComprobante;
        entity.Remunerado = dto.Remunerado;
        entity.BaseLegalLft = dto.BaseLegalLft;
        entity.Descripcion = dto.Descripcion;
        entity.Icono = dto.Icono;
        entity.Orden = dto.Orden;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return MapTipo(entity);
    }

    public async Task<IReadOnlyList<CalendarioLaboralDto>> ListCalendarsAsync(int? anio, CancellationToken cancellationToken = default)
    {
        var query = _context.CalendariosLaborales.AsNoTracking()
            .Include(c => c.Sede)
            .Include(c => c.DiasInhabiles)
            .Where(c => c.IsActive);

        if (anio.HasValue) query = query.Where(c => c.Anio == anio.Value);

        var items = await query.OrderByDescending(c => c.Anio).ToListAsync(cancellationToken);
        return items.Select(MapCalendario).ToList();
    }

    public async Task<CalendarioLaboralDto> UpsertCalendarAsync(Guid? id, UpsertCalendarioDto dto, CancellationToken cancellationToken = default)
    {
        CalendarioLaboral entity;
        if (id.HasValue)
        {
            entity = await _context.CalendariosLaborales
                .Include(c => c.Sede)
                .Include(c => c.DiasInhabiles)
                .FirstAsync(c => c.Id == id.Value, cancellationToken);
            entity.Nombre = dto.Nombre.Trim();
            entity.Anio = dto.Anio;
            entity.SedeId = dto.SedeId;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            entity = new CalendarioLaboral
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre.Trim(),
                Anio = dto.Anio,
                SedeId = dto.SedeId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
            };
            _context.CalendariosLaborales.Add(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _context.Entry(entity).Reference(c => c.Sede).LoadAsync(cancellationToken);
        await _context.Entry(entity).Collection(c => c.DiasInhabiles).LoadAsync(cancellationToken);
        return MapCalendario(entity);
    }

    public async Task<DiaInhabilDto> AddHolidayAsync(Guid calendarioId, UpsertDiaInhabilDto dto, CancellationToken cancellationToken = default)
    {
        var cal = await _context.CalendariosLaborales.FirstOrDefaultAsync(c => c.Id == calendarioId, cancellationToken)
            ?? throw new InvalidOperationException("Calendario no encontrado.");

        var dia = new DiaInhabil
        {
            Id = Guid.NewGuid(),
            CalendarioId = cal.Id,
            Fecha = dto.Fecha.Date,
            Descripcion = dto.Descripcion.Trim(),
            EsOficial = dto.EsOficial,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };
        _context.DiasInhabiles.Add(dia);
        await _context.SaveChangesAsync(cancellationToken);
        return new DiaInhabilDto(dia.Id, dia.Fecha, dia.Descripcion, dia.EsOficial);
    }

    public async Task<IReadOnlyList<AdminBalanceDto>> ListBalancesAsync(int? anio, string? search, CancellationToken cancellationToken = default)
    {
        var year = anio ?? DateTime.UtcNow.Year;
        var query = _context.SaldosVacaciones.AsNoTracking()
            .Include(s => s.Colaborador).ThenInclude(c => c.Puesto)
            .Include(s => s.Politica)
            .Where(s => s.Anio == year && s.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim().ToLower();
            query = query.Where(s =>
                s.Colaborador.Nombre.ToLower().Contains(q) ||
                s.Colaborador.ApellidoPaterno.ToLower().Contains(q) ||
                s.Colaborador.NumeroEmpleado.Contains(q));
        }

        var saldos = await query.OrderBy(s => s.Colaborador.ApellidoPaterno).ToListAsync(cancellationToken);
        var result = new List<AdminBalanceDto>();

        foreach (var s in saldos)
        {
            var comprometidos = await _saldoService.GetDiasComprometidosAsync(s.ColaboradorId, year, null, cancellationToken);
            var disponibles = s.DiasAsignados - s.DiasUsados - comprometidos;
            result.Add(new AdminBalanceDto(
                s.ColaboradorId,
                s.Colaborador.NumeroEmpleado,
                $"{s.Colaborador.Nombre} {s.Colaborador.ApellidoPaterno}",
                s.Anio,
                s.Politica.Nombre,
                s.DiasAsignados,
                s.DiasUsados,
                comprometidos,
                disponibles,
                s.UpdatedAt ?? s.CreatedAt));
        }

        return result;
    }

    public async Task AdjustBalanceAsync(Guid colaboradorId, AdjustBalanceDto dto, int? anio, string performedBy, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Motivo))
            throw new InvalidOperationException("El motivo del ajuste es obligatorio.");

        var year = anio ?? DateTime.UtcNow.Year;
        await _saldoService.EnsureSaldoAnualAsync(colaboradorId, year, cancellationToken);

        var saldo = await _context.SaldosVacaciones
            .FirstAsync(s => s.ColaboradorId == colaboradorId && s.Anio == year && s.IsActive, cancellationToken);

        var anterior = saldo.DiasAsignados;
        saldo.DiasAsignados = dto.DiasAsignados;
        saldo.UpdatedAt = DateTime.UtcNow;

        _context.AjustesSaldo.Add(new AjusteSaldo
        {
            Id = Guid.NewGuid(),
            SaldoId = saldo.Id,
            ColaboradorId = colaboradorId,
            DiasAnteriores = anterior,
            DiasNuevos = dto.DiasAsignados,
            Delta = dto.DiasAsignados - anterior,
            Motivo = dto.Motivo.Trim(),
            RealizadoPor = performedBy,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        });

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Vacaciones", "AjusteSaldo", "SaldoVacaciones", saldo.Id, performedBy, dto.Motivo, cancellationToken);
    }

    public async Task RecalculateBalancesAsync(RecalculateBalanceDto dto, string performedBy, CancellationToken cancellationToken = default)
    {
        var year = dto.Anio ?? DateTime.UtcNow.Year;
        var colaboradores = await _context.Colaboradores.AsNoTracking()
            .Where(c => c.IsActive)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        foreach (var id in colaboradores)
            await _saldoService.EnsureSaldoAnualAsync(id, year, cancellationToken);

        await _audit.LogEventoAsync("Vacaciones", "RecalcularSaldos", "SaldoVacaciones", Guid.Empty, performedBy, $"Año {year}", cancellationToken);
    }

    public async Task<IReadOnlyList<AdminSolicitudDto>> ListRequestsAsync(
        EstadoSolicitud? estado,
        Guid? sedeId,
        Guid? tipoId,
        DateTime? desde,
        DateTime? hasta,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _context.SolicitudesAusencia.AsNoTracking()
            .Include(s => s.Colaborador).ThenInclude(c => c.Departamento).ThenInclude(d => d.Area)
            .Include(s => s.TipoAusencia)
            .Include(s => s.Aprobaciones).ThenInclude(a => a.Aprobador)
            .Where(s => s.IsActive);

        if (estado.HasValue) query = query.Where(s => s.Estado == estado);
        if (sedeId.HasValue) query = query.Where(s => s.Colaborador.SedeId == sedeId);
        if (tipoId.HasValue) query = query.Where(s => s.TipoAusenciaId == tipoId);
        if (desde.HasValue) query = query.Where(s => s.FechaFin >= desde.Value);
        if (hasta.HasValue) query = query.Where(s => s.FechaInicio <= hasta.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim().ToLower();
            query = query.Where(s =>
                s.Colaborador.Nombre.ToLower().Contains(q) ||
                s.Colaborador.ApellidoPaterno.ToLower().Contains(q) ||
                s.Colaborador.NumeroEmpleado.Contains(q));
        }

        return await query.OrderByDescending(s => s.CreatedAt).Select(s => new AdminSolicitudDto(
            s.Id,
            s.Colaborador.NumeroEmpleado,
            s.Colaborador.Nombre + " " + s.Colaborador.ApellidoPaterno,
            s.Colaborador.Departamento.Nombre,
            s.Colaborador.Departamento.Area.Nombre,
            s.TipoAusencia.Nombre,
            s.FechaInicio,
            s.FechaFin,
            s.DiasSolicitados,
            s.Estado,
            s.CreatedAt,
            s.Aprobaciones.OrderByDescending(a => a.FechaDecision).Select(a => a.Aprobador.Nombre).FirstOrDefault()
        )).ToListAsync(cancellationToken);
    }

    public async Task CancelAdministrativelyAsync(Guid solicitudId, string motivo, string performedBy, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new InvalidOperationException("El motivo es obligatorio.");

        var solicitud = await _context.SolicitudesAusencia
            .Include(s => s.TipoAusencia)
            .FirstOrDefaultAsync(s => s.Id == solicitudId && s.IsActive, cancellationToken)
            ?? throw new InvalidOperationException("Solicitud no encontrada.");

        if (solicitud.Estado is EstadoSolicitud.Cancelada or EstadoSolicitud.Rechazada)
            throw new InvalidOperationException("La solicitud ya está cerrada.");

        var anterior = solicitud.Estado.ToString();

        if (solicitud.Estado == EstadoSolicitud.Aprobada && solicitud.TipoAusencia.DescuentaSaldo)
        {
            var saldo = await _context.SaldosVacaciones
                .FirstOrDefaultAsync(s => s.ColaboradorId == solicitud.ColaboradorId && s.Anio == solicitud.FechaInicio.Year, cancellationToken);
            if (saldo is not null)
            {
                saldo.DiasUsados = Math.Max(0, saldo.DiasUsados - solicitud.DiasSolicitados);
                saldo.UpdatedAt = DateTime.UtcNow;
            }
        }

        solicitud.Estado = EstadoSolicitud.Cancelada;
        solicitud.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogCambioEstadoAsync("SolicitudAusencia", solicitud.Id, anterior, "Cancelada", performedBy, motivo, cancellationToken);
    }

    public async Task<IReadOnlyList<IncidenciaNominaDto>> ListPayrollIncidentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.IncidenciasNomina.AsNoTracking()
            .Include(i => i.Colaborador)
            .Where(i => i.IsActive)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new IncidenciaNominaDto(
                i.Id, i.SolicitudId, i.NumeroEmpleado,
                i.Colaborador.Nombre + " " + i.Colaborador.ApellidoPaterno,
                i.TipoIncidencia, i.FechaInicio, i.FechaFin, i.Dias, i.Estado, i.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    private static TipoAusenciaDto MapTipo(TipoAusencia t) => new(
        t.Id, t.Codigo, t.Nombre, t.DescuentaSaldo, t.RequiereAprobacion, t.Color,
        t.Categoria, t.EsParcial, t.PermiteMultiDia, t.DiasMaximosAnuales, t.DiasMaximosEvento,
        t.RequiereComprobante, t.Remunerado, t.BaseLegalLft, t.Descripcion, t.Icono, t.Orden);

    private static CalendarioLaboralDto MapCalendario(CalendarioLaboral c) => new(
        c.Id, c.Nombre, c.Anio, c.SedeId, c.Sede?.Nombre, c.IsActive,
        c.DiasInhabiles.Where(d => d.IsActive).OrderBy(d => d.Fecha)
            .Select(d => new DiaInhabilDto(d.Id, d.Fecha, d.Descripcion, d.EsOficial)).ToList());
}
