using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UrreaHub.Application.Common;
using UrreaHub.Application.HCM;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Application.Nomina;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;
using UrreaHub.Domain.Vacaciones;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class HcmEmployeeService : IHcmEmployeeService
{
    private readonly UrreaHubDbContext _context;
    private readonly IAuditService _audit;

    public HcmEmployeeService(UrreaHubDbContext context, IAuditService audit)
    {
        _context = context;
        _audit = audit;
    }

    public async Task<HcmDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var activos = await _context.Colaboradores.CountAsync(c => c.IsActive && c.FechaBaja == null, cancellationToken);
        var centrosCosto = await _context.CentrosCosto.CountAsync(c => c.IsActive, cancellationToken);
        var unidades = await _context.Departamentos.CountAsync(d => d.IsActive, cancellationToken);
        var sedes = await _context.Sedes.CountAsync(s => s.IsActive, cancellationToken);
        var ultimaSync = await _context.Colaboradores
            .Where(c => c.NominaSyncAt != null)
            .MaxAsync(c => (DateTime?)c.NominaSyncAt, cancellationToken);

        return new HcmDashboardDto(activos, centrosCosto, unidades, sedes, ultimaSync);
    }

    public async Task<HcmCatalogsDto> GetCatalogsAsync(CancellationToken cancellationToken = default)
    {
        var locations = await _context.Sedes.AsNoTracking()
            .Where(s => s.IsActive).OrderBy(s => s.Nombre)
            .Select(s => new HcmCatalogItemDto(s.Id, s.Nombre, s.Codigo)).ToListAsync(cancellationToken);
        var departments = await _context.Departamentos.AsNoTracking()
            .Where(d => d.IsActive).OrderBy(d => d.Nombre)
            .Select(d => new HcmCatalogItemDto(d.Id, d.Nombre, d.Codigo)).ToListAsync(cancellationToken);
        var positions = await _context.Puestos.AsNoTracking()
            .Where(p => p.IsActive).OrderBy(p => p.Nombre)
            .Select(p => new HcmCatalogItemDto(p.Id, p.Nombre, p.Codigo)).ToListAsync(cancellationToken);
        var costCenters = await _context.CentrosCosto.AsNoTracking()
            .Where(c => c.IsActive).OrderBy(c => c.Nombre)
            .Select(c => new HcmCatalogItemDto(c.Id, c.Nombre, c.Codigo)).ToListAsync(cancellationToken);
        var contracts = await _context.RelacionesLaborales.AsNoTracking()
            .Where(r => r.IsActive).OrderBy(r => r.Nombre)
            .Select(r => new HcmCatalogItemDto(r.Id, r.Nombre, r.Codigo)).ToListAsync(cancellationToken);
        return new HcmCatalogsDto(locations, departments, positions, costCenters, contracts);
    }

    public async Task<PagedResult<EmployeeListDto>> GetEmployeesAsync(
        int page, int pageSize, string? search,
        Guid? departmentId, Guid? locationId, string? status,
        string? externalSource, string? syncStatus,
        Guid? positionId, Guid? costCenterId, Guid? contractTypeId,
        CancellationToken cancellationToken = default)
    {
        var query = BuildEmployeeQuery(search, departmentId, locationId, status, externalSource, syncStatus, positionId, costCenterId, contractTypeId);
        var total = await query.CountAsync(cancellationToken);
        var now = DateTime.UtcNow;

        var items = await query
            .OrderBy(c => c.ApellidoPaterno).ThenBy(c => c.Nombre)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => new EmployeeListDto(
                c.Id, c.NumeroEmpleado,
                $"{c.Nombre} {c.ApellidoPaterno}",
                c.NombrePreferido ?? c.Nombre,
                c.Puesto.Nombre, c.Departamento.Nombre, c.Departamento.Area.Nombre,
                c.CentroCosto != null ? c.CentroCosto.Codigo : null,
                c.JefeDirecto != null ? $"{c.JefeDirecto.Nombre} {c.JefeDirecto.ApellidoPaterno}" : null,
                c.Sede != null ? c.Sede.Nombre : null,
                c.RelacionLaboral.Nombre,
                ResolveStatus(c),
                c.FechaIngreso,
                (int)((now - c.FechaIngreso).TotalDays / 365.25),
                c.ExternalSource, c.SyncStatus, c.NominaSyncAt, c.IsManualOverride))
            .ToListAsync(cancellationToken);

        return new PagedResult<EmployeeListDto> { Items = items, Total = total, Page = page, PageSize = pageSize };
    }

    public async Task<EmployeeDetailDto?> GetEmployeeAsync(Guid id, bool includeSensitive, CancellationToken cancellationToken = default)
    {
        var c = await _context.Colaboradores.AsNoTracking()
            .Include(x => x.Puesto)
            .Include(x => x.Departamento).ThenInclude(d => d.Area)
            .Include(x => x.Sede)
            .Include(x => x.CentroCosto)
            .Include(x => x.RelacionLaboral)
            .Include(x => x.JefeDirecto)
            .Include(x => x.DatosSensibles)
            .Include(x => x.DatosLaborales)
            .Include(x => x.Subordinados.Where(s => s.IsActive)).ThenInclude(s => s.Puesto)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (c is null) return null;

        var rfc = c.DatosSensibles?.Rfc;
        EmployeePersonalDto? personal = null;
        if (includeSensitive && c.DatosSensibles is not null)
        {
            var s = c.DatosSensibles;
            personal = new EmployeePersonalDto(
                MaskIfNeeded(s.Rfc, s.Enmascarado),
                MaskIfNeeded(s.Curp, s.Enmascarado),
                MaskIfNeeded(s.Nss, s.Enmascarado),
                s.FechaNacimiento,
                s.Domicilio,
                s.Enmascarado);
        }

        return new EmployeeDetailDto(
            c.Id, c.NumeroEmpleado, c.Nombre, c.ApellidoPaterno, c.ApellidoMaterno,
            c.NombrePreferido, c.Email, c.Telefono, ResolveStatus(c),
            c.FechaIngreso, c.FechaBaja,
            c.Puesto.Nombre, c.Departamento.Nombre, c.AreaName(),
            c.Sede?.Nombre,
            c.CentroCosto != null ? $"{c.CentroCosto.Codigo} · {c.CentroCosto.Nombre}" : null,
            c.RelacionLaboral.Nombre,
            c.JefeDirecto is null ? null : $"{c.JefeDirecto.Nombre} {c.JefeDirecto.ApellidoPaterno}",
            (int)((DateTime.UtcNow - c.FechaIngreso).TotalDays / 365.25),
            c.ExternalSource, c.ExternalEmployeeId, c.SyncStatus, c.NominaSyncAt, c.IsManualOverride,
            personal,
            c.DatosLaborales is null ? null : new EmployeeEmploymentDto(
                c.DatosLaborales.Jornada, c.DatosLaborales.Turno, c.DatosLaborales.GrupoNomina,
                c.DatosLaborales.Sindicalizado, c.DatosLaborales.NivelVisibilidadCompensacion),
            c.Subordinados.Select(s => new EmployeeSummaryDto(
                s.Id, s.NumeroEmpleado, $"{s.Nombre} {s.ApellidoPaterno}", s.Puesto.Nombre)).ToList());
    }

    public async Task<IReadOnlyList<EmployeeMovementDto>> GetMovementsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MovimientosColaborador.AsNoTracking()
            .Where(m => m.ColaboradorId == id && m.IsActive)
            .OrderByDescending(m => m.FechaEfectiva)
            .Select(m => new EmployeeMovementDto(
                m.Id, m.TipoMovimiento, m.FechaEfectiva, m.ValorAnterior, m.ValorNuevo,
                m.Origen, m.ReferenciaExterna, m.CreadoPor, m.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<EmployeeOrganizationDto?> GetOrganizationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var c = await _context.Colaboradores.AsNoTracking()
            .Include(x => x.Puesto)
            .Include(x => x.Departamento).ThenInclude(d => d.Area)
            .Include(x => x.Sede)
            .Include(x => x.CentroCosto)
            .Include(x => x.JefeDirecto)
            .Include(x => x.Subordinados.Where(s => s.IsActive))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (c is null) return null;

        return new EmployeeOrganizationDto(
            c.JefeDirectoId,
            c.JefeDirecto is null ? null : $"{c.JefeDirecto.Nombre} {c.JefeDirecto.ApellidoPaterno}",
            c.JefeDirecto?.NumeroEmpleado,
            c.DepartamentoId, c.Departamento.Nombre, c.AreaName(),
            c.SedeId, c.Sede?.Nombre,
            c.CentroCostoId, c.CentroCosto != null ? $"{c.CentroCosto.Codigo} · {c.CentroCosto.Nombre}" : null,
            c.PuestoId, c.Puesto.Nombre,
            c.Subordinados.Count);
    }

    public async Task<IReadOnlyList<EmployeeAuditLogDto>> GetAuditLogAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var bitacora = await _context.BitacoraEventos.AsNoTracking()
            .Where(b => b.EntidadId == id && b.IsActive)
            .OrderByDescending(b => b.FechaEvento)
            .Take(50)
            .Select(b => new EmployeeAuditLogDto(b.FechaEvento, b.Modulo, b.Accion, b.Usuario, b.Detalle))
            .ToListAsync(cancellationToken);

        var cambios = await _context.CambiosEstado.AsNoTracking()
            .Where(c => c.EntidadId == id && c.IsActive)
            .OrderByDescending(c => c.FechaCambio)
            .Take(25)
            .Select(c => new EmployeeAuditLogDto(
                c.FechaCambio, "Estado", "CambioEstado", c.Usuario,
                $"{c.EstadoAnterior} → {c.EstadoNuevo}" + (c.Motivo != null ? $" · {c.Motivo}" : "")))
            .ToListAsync(cancellationToken);

        return bitacora.Concat(cambios).OrderByDescending(x => x.OccurredAt).Take(50).ToList();
    }

    public async Task<EmployeeVacationSummaryDto?> GetVacationSummaryAsync(Guid id, int? year, CancellationToken cancellationToken = default)
    {
        var y = year ?? DateTime.UtcNow.Year;
        var saldo = await _context.SaldosVacaciones.AsNoTracking()
            .FirstOrDefaultAsync(s => s.ColaboradorId == id && s.Anio == y && s.IsActive, cancellationToken);

        var requests = await _context.SolicitudesAusencia.AsNoTracking()
            .Where(s => s.ColaboradorId == id && s.FechaInicio.Year == y && s.IsActive)
            .ToListAsync(cancellationToken);

        if (saldo is null && requests.Count == 0) return null;

        return new EmployeeVacationSummaryDto(
            y,
            saldo?.DiasAsignados ?? 0,
            saldo?.DiasUsados ?? 0,
            saldo?.DiasPendientes ?? 0,
            requests.Count,
            requests.Count(r => r.Estado == EstadoSolicitud.Pendiente));
    }

    public async Task<IReadOnlyList<EmployeeDocumentDto>> GetDocumentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Documentos.AsNoTracking()
            .Where(d => d.IsActive && d.Expediente.ColaboradorId == id && d.Expediente.IsActive)
            .OrderBy(d => d.Nombre)
            .Select(d => new EmployeeDocumentDto(
                d.Id,
                d.Nombre,
                d.TipoDocumento.Nombre,
                d.VersionActual,
                d.Vigencia != null && d.Vigencia.FechaFin != null && d.Vigencia.FechaFin < DateTime.UtcNow
                    ? "Vencido" : "Vigente",
                d.Vigencia != null ? d.Vigencia.FechaFin : null))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmployeeModuleLinkDto>> GetModuleLinksAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var vacReq = await _context.SolicitudesAusencia.CountAsync(s => s.ColaboradorId == id && s.IsActive, cancellationToken);
        var attReq = await _context.RegistrosAsistencia.CountAsync(r => r.ColaboradorId == id && r.IsActive, cancellationToken);
        var onboardingPlans = await _context.PlanesOnboarding.CountAsync(p => p.ColaboradorId == id && p.IsActive, cancellationToken);
        var courses = await _context.InscripcionesCurso.CountAsync(i => i.ColaboradorId == id && i.IsActive, cancellationToken);
        var evals = await _context.EvaluacionesDesempeno.CountAsync(e => e.ColaboradorId == id && e.IsActive, cancellationToken);
        var benefits = await _context.SolicitudesBeneficio.CountAsync(b => b.ColaboradorId == id && b.IsActive, cancellationToken);
        var adjustments = await _context.SolicitudesAjusteCompensacion.CountAsync(a => a.ColaboradorId == id && a.IsActive, cancellationToken);
        var docs = await _context.Expedientes.Where(e => e.ColaboradorId == id).SelectMany(e => e.Documentos).CountAsync(cancellationToken);

        return new List<EmployeeModuleLinkDto>
        {
            new("vacations", "Vacaciones y permisos", vacReq, true, vacReq > 0 ? $"{vacReq} solicitudes registradas" : "Sin solicitudes"),
            new("attendance", "Control de asistencia", attReq, true, attReq > 0 ? $"{attReq} registros" : "Sin registros"),
            new("onboarding", "Onboarding", onboardingPlans, true, onboardingPlans > 0 ? $"{onboardingPlans} plan(es)" : "Sin plan activo"),
            new("compensation", "Compensación", adjustments, true, adjustments > 0 ? $"{adjustments} ajuste(s)" : "Sin ajustes"),
            new("training", "Capacitación (LMS)", courses, true, courses > 0 ? $"{courses} inscripciones" : "Sin inscripciones"),
            new("performance", "Desempeño", evals, true, evals > 0 ? $"{evals} evaluaciones" : "Sin evaluaciones"),
            new("benefits", "Beneficios", benefits, true, benefits > 0 ? $"{benefits} solicitudes" : "Sin solicitudes activas"),
            new("documents", "Expediente digital", docs, true, docs > 0 ? $"{docs} documentos" : "Expediente vacío"),
        };
    }

    public async Task<HcmDataQualityReportDto> GetDataQualityReportAsync(CancellationToken cancellationToken = default)
    {
        var summary = await GetDataQualityAsync(cancellationToken);
        var activos = _context.Colaboradores.AsNoTracking()
            .Include(c => c.Puesto)
            .Include(c => c.DatosSensibles)
            .Where(c => c.IsActive && c.FechaBaja == null);

        async Task<HcmDataQualityIssueDto> Issue(string code, string label, IQueryable<Colaborador> q)
        {
            var count = await q.CountAsync(cancellationToken);
            var samples = await q.OrderBy(c => c.ApellidoPaterno).Take(5)
                .Select(c => new EmployeeSummaryDto(c.Id, c.NumeroEmpleado, $"{c.Nombre} {c.ApellidoPaterno}", c.Puesto.Nombre))
                .ToListAsync(cancellationToken);
            return new HcmDataQualityIssueDto(code, label, count, samples);
        }

        var issues = new List<HcmDataQualityIssueDto>
        {
            await Issue("no_manager", "Sin jefe asignado", activos.Where(c => c.JefeDirectoId == null)),
            await Issue("no_cost_center", "Sin centro de costo", activos.Where(c => c.CentroCostoId == null)),
            await Issue("missing_rfc", "RFC faltante", activos.Where(c => c.DatosSensibles == null || c.DatosSensibles.Rfc == null)),
            await Issue("missing_curp", "CURP faltante", activos.Where(c => c.DatosSensibles == null || c.DatosSensibles.Curp == null)),
            await Issue("missing_nss", "NSS faltante", activos.Where(c => c.DatosSensibles == null || c.DatosSensibles.Nss == null)),
            await Issue("pending_sync", "Pendientes de sincronización", activos.Where(c => c.SyncStatus == EmployeeSyncStatus.Pending)),
            await Issue("manual_conflict", "Override manual con conflicto", activos.Where(c => c.IsManualOverride && c.SyncStatus == EmployeeSyncStatus.Conflict)),
        };

        return new HcmDataQualityReportDto(summary, issues.Where(i => i.Count > 0).ToList());
    }

    public async Task<EmployeeDetailDto?> UpdateEmployeeAsync(Guid id, EmployeeUpdateDto dto, string performedBy, CancellationToken cancellationToken = default)
    {
        var c = await _context.Colaboradores.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
        if (c is null) return null;

        if (dto.PreferredName is not null) c.NombrePreferido = dto.PreferredName;
        if (dto.Phone is not null) c.Telefono = dto.Phone;
        c.IsManualOverride = true;
        c.SyncStatus = EmployeeSyncStatus.ManualOverride;
        c.UpdatedAt = DateTime.UtcNow;

        _context.MovimientosColaborador.Add(new MovimientoColaborador
        {
            Id = Guid.NewGuid(),
            ColaboradorId = id,
            TipoMovimiento = "CorreccionManual",
            FechaEfectiva = DateTime.UtcNow.Date,
            ValorNuevo = "Actualización campos permitidos",
            Origen = EmployeeExternalSource.Manual,
            CreadoPor = performedBy,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        });

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("HCM", "UpdateEmployee", "Colaborador", id, performedBy, "Edición manual de campos permitidos", cancellationToken);
        return await GetEmployeeAsync(id, true, cancellationToken);
    }

    public async Task<HcmDataQualityDto> GetDataQualityAsync(CancellationToken cancellationToken = default)
    {
        var activos = _context.Colaboradores.Where(c => c.IsActive && c.FechaBaja == null);
        var total = await activos.CountAsync(cancellationToken);
        var withoutManager = await activos.CountAsync(c => c.JefeDirectoId == null, cancellationToken);
        var withoutCc = await activos.CountAsync(c => c.CentroCostoId == null, cancellationToken);
        var missingRfc = await activos.CountAsync(c =>
            c.DatosSensibles == null || c.DatosSensibles.Rfc == null, cancellationToken);
        var missingCurp = await activos.CountAsync(c => c.DatosSensibles == null || c.DatosSensibles.Curp == null, cancellationToken);
        var missingNss = await activos.CountAsync(c => c.DatosSensibles == null || c.DatosSensibles.Nss == null, cancellationToken);
        var pendingSync = await activos.CountAsync(c => c.SyncStatus == EmployeeSyncStatus.Pending, cancellationToken);
        var conflicts = await activos.CountAsync(c => c.IsManualOverride && c.SyncStatus == EmployeeSyncStatus.Conflict, cancellationToken);

        return new HcmDataQualityDto(withoutManager, withoutCc, missingRfc, missingCurp, missingNss, pendingSync, conflicts, total);
    }

    public async Task<bool> TriggerEmployeeSyncAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var c = await _context.Colaboradores.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (c is null) return false;
        if (c.IsManualOverride)
        {
            c.SyncStatus = EmployeeSyncStatus.ManualOverride;
            await _context.SaveChangesAsync(cancellationToken);
            return false;
        }

        c.SyncStatus = EmployeeSyncStatus.Synced;
        c.NominaSyncAt = DateTime.UtcNow;
        c.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("HCM", "SyncEmployee", "Colaborador", id, "system", "Resincronización manual solicitada", cancellationToken);
        return true;
    }

    private IQueryable<Colaborador> BuildEmployeeQuery(
        string? search, Guid? departmentId, Guid? locationId, string? status,
        string? externalSource, string? syncStatus,
        Guid? positionId, Guid? costCenterId, Guid? contractTypeId)
    {
        var query = _context.Colaboradores.AsNoTracking()
            .Include(c => c.Puesto)
            .Include(c => c.Departamento).ThenInclude(d => d.Area)
            .Include(c => c.Sede)
            .Include(c => c.CentroCosto)
            .Include(c => c.RelacionLaboral)
            .Include(c => c.JefeDirecto)
            .AsQueryable();

        query = string.IsNullOrWhiteSpace(status) switch
        {
            false when status!.Equals("activo", StringComparison.OrdinalIgnoreCase) => query.Where(c => c.IsActive && c.FechaBaja == null),
            false when status.Equals("baja", StringComparison.OrdinalIgnoreCase) => query.Where(c => !c.IsActive || c.FechaBaja != null),
            false when status.Equals("all", StringComparison.OrdinalIgnoreCase) => query,
            _ => query.Where(c => c.IsActive && c.FechaBaja == null),
        };

        if (departmentId.HasValue) query = query.Where(c => c.DepartamentoId == departmentId);
        if (locationId.HasValue) query = query.Where(c => c.SedeId == locationId);
        if (positionId.HasValue) query = query.Where(c => c.PuestoId == positionId);
        if (costCenterId.HasValue) query = query.Where(c => c.CentroCostoId == costCenterId);
        if (contractTypeId.HasValue) query = query.Where(c => c.RelacionLaboralId == contractTypeId);
        if (!string.IsNullOrWhiteSpace(externalSource)) query = query.Where(c => c.ExternalSource == externalSource);
        if (!string.IsNullOrWhiteSpace(syncStatus)) query = query.Where(c => c.SyncStatus == syncStatus);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(c =>
                c.NumeroEmpleado.ToLower().Contains(term) ||
                c.Nombre.ToLower().Contains(term) ||
                c.ApellidoPaterno.ToLower().Contains(term) ||
                (c.DatosSensibles != null && c.DatosSensibles.Rfc != null && c.DatosSensibles.Rfc.ToLower().Contains(term)) ||
                c.Puesto.Nombre.ToLower().Contains(term));
        }

        return query;
    }

    private static string ResolveStatus(Colaborador c) =>
        !c.IsActive || c.FechaBaja != null ? "baja" : "activo";

    private static string? MaskIfNeeded(string? value, bool masked) =>
        masked && !string.IsNullOrEmpty(value) && value.Length > 4
            ? new string('*', value.Length - 4) + value[^4..]
            : value;
}

internal static class ColaboradorExtensions
{
    public static string? AreaName(this Colaborador c) => c.Departamento?.Area?.Nombre;
}

public class IntegrationService : IIntegrationService
{
    private readonly UrreaHubDbContext _context;
    private readonly INominaSyncService _nominaSync;
    private readonly IAuditService _audit;

    public IntegrationService(UrreaHubDbContext context, INominaSyncService nominaSync, IAuditService audit)
    {
        _context = context;
        _nominaSync = nominaSync;
        _audit = audit;
    }

    public async Task<IReadOnlyList<SyncRunDto>> GetSyncRunsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Integraciones.AsNoTracking()
            .Where(i => i.IsActive)
            .OrderByDescending(i => i.UltimaEjecucion)
            .Select(i => new SyncRunDto(i.Id, i.Nombre, i.SistemaExterno, i.Estado.ToString(), i.UltimaEjecucion))
            .ToListAsync(cancellationToken);
    }

    public Task<int> RunNominaSyncAsync(CancellationToken cancellationToken = default)
        => _nominaSync.SyncAsync(cancellationToken);

    public async Task<CdmUpsertResultDto> UpsertEmployeeFromCdmAsync(CdmEmployeeUpsertDto payload, string performedBy, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(payload.EmployeeNumber))
            return new CdmUpsertResultDto(false, null, "Rejected", "EmployeeNumber requerido.");

        var colaborador = await _context.Colaboradores
            .Include(c => c.DatosSensibles)
            .FirstOrDefaultAsync(c =>
                c.ExternalEmployeeId == payload.ExternalEmployeeId ||
                c.NumeroEmpleado == payload.EmployeeNumber, cancellationToken);

        var isNew = colaborador is null;
        if (isNew)
        {
            var relacion = await _context.RelacionesLaborales.FirstAsync(cancellationToken);
            var depto = await _context.Departamentos.Include(d => d.Area).FirstAsync(cancellationToken);
            var puesto = await _context.Puestos.FirstAsync(cancellationToken);
            colaborador = new Colaborador
            {
                Id = Guid.NewGuid(),
                NumeroEmpleado = payload.EmployeeNumber,
                RelacionLaboralId = relacion.Id,
                DepartamentoId = depto.Id,
                PuestoId = puesto.Id,
                FechaIngreso = payload.HireDate,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };
            _context.Colaboradores.Add(colaborador);
        }

        if (colaborador!.IsManualOverride)
            return new CdmUpsertResultDto(false, colaborador.Id, "Blocked", "Registro con override manual; sync bloqueado.");

        colaborador.Nombre = payload.LegalFirstName;
        colaborador.ApellidoPaterno = payload.LegalLastName;
        colaborador.NombrePreferido = payload.PreferredName;
        colaborador.Email = payload.WorkEmail ?? colaborador.Email;
        colaborador.ExternalSource = EmployeeExternalSource.SapCdm;
        colaborador.ExternalEmployeeId = payload.ExternalEmployeeId;
        colaborador.ExternalSystemCode = payload.SourceSystem;
        colaborador.SyncStatus = EmployeeSyncStatus.Synced;
        colaborador.NominaSyncAt = DateTime.UtcNow;
        colaborador.SyncHash = ComputeHash(payload);
        colaborador.UpdatedAt = DateTime.UtcNow;

        if (colaborador.DatosSensibles is null)
        {
            colaborador.DatosSensibles = new ColaboradorDatosSensibles
            {
                Id = Guid.NewGuid(),
                ColaboradorId = colaborador.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };
            _context.ColaboradoresDatosSensibles.Add(colaborador.DatosSensibles);
        }

        if (!string.IsNullOrWhiteSpace(payload.Rfc))
            colaborador.DatosSensibles.Rfc = payload.Rfc;
        colaborador.DatosSensibles.Curp = payload.Curp ?? colaborador.DatosSensibles.Curp;
        colaborador.DatosSensibles.Nss = payload.Nss ?? colaborador.DatosSensibles.Nss;

        _context.MovimientosColaborador.Add(new MovimientoColaborador
        {
            Id = Guid.NewGuid(),
            ColaboradorId = colaborador.Id,
            TipoMovimiento = payload.MovementType ?? (isNew ? "Alta" : "EmployeeUpdate"),
            FechaEfectiva = payload.EffectiveDate ?? DateTime.UtcNow.Date,
            Origen = EmployeeExternalSource.SapCdm,
            ReferenciaExterna = payload.ExternalEmployeeId,
            CreadoPor = performedBy,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        });

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Integrations", "CdmUpsert", "Colaborador", colaborador.Id, performedBy,
            $"Acción: {(isNew ? "Created" : "Updated")}", cancellationToken);

        return new CdmUpsertResultDto(true, colaborador.Id, isNew ? "Created" : "Updated", null);
    }

    private static string ComputeHash(CdmEmployeeUpsertDto p)
    {
        var raw = $"{p.ExternalEmployeeId}|{p.EmployeeNumber}|{p.LegalFirstName}|{p.LegalLastName}|{p.HireDate:O}";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
    }
}
