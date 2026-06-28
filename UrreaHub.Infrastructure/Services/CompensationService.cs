using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Common;
using UrreaHub.Application.Compensaciones;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Compensaciones;
using UrreaHub.Domain.CoreRH;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class CompensationService : ICompensationService
{
    private readonly UrreaHubDbContext _context;
    private readonly IAuditService _audit;

    public CompensationService(UrreaHubDbContext context, IAuditService audit)
    {
        _context = context;
        _audit = audit;
    }

    public async Task<IReadOnlyList<CompensacionColaboradorDto>> ListColaboradoresAsync(CancellationToken cancellationToken = default)
    {
        var items = await LoadColaboradoresQuery().ToListAsync(cancellationToken);
        return items.Select(MapColaborador).ToList();
    }

    public async Task<CompensacionColaboradorDto?> GetColaboradorAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var c = await LoadColaboradoresQuery().FirstOrDefaultAsync(x => x.Id == colaboradorId, cancellationToken);
        return c is null ? null : MapColaborador(c);
    }

    public async Task<Result<SolicitudAjusteDto>> CreateAdjustmentRequestAsync(
        Guid solicitanteId, CrearSolicitudAjusteDto dto, CancellationToken cancellationToken = default)
    {
        var col = await _context.Colaboradores.Include(c => c.DatosLaborales).FirstOrDefaultAsync(c => c.Id == dto.ColaboradorId, cancellationToken);
        if (col is null) return Result<SolicitudAjusteDto>.Fail("Colaborador no encontrado.");

        var now = DateTime.UtcNow;
        var id = Guid.NewGuid();
        var valorAnterior = GetCurrentValue(col, dto.TipoAjuste);

        var solicitud = new SolicitudAjusteCompensacion
        {
            Id = id,
            ColaboradorId = dto.ColaboradorId,
            SolicitanteId = solicitanteId,
            TipoAjuste = dto.TipoAjuste,
            Estado = EstadoAjusteCompensacion.Borrador,
            ValorAnterior = valorAnterior,
            ValorNuevo = dto.ValorNuevo,
            Motivo = dto.Motivo,
            FechaSolicitud = now,
            RequiereFinanzas = dto.RequiereFinanzas || dto.TipoAjuste is TipoAjusteCompensacion.AjusteExtraordinario or TipoAjusteCompensacion.Promocion,
            MontoReferencia = dto.MontoReferencia,
            CreatedAt = now,
            IsActive = true,
        };
        AddHistorial(solicitud, "Creada", null, solicitanteId, now);
        _context.SolicitudesAjusteCompensacion.Add(solicitud);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await GetSolicitudAsync(id, cancellationToken);
        return Result<SolicitudAjusteDto>.Ok(created!);
    }

    public async Task<IReadOnlyList<SolicitudAjusteDto>> ListAdjustmentRequestsAsync(
        EstadoAjusteCompensacion? estado, CancellationToken cancellationToken = default)
    {
        var query = _context.SolicitudesAjusteCompensacion.AsNoTracking()
            .Include(s => s.Colaborador)
            .Include(s => s.Historial).ThenInclude(h => h.Usuario)
            .Where(s => s.IsActive);
        if (estado.HasValue) query = query.Where(s => s.Estado == estado.Value);
        var items = await query.OrderByDescending(s => s.FechaSolicitud).ToListAsync(cancellationToken);
        return items.Select(MapSolicitud).ToList();
    }

    public async Task<Result<SolicitudAjusteDto>> SubmitAdjustmentAsync(Guid solicitanteId, Guid id, CancellationToken cancellationToken = default)
    {
        var s = await _context.SolicitudesAjusteCompensacion
            .Include(x => x.Aprobaciones)
            .FirstOrDefaultAsync(x => x.Id == id && x.SolicitanteId == solicitanteId && x.IsActive, cancellationToken);
        if (s is null) return Result<SolicitudAjusteDto>.Fail("Solicitud no encontrada.");
        if (s.Estado != EstadoAjusteCompensacion.Borrador) return Result<SolicitudAjusteDto>.Fail("Solo borradores pueden enviarse.");

        var rh = await _context.Colaboradores.FirstAsync(c => c.NumeroEmpleado == "1005", cancellationToken);
        var now = DateTime.UtcNow;
        foreach (var a in s.Aprobaciones.ToList()) _context.AprobacionesAjusteCompensacion.Remove(a);
        s.Aprobaciones.Clear();

        s.Aprobaciones.Add(new AprobacionAjusteCompensacion
        {
            Id = Guid.NewGuid(), SolicitudId = id, AprobadorId = rh.Id, Orden = 1,
            RolAprobador = "DH", Decision = EstadoAjusteCompensacion.EnRevisionDh, CreatedAt = now, IsActive = true,
        });
        if (s.RequiereFinanzas)
        {
            var fin = await _context.Colaboradores.FirstAsync(c => c.NumeroEmpleado == "1001", cancellationToken);
            s.Aprobaciones.Add(new AprobacionAjusteCompensacion
            {
                Id = Guid.NewGuid(), SolicitudId = id, AprobadorId = fin.Id, Orden = 2,
                RolAprobador = "Finanzas", Decision = EstadoAjusteCompensacion.EnRevisionFinanzas, CreatedAt = now, IsActive = true,
            });
        }

        s.Estado = EstadoAjusteCompensacion.EnRevisionDh;
        s.UpdatedAt = now;
        AddHistorial(s, "Enviada", "En revisión DH", solicitanteId, now);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<SolicitudAjusteDto>.Ok((await GetSolicitudAsync(id, cancellationToken))!);
    }

    public async Task<Result<SolicitudAjusteDto>> ApproveAdjustmentAsync(
        Guid aprobadorId, Guid id, DecisionAjusteDto dto, bool isFinanzas, CancellationToken cancellationToken = default)
    {
        var s = await _context.SolicitudesAjusteCompensacion.Include(x => x.Aprobaciones).FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
        if (s is null) return Result<SolicitudAjusteDto>.Fail("Solicitud no encontrada.");

        var pending = s.Aprobaciones.Where(a => a.IsActive).OrderBy(a => a.Orden)
            .FirstOrDefault(a => a.Decision is EstadoAjusteCompensacion.EnRevisionDh or EstadoAjusteCompensacion.EnRevisionFinanzas);
        if (pending is null || pending.AprobadorId != aprobadorId)
            return Result<SolicitudAjusteDto>.Fail("No eres el aprobador actual.");

        var now = DateTime.UtcNow;
        pending.Decision = EstadoAjusteCompensacion.Aprobado;
        pending.FechaDecision = now;
        pending.Comentario = dto.Comentario;

        var next = s.Aprobaciones.Where(a => a.IsActive && a.Orden > pending.Orden)
            .FirstOrDefault(a => a.Decision is EstadoAjusteCompensacion.EnRevisionDh or EstadoAjusteCompensacion.EnRevisionFinanzas);
        s.Estado = next is null ? EstadoAjusteCompensacion.Aprobado : next.Decision;
        s.FechaDecision = next is null ? now : null;
        AddHistorial(s, "Aprobada", dto.Comentario, aprobadorId, now);
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Compensaciones", "AprobarAjuste", "SolicitudAjusteCompensacion", id, aprobadorId.ToString(), dto.Comentario, cancellationToken);
        return Result<SolicitudAjusteDto>.Ok((await GetSolicitudAsync(id, cancellationToken))!);
    }

    public async Task<Result<SolicitudAjusteDto>> RejectAdjustmentAsync(
        Guid aprobadorId, Guid id, DecisionAjusteDto dto, CancellationToken cancellationToken = default)
    {
        var s = await _context.SolicitudesAjusteCompensacion.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
        if (s is null) return Result<SolicitudAjusteDto>.Fail("Solicitud no encontrada.");
        s.Estado = EstadoAjusteCompensacion.Rechazado;
        s.FechaDecision = DateTime.UtcNow;
        AddHistorial(s, "Rechazada", dto.Comentario, aprobadorId, DateTime.UtcNow);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<SolicitudAjusteDto>.Ok((await GetSolicitudAsync(id, cancellationToken))!);
    }

    public async Task<Result<SolicitudAjusteDto>> ApplyAdjustmentAsync(Guid id, string performedBy, CancellationToken cancellationToken = default)
    {
        var s = await _context.SolicitudesAjusteCompensacion
            .Include(x => x.Colaborador).ThenInclude(c => c.DatosLaborales)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
        if (s is null) return Result<SolicitudAjusteDto>.Fail("Solicitud no encontrada.");
        if (s.Estado != EstadoAjusteCompensacion.Aprobado)
            return Result<SolicitudAjusteDto>.Fail("La solicitud debe estar aprobada.");

        ApplyToLaborData(s);
        s.Colaborador.IsManualOverride = true;
        s.Estado = EstadoAjusteCompensacion.AplicadoSap;
        s.UpdatedAt = DateTime.UtcNow;
        AddHistorial(s, "Aplicada", "Cambio aplicado en HCM", null, DateTime.UtcNow);
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Compensaciones", "AplicarAjuste", "SolicitudAjusteCompensacion", id, performedBy, s.ValorNuevo, cancellationToken);
        return Result<SolicitudAjusteDto>.Ok((await GetSolicitudAsync(id, cancellationToken))!);
    }

    public async Task<IReadOnlyList<TabuladorDto>> ListTabuladoresAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.Tabuladores.AsNoTracking()
            .Include(t => t.Bandas)
            .Where(t => t.IsActive)
            .ToListAsync(cancellationToken);
        return items.Select(t => new TabuladorDto(
            t.Id, t.Codigo, t.Nombre, t.Moneda,
            t.Bandas.Where(b => b.IsActive).Select(b => new BandaSalarialDto(b.Nivel, b.Minimo, b.Medio, b.Maximo)).ToList())).ToList();
    }

    public async Task<MiCompensacionDto> GetMyPackageAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var col = await _context.Colaboradores.AsNoTracking()
            .Include(c => c.DatosLaborales)
            .FirstOrDefaultAsync(c => c.Id == colaboradorId, cancellationToken);

        var muestraDetalle = col?.DatosLaborales?.NivelVisibilidadCompensacion != "Restricted";

        var beneficios = await _context.Beneficios.AsNoTracking().Where(b => b.IsActive).ToListAsync(cancellationToken);
        var solicitudesBen = await _context.SolicitudesBeneficio.AsNoTracking()
            .Include(s => s.Beneficio)
            .Where(s => s.ColaboradorId == colaboradorId && s.IsActive)
            .OrderByDescending(s => s.FechaSolicitud)
            .Take(10)
            .ToListAsync(cancellationToken);

        var activos = solicitudesBen.Where(s => s.Estado == EstadoSolicitud.Aprobada)
            .Select(s => new BeneficioActivoDto(s.BeneficioId, s.Beneficio.Nombre, s.Beneficio.Descripcion)).ToList();

        var disponibles = beneficios.Select(b => new BeneficioDisponibleDto(b.Id, b.Codigo, b.Nombre, b.Descripcion)).ToList();

        var ajustes = await _context.SolicitudesAjusteCompensacion.AsNoTracking()
            .Include(s => s.Colaborador)
            .Where(s => s.ColaboradorId == colaboradorId && s.IsActive)
            .OrderByDescending(s => s.FechaSolicitud)
            .Take(5)
            .ToListAsync(cancellationToken);

        return new MiCompensacionDto(
            activos,
            disponibles,
            solicitudesBen.Select(s => new SolicitudBeneficioResumenDto(s.Id, s.Beneficio.Nombre, s.Estado.ToString(), s.FechaSolicitud)).ToList(),
            ajustes.Select(s => new SolicitudAjusteResumenDto(s.Id, $"{s.Colaborador.Nombre} {s.Colaborador.ApellidoPaterno}", s.TipoAjuste.ToString(), s.Estado.ToString(), s.FechaSolicitud)).ToList(),
            muestraDetalle);
    }

    public async Task<CompensacionDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var conDatos = await _context.ColaboradoresDatosLaborales.CountAsync(d => d.IsActive, cancellationToken);
        var solicitudes = await _context.SolicitudesAjusteCompensacion.AsNoTracking()
            .Include(s => s.Colaborador).Where(s => s.IsActive).ToListAsync(cancellationToken);
        return new CompensacionDashboardDto(
            conDatos,
            solicitudes.Count(s => s.Estado is EstadoAjusteCompensacion.EnRevisionDh or EstadoAjusteCompensacion.EnRevisionFinanzas),
            solicitudes.Count(s => s.Estado == EstadoAjusteCompensacion.Aprobado),
            solicitudes.Count(s => s.Estado == EstadoAjusteCompensacion.AplicadoSap),
            solicitudes.OrderByDescending(s => s.FechaSolicitud).Take(8)
                .Select(s => new SolicitudAjusteResumenDto(s.Id, $"{s.Colaborador.Nombre} {s.Colaborador.ApellidoPaterno}", s.TipoAjuste.ToString(), s.Estado.ToString(), s.FechaSolicitud)).ToList());
    }

    private IQueryable<Colaborador> LoadColaboradoresQuery() =>
        _context.Colaboradores.AsNoTracking()
            .Include(c => c.Puesto)
            .Include(c => c.Departamento)
            .Include(c => c.CentroCosto)
            .Include(c => c.DatosLaborales)
            .Where(c => c.IsActive && c.FechaBaja == null);

    private async Task<SolicitudAjusteDto?> GetSolicitudAsync(Guid id, CancellationToken cancellationToken)
    {
        var s = await _context.SolicitudesAjusteCompensacion.AsNoTracking()
            .Include(x => x.Colaborador)
            .Include(x => x.Historial).ThenInclude(h => h.Usuario)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return s is null ? null : MapSolicitud(s);
    }

    private static CompensacionColaboradorDto MapColaborador(Colaborador c)
    {
        var dl = c.DatosLaborales;
        return new CompensacionColaboradorDto(
            c.Id, c.NumeroEmpleado, $"{c.Nombre} {c.ApellidoPaterno}", c.Puesto.Nombre, c.Departamento.Nombre,
            c.CentroCosto?.Nombre, dl?.GrupoNomina, dl?.NivelSalarial, dl?.Jornada, dl?.Turno,
            dl?.Sindicalizado ?? false, dl?.NivelVisibilidadCompensacion ?? "Restricted",
            c.ExternalSource ?? "Manual", dl?.UpdatedAt ?? c.UpdatedAt, c.IsManualOverride);
    }

    private static SolicitudAjusteDto MapSolicitud(SolicitudAjusteCompensacion s) => new(
        s.Id, $"{s.Colaborador.Nombre} {s.Colaborador.ApellidoPaterno}", s.Colaborador.NumeroEmpleado,
        s.TipoAjuste.ToString(), s.Estado.ToString(), s.ValorAnterior ?? "—", s.ValorNuevo, s.Motivo,
        s.FechaSolicitud, s.MontoReferencia,
        s.Historial.OrderByDescending(h => h.FechaAccion).Select(h => new HistorialAjusteDto(
            h.Accion, h.Detalle, h.FechaAccion,
            h.Usuario is null ? null : $"{h.Usuario.Nombre} {h.Usuario.ApellidoPaterno}")).ToList());

    private static string? GetCurrentValue(Colaborador c, TipoAjusteCompensacion tipo)
    {
        var dl = c.DatosLaborales;
        return tipo switch
        {
            TipoAjusteCompensacion.CambioGrupoNomina => dl?.GrupoNomina,
            TipoAjusteCompensacion.CambioJornada => dl?.Jornada,
            TipoAjusteCompensacion.CambioTurno => dl?.Turno,
            TipoAjusteCompensacion.CambioNivelSalarial or TipoAjusteCompensacion.AjusteTabulador => dl?.NivelSalarial,
            _ => null,
        };
    }

    private static void ApplyToLaborData(SolicitudAjusteCompensacion s)
    {
        s.Colaborador.DatosLaborales ??= new ColaboradorDatosLaborales
        {
            Id = Guid.NewGuid(), ColaboradorId = s.ColaboradorId, CreatedAt = DateTime.UtcNow, IsActive = true,
        };
        var dl = s.Colaborador.DatosLaborales;
        dl.UpdatedAt = DateTime.UtcNow;
        switch (s.TipoAjuste)
        {
            case TipoAjusteCompensacion.CambioGrupoNomina: dl.GrupoNomina = s.ValorNuevo; break;
            case TipoAjusteCompensacion.CambioJornada: dl.Jornada = s.ValorNuevo; break;
            case TipoAjusteCompensacion.CambioTurno: dl.Turno = s.ValorNuevo; break;
            case TipoAjusteCompensacion.CambioNivelSalarial:
            case TipoAjusteCompensacion.AjusteTabulador:
            case TipoAjusteCompensacion.Promocion:
            case TipoAjusteCompensacion.AjusteExtraordinario:
                dl.NivelSalarial = s.ValorNuevo; break;
        }
    }

    private static void AddHistorial(SolicitudAjusteCompensacion s, string accion, string? detalle, Guid? userId, DateTime now)
    {
        s.Historial.Add(new HistorialAjusteCompensacion
        {
            Id = Guid.NewGuid(), SolicitudId = s.Id, Accion = accion, Detalle = detalle,
            FechaAccion = now, UsuarioId = userId, CreatedAt = now, IsActive = true,
        });
    }
}

public class BenefitsAdminService : IBenefitsAdminService
{
    private readonly UrreaHubDbContext _context;

    public BenefitsAdminService(UrreaHubDbContext context) => _context = context;

    public async Task<IReadOnlyList<BeneficioDisponibleDto>> ListBenefitsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.Beneficios.AsNoTracking().Where(b => b.IsActive).OrderBy(b => b.Nombre).ToListAsync(cancellationToken);
        return items.Select(b => new BeneficioDisponibleDto(b.Id, b.Codigo, b.Nombre, b.Descripcion)).ToList();
    }

    public async Task<IReadOnlyList<SolicitudBeneficioAdminDto>> ListBenefitRequestsAsync(
        Domain.Common.EstadoSolicitud? estado, CancellationToken cancellationToken = default)
    {
        var query = _context.SolicitudesBeneficio.AsNoTracking()
            .Include(s => s.Beneficio).Include(s => s.Colaborador).Where(s => s.IsActive);
        if (estado.HasValue) query = query.Where(s => s.Estado == estado.Value);
        var items = await query.OrderByDescending(s => s.FechaSolicitud).ToListAsync(cancellationToken);
        return items.Select(s => new SolicitudBeneficioAdminDto(
            s.Id, $"{s.Colaborador.Nombre} {s.Colaborador.ApellidoPaterno}", s.Beneficio.Nombre,
            s.MontoSolicitado, s.Estado.ToString(), s.FechaSolicitud)).ToList();
    }

    public async Task<Result<SolicitudBeneficioAdminDto>> ApproveBenefitRequestAsync(
        Guid aprobadorId, Guid solicitudId, DecisionAjusteDto dto, CancellationToken cancellationToken = default)
        => await DecideBenefitAsync(aprobadorId, solicitudId, Domain.Common.EstadoSolicitud.Aprobada, dto, cancellationToken);

    public async Task<Result<SolicitudBeneficioAdminDto>> RejectBenefitRequestAsync(
        Guid aprobadorId, Guid solicitudId, DecisionAjusteDto dto, CancellationToken cancellationToken = default)
        => await DecideBenefitAsync(aprobadorId, solicitudId, Domain.Common.EstadoSolicitud.Rechazada, dto, cancellationToken);

    public async Task<Result<SolicitudBeneficioAdminDto>> CreateBenefitRequestAsync(
        Guid colaboradorId, Guid beneficioId, decimal? monto, string? justificacion, CancellationToken cancellationToken = default)
    {
        var ben = await _context.Beneficios.FirstOrDefaultAsync(b => b.Id == beneficioId && b.IsActive, cancellationToken);
        if (ben is null) return Result<SolicitudBeneficioAdminDto>.Fail("Beneficio no encontrado.");
        var now = DateTime.UtcNow;
        var s = new Domain.Beneficios.SolicitudBeneficio
        {
            Id = Guid.NewGuid(), ColaboradorId = colaboradorId, BeneficioId = beneficioId,
            FechaSolicitud = now, MontoSolicitado = monto, Justificacion = justificacion,
            Estado = Domain.Common.EstadoSolicitud.Pendiente, CreatedAt = now, IsActive = true,
        };
        _context.SolicitudesBeneficio.Add(s);
        await _context.SaveChangesAsync(cancellationToken);
        var created = await MapBenefitRequestAsync(s.Id, cancellationToken);
        return Result<SolicitudBeneficioAdminDto>.Ok(created!);
    }

    private async Task<Result<SolicitudBeneficioAdminDto>> DecideBenefitAsync(
        Guid aprobadorId, Guid solicitudId, Domain.Common.EstadoSolicitud estado, DecisionAjusteDto dto, CancellationToken cancellationToken)
    {
        var s = await _context.SolicitudesBeneficio.FirstOrDefaultAsync(x => x.Id == solicitudId && x.IsActive, cancellationToken);
        if (s is null) return Result<SolicitudBeneficioAdminDto>.Fail("Solicitud no encontrada.");
        s.Estado = estado;
        s.UpdatedAt = DateTime.UtcNow;
        _context.AprobacionesBeneficio.Add(new Domain.Beneficios.AprobacionBeneficio
        {
            Id = Guid.NewGuid(), SolicitudId = solicitudId, AprobadorId = aprobadorId,
            Decision = estado, Comentario = dto.Comentario, FechaDecision = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow, IsActive = true,
        });
        await _context.SaveChangesAsync(cancellationToken);
        return Result<SolicitudBeneficioAdminDto>.Ok((await MapBenefitRequestAsync(solicitudId, cancellationToken))!);
    }

    private async Task<SolicitudBeneficioAdminDto?> MapBenefitRequestAsync(Guid id, CancellationToken cancellationToken)
    {
        var s = await _context.SolicitudesBeneficio.AsNoTracking()
            .Include(x => x.Beneficio).Include(x => x.Colaborador)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return s is null ? null : new SolicitudBeneficioAdminDto(
            s.Id, $"{s.Colaborador.Nombre} {s.Colaborador.ApellidoPaterno}", s.Beneficio.Nombre,
            s.MontoSolicitado, s.Estado.ToString(), s.FechaSolicitud);
    }
}
