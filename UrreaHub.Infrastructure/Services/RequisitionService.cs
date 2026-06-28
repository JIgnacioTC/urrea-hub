using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Common;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Application.Requisiciones;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Reclutamiento;
using UrreaHub.Domain.Requisiciones;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class RequisitionService : IRequisitionService
{
    private readonly UrreaHubDbContext _context;
    private readonly IAuditService _audit;

    public RequisitionService(UrreaHubDbContext context, IAuditService audit)
    {
        _context = context;
        _audit = audit;
    }

    public async Task<IReadOnlyList<RequisicionResumenDto>> ListMyAsync(Guid solicitanteId, CancellationToken cancellationToken = default)
    {
        var items = await _context.RequisicionesPersonal.AsNoTracking()
            .Include(r => r.Solicitante)
            .Include(r => r.Departamento)
            .Include(r => r.Aprobadores).ThenInclude(a => a.Aprobador)
            .Where(r => r.SolicitanteId == solicitanteId && r.IsActive)
            .OrderByDescending(r => r.FechaSolicitud)
            .ToListAsync(cancellationToken);
        return items.Select(MapResumen).ToList();
    }

    public async Task<RequisicionDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var r = await LoadQuery().FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
        return r is null ? null : MapDetail(r);
    }

    public async Task<Result<RequisicionDto>> CreateAsync(Guid solicitanteId, CrearRequisicionDto dto, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var id = Guid.NewGuid();
        var folio = await NextFolioAsync(cancellationToken);

        var req = new RequisicionPersonal
        {
            Id = id,
            Folio = folio,
            Titulo = dto.Titulo,
            VacantesSolicitadas = dto.VacantesSolicitadas,
            Estado = EstadoSolicitud.Borrador,
            FechaSolicitud = now,
            SolicitanteId = solicitanteId,
            DepartamentoId = dto.DepartamentoId,
            CreatedAt = now,
            IsActive = true,
            Justificacion = new JustificacionRequisicion
            {
                Id = Guid.NewGuid(),
                RequisicionId = id,
                Motivo = dto.Motivo,
                ImpactoNegocio = dto.ImpactoNegocio,
                AlternativasConsideradas = dto.AlternativasConsideradas,
                CreatedAt = now,
                IsActive = true,
            },
            Presupuesto = new PresupuestoRequisicion
            {
                Id = Guid.NewGuid(),
                RequisicionId = id,
                MontoAutorizado = dto.MontoAutorizado,
                Moneda = dto.Moneda,
                CentroCostoCodigo = dto.CentroCostoCodigo,
                CreatedAt = now,
                IsActive = true,
            },
            Perfil = new PerfilRequisicion
            {
                Id = Guid.NewGuid(),
                RequisicionId = id,
                DescripcionPuesto = dto.DescripcionPuesto,
                ExperienciaRequerida = dto.ExperienciaRequerida,
                EducacionRequerida = dto.EducacionRequerida,
                CompetenciasRequeridas = dto.CompetenciasRequeridas,
                CreatedAt = now,
                IsActive = true,
            },
        };

        AddHistorial(req, "Creada", "Requisición en borrador", solicitanteId, now);
        _context.RequisicionesPersonal.Add(req);
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Requisiciones", "Crear", "RequisicionPersonal", id, solicitanteId.ToString(), folio, cancellationToken);

        var created = await GetAsync(id, cancellationToken);
        return Result<RequisicionDto>.Ok(created!);
    }

    public async Task<Result<RequisicionDto>> UpdateAsync(Guid solicitanteId, Guid id, ActualizarRequisicionDto dto, CancellationToken cancellationToken = default)
    {
        var req = await _context.RequisicionesPersonal
            .Include(r => r.Justificacion)
            .Include(r => r.Presupuesto)
            .Include(r => r.Perfil)
            .FirstOrDefaultAsync(r => r.Id == id && r.SolicitanteId == solicitanteId && r.IsActive, cancellationToken);

        if (req is null) return Result<RequisicionDto>.Fail("Requisición no encontrada.");
        if (req.Estado != EstadoSolicitud.Borrador && req.Estado != EstadoSolicitud.SolicitaAjuste)
            return Result<RequisicionDto>.Fail("Solo se pueden editar borradores o requisiciones con ajustes solicitados.");

        req.Titulo = dto.Titulo;
        req.VacantesSolicitadas = dto.VacantesSolicitadas;
        req.DepartamentoId = dto.DepartamentoId;
        req.UpdatedAt = DateTime.UtcNow;
        if (req.Estado == EstadoSolicitud.SolicitaAjuste) req.Estado = EstadoSolicitud.Borrador;

        if (req.Justificacion is not null)
        {
            req.Justificacion.Motivo = dto.Motivo;
            req.Justificacion.ImpactoNegocio = dto.ImpactoNegocio;
            req.Justificacion.AlternativasConsideradas = dto.AlternativasConsideradas;
        }
        if (req.Presupuesto is not null)
        {
            req.Presupuesto.MontoAutorizado = dto.MontoAutorizado;
            req.Presupuesto.Moneda = dto.Moneda;
            req.Presupuesto.CentroCostoCodigo = dto.CentroCostoCodigo;
        }
        if (req.Perfil is not null)
        {
            req.Perfil.DescripcionPuesto = dto.DescripcionPuesto;
            req.Perfil.ExperienciaRequerida = dto.ExperienciaRequerida;
            req.Perfil.EducacionRequerida = dto.EducacionRequerida;
            req.Perfil.CompetenciasRequeridas = dto.CompetenciasRequeridas;
        }

        await _context.SaveChangesAsync(cancellationToken);
        var updated = await GetAsync(id, cancellationToken);
        return Result<RequisicionDto>.Ok(updated!);
    }

    public async Task<Result<RequisicionDto>> SubmitAsync(Guid solicitanteId, Guid id, CancellationToken cancellationToken = default)
    {
        var req = await _context.RequisicionesPersonal
            .Include(r => r.Aprobadores)
            .FirstOrDefaultAsync(r => r.Id == id && r.SolicitanteId == solicitanteId && r.IsActive, cancellationToken);

        if (req is null) return Result<RequisicionDto>.Fail("Requisición no encontrada.");
        if (req.Estado != EstadoSolicitud.Borrador && req.Estado != EstadoSolicitud.SolicitaAjuste)
            return Result<RequisicionDto>.Fail("La requisición no está en borrador.");

        var solicitante = await _context.Colaboradores.AsNoTracking().FirstAsync(c => c.Id == solicitanteId, cancellationToken);
        var rhAdmin = await _context.Colaboradores.AsNoTracking().FirstOrDefaultAsync(c => c.NumeroEmpleado == "1005", cancellationToken);
        var now = DateTime.UtcNow;

        foreach (var a in req.Aprobadores.ToList()) _context.AprobadoresRequisicion.Remove(a);
        req.Aprobadores.Clear();

        var orden = 1;
        if (solicitante.JefeDirectoId.HasValue)
        {
            req.Aprobadores.Add(new AprobadorRequisicion
            {
                Id = Guid.NewGuid(),
                RequisicionId = id,
                AprobadorId = solicitante.JefeDirectoId.Value,
                Orden = orden++,
                Decision = EstadoSolicitud.Pendiente,
                CreatedAt = now,
                IsActive = true,
            });
        }
        if (rhAdmin is not null && rhAdmin.Id != solicitante.JefeDirectoId)
        {
            req.Aprobadores.Add(new AprobadorRequisicion
            {
                Id = Guid.NewGuid(),
                RequisicionId = id,
                AprobadorId = rhAdmin.Id,
                Orden = orden,
                Decision = EstadoSolicitud.Pendiente,
                CreatedAt = now,
                IsActive = true,
            });
        }

        req.Estado = EstadoSolicitud.Pendiente;
        req.UpdatedAt = now;
        AddHistorial(req, "Enviada", "Requisición enviada a aprobación", solicitanteId, now);
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Requisiciones", "Enviar", "RequisicionPersonal", id, solicitanteId.ToString(), null, cancellationToken);

        var updated = await GetAsync(id, cancellationToken);
        return Result<RequisicionDto>.Ok(updated!);
    }

    public async Task<IReadOnlyList<RequisicionResumenDto>> ListPendingApprovalsAsync(Guid aprobadorId, CancellationToken cancellationToken = default)
    {
        var ids = await _context.AprobadoresRequisicion.AsNoTracking()
            .Where(a => a.AprobadorId == aprobadorId && a.Decision == EstadoSolicitud.Pendiente && a.IsActive)
            .Select(a => a.RequisicionId)
            .ToListAsync(cancellationToken);

        var items = await _context.RequisicionesPersonal.AsNoTracking()
            .Include(r => r.Solicitante)
            .Include(r => r.Departamento)
            .Include(r => r.Aprobadores).ThenInclude(a => a.Aprobador)
            .Where(r => ids.Contains(r.Id) && r.Estado == EstadoSolicitud.Pendiente && r.IsActive)
            .OrderBy(r => r.FechaSolicitud)
            .ToListAsync(cancellationToken);

        return items.Where(r => GetCurrentApprover(r)?.AprobadorId == aprobadorId).Select(MapResumen).ToList();
    }

    public async Task<Result<RequisicionDto>> ApproveAsync(Guid aprobadorId, Guid id, DecisionRequisicionDto dto, CancellationToken cancellationToken = default)
        => await DecideAsync(aprobadorId, id, dto, EstadoSolicitud.Aprobada, "Aprobada", cancellationToken);

    public async Task<Result<RequisicionDto>> RejectAsync(Guid aprobadorId, Guid id, DecisionRequisicionDto dto, CancellationToken cancellationToken = default)
        => await DecideAsync(aprobadorId, id, dto, EstadoSolicitud.Rechazada, "Rechazada", cancellationToken);

    public async Task<Result<RequisicionDto>> RequestChangesAsync(Guid aprobadorId, Guid id, DecisionRequisicionDto dto, CancellationToken cancellationToken = default)
        => await DecideAsync(aprobadorId, id, dto, EstadoSolicitud.SolicitaAjuste, "SolicitaAjuste", cancellationToken);

    private async Task<Result<RequisicionDto>> DecideAsync(
        Guid aprobadorId, Guid id, DecisionRequisicionDto dto, EstadoSolicitud decision, string accion, CancellationToken cancellationToken)
    {
        var req = await _context.RequisicionesPersonal
            .Include(r => r.Aprobadores)
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive, cancellationToken);

        if (req is null) return Result<RequisicionDto>.Fail("Requisición no encontrada.");
        var current = GetCurrentApprover(req);
        if (current is null || current.AprobadorId != aprobadorId)
            return Result<RequisicionDto>.Fail("No eres el aprobador actual.");

        var now = DateTime.UtcNow;
        current.Decision = decision;
        current.FechaDecision = now;
        current.Comentario = dto.Comentario;

        if (decision == EstadoSolicitud.Rechazada || decision == EstadoSolicitud.SolicitaAjuste)
        {
            req.Estado = decision;
        }
        else if (decision == EstadoSolicitud.Aprobada)
        {
            var pending = req.Aprobadores.Where(a => a.IsActive && a.Decision == EstadoSolicitud.Pendiente).OrderBy(a => a.Orden).FirstOrDefault();
            req.Estado = pending is null ? EstadoSolicitud.Aprobada : EstadoSolicitud.Pendiente;
        }

        req.UpdatedAt = now;
        AddHistorial(req, accion, dto.Comentario, aprobadorId, now);
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Requisiciones", accion, "RequisicionPersonal", id, aprobadorId.ToString(), dto.Comentario, cancellationToken);

        var updated = await GetAsync(id, cancellationToken);
        return Result<RequisicionDto>.Ok(updated!);
    }

    private IQueryable<RequisicionPersonal> LoadQuery() =>
        _context.RequisicionesPersonal.AsNoTracking()
            .Include(r => r.Solicitante)
            .Include(r => r.Departamento)
            .Include(r => r.Justificacion)
            .Include(r => r.Presupuesto)
            .Include(r => r.Perfil)
            .Include(r => r.Aprobadores).ThenInclude(a => a.Aprobador)
            .Include(r => r.Historial).ThenInclude(h => h.Usuario);

    internal static RequisicionResumenDto MapResumen(RequisicionPersonal r)
    {
        var current = GetCurrentApprover(r);
        return new RequisicionResumenDto(
            r.Id,
            r.Folio,
            r.Titulo,
            r.Departamento?.Nombre,
            r.VacantesSolicitadas,
            r.Estado.ToString(),
            $"{r.Solicitante.Nombre} {r.Solicitante.ApellidoPaterno}",
            current is null ? null : $"{current.Aprobador.Nombre} {current.Aprobador.ApellidoPaterno}",
            r.FechaSolicitud);
    }

    internal static RequisicionDto MapDetail(RequisicionPersonal r) => new(
        r.Id,
        r.Folio,
        r.Titulo,
        r.VacantesSolicitadas,
        r.Estado.ToString(),
        r.FechaSolicitud,
        $"{r.Solicitante.Nombre} {r.Solicitante.ApellidoPaterno}",
        r.Departamento?.Nombre,
        GetCurrentApprover(r) is { } c ? $"{c.Aprobador.Nombre} {c.Aprobador.ApellidoPaterno}" : null,
        r.Presupuesto?.MontoAutorizado,
        r.Presupuesto?.Moneda,
        r.Justificacion?.Motivo,
        r.Justificacion?.ImpactoNegocio,
        r.Perfil?.DescripcionPuesto,
        r.Historial.OrderByDescending(h => h.FechaAccion).Select(h => new HistorialRequisicionDto(
            h.Accion,
            h.Detalle,
            h.FechaAccion,
            h.Usuario is null ? null : $"{h.Usuario.Nombre} {h.Usuario.ApellidoPaterno}")).ToList());

    internal static AprobadorRequisicion? GetCurrentApprover(RequisicionPersonal r) =>
        r.Aprobadores.Where(a => a.IsActive).OrderBy(a => a.Orden)
            .FirstOrDefault(a => a.Decision == EstadoSolicitud.Pendiente);

    private async Task<string> NextFolioAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _context.RequisicionesPersonal.CountAsync(r => r.FechaSolicitud.Year == year, cancellationToken);
        return $"REQ-{year}-{(count + 1):D4}";
    }

    private static void AddHistorial(RequisicionPersonal req, string accion, string? detalle, Guid? usuarioId, DateTime now)
    {
        req.Historial.Add(new HistorialRequisicion
        {
            Id = Guid.NewGuid(),
            RequisicionId = req.Id,
            Accion = accion,
            Detalle = detalle,
            FechaAccion = now,
            UsuarioId = usuarioId,
            CreatedAt = now,
            IsActive = true,
        });
    }
}

public class RequisitionAdminService : IRequisitionAdminService
{
    private readonly UrreaHubDbContext _context;
    private readonly IAuditService _audit;

    public RequisitionAdminService(UrreaHubDbContext context, IAuditService audit)
    {
        _context = context;
        _audit = audit;
    }

    public async Task<RequisicionDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var all = await _context.RequisicionesPersonal.AsNoTracking()
            .Include(r => r.Solicitante)
            .Include(r => r.Departamento)
            .Include(r => r.Aprobadores).ThenInclude(a => a.Aprobador)
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);

        return new RequisicionDashboardDto(
            all.Count,
            all.Count(r => r.Estado == EstadoSolicitud.Borrador),
            all.Count(r => r.Estado == EstadoSolicitud.Pendiente),
            all.Count(r => r.Estado == EstadoSolicitud.Aprobada),
            all.Count(r => r.Estado == EstadoSolicitud.ConvertidaVacante),
            all.OrderByDescending(r => r.FechaSolicitud).Take(8).Select(RequisitionService.MapResumen).ToList());
    }

    public async Task<IReadOnlyList<RequisicionResumenDto>> ListAllAsync(EstadoSolicitud? estado, Guid? departamentoId, CancellationToken cancellationToken = default)
    {
        var query = _context.RequisicionesPersonal.AsNoTracking()
            .Include(r => r.Solicitante)
            .Include(r => r.Departamento)
            .Include(r => r.Aprobadores).ThenInclude(a => a.Aprobador)
            .Where(r => r.IsActive);

        if (estado.HasValue) query = query.Where(r => r.Estado == estado.Value);
        if (departamentoId.HasValue) query = query.Where(r => r.DepartamentoId == departamentoId.Value);

        var items = await query.OrderByDescending(r => r.FechaSolicitud).ToListAsync(cancellationToken);
        return items.Select(RequisitionService.MapResumen).ToList();
    }

    public async Task<Result<Guid>> ConvertToVacancyAsync(Guid requisicionId, string performedBy, CancellationToken cancellationToken = default)
    {
        var req = await _context.RequisicionesPersonal
            .Include(r => r.Perfil)
            .FirstOrDefaultAsync(r => r.Id == requisicionId && r.IsActive, cancellationToken);

        if (req is null) return Result<Guid>.Fail("Requisición no encontrada.");
        if (req.Estado != EstadoSolicitud.Aprobada)
            return Result<Guid>.Fail("Solo requisiciones aprobadas pueden convertirse en vacante.");

        var existing = await _context.VacantesReclutamiento.AnyAsync(v => v.RequisicionId == requisicionId, cancellationToken);
        if (existing) return Result<Guid>.Fail("Ya existe vacante para esta requisición.");

        var now = DateTime.UtcNow;
        var count = await _context.VacantesReclutamiento.CountAsync(cancellationToken);
        var vacante = new VacanteReclutamiento
        {
            Id = Guid.NewGuid(),
            Codigo = $"VAC-{now.Year}-{(count + 1):D4}",
            Titulo = req.Titulo,
            Descripcion = req.Perfil?.DescripcionPuesto,
            Estado = EstadoVacante.Abierta,
            FechaPublicacion = now,
            RequisicionId = req.Id,
            CreatedAt = now,
            IsActive = true,
        };

        req.Estado = EstadoSolicitud.ConvertidaVacante;
        req.UpdatedAt = now;
        _context.VacantesReclutamiento.Add(vacante);
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Requisiciones", "ConvertirVacante", "VacanteReclutamiento", vacante.Id, performedBy, req.Folio, cancellationToken);

        return Result<Guid>.Ok(vacante.Id);
    }
}
