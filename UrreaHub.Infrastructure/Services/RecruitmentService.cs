using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Common;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Application.Onboarding;
using UrreaHub.Application.Reclutamiento;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;
using UrreaHub.Domain.Reclutamiento;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class RecruitmentService : IRecruitmentService
{
    private readonly UrreaHubDbContext _context;
    private readonly IAuditService _audit;
    private readonly IOnboardingAdminService _onboardingAdmin;

    public RecruitmentService(UrreaHubDbContext context, IAuditService audit, IOnboardingAdminService onboardingAdmin)
    {
        _context = context;
        _audit = audit;
        _onboardingAdmin = onboardingAdmin;
    }

    public async Task<RecruitmentDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var vacantes = await _context.VacantesReclutamiento.AsNoTracking()
            .Include(v => v.Postulaciones)
            .Include(v => v.Requisicion)
            .Where(v => v.IsActive)
            .ToListAsync(cancellationToken);

        var postulaciones = await _context.Postulaciones.AsNoTracking()
            .Include(p => p.Entrevistas)
            .Include(p => p.Oferta)
            .Where(p => p.IsActive && p.Estado != EstadoPostulacion.Rechazado && p.Estado != EstadoPostulacion.Contratado)
            .ToListAsync(cancellationToken);

        var requisicionesPorConvertir = await _context.RequisicionesPersonal.CountAsync(
            r => r.IsActive && r.Estado == EstadoSolicitud.Aprobada, cancellationToken);

        var now = DateTime.UtcNow;
        return new RecruitmentDashboardDto(
            vacantes.Count(v => v.Estado == EstadoVacante.Abierta),
            postulaciones.Count,
            postulaciones.SelectMany(p => p.Entrevistas).Count(e => e.FechaHora > now),
            postulaciones.Count(p => p.Oferta != null && !p.Oferta.Aceptada),
            postulaciones.Count(p => p.Oferta != null && p.Oferta.Aceptada),
            requisicionesPorConvertir,
            vacantes.OrderByDescending(v => v.FechaPublicacion).Take(6).Select(MapVacante).ToList());
    }

    public async Task<IReadOnlyList<VacanteDto>> ListVacanciesAsync(EstadoVacante? estado, CancellationToken cancellationToken = default)
    {
        var query = _context.VacantesReclutamiento.AsNoTracking()
            .Include(v => v.Postulaciones)
            .Include(v => v.Requisicion)
            .Where(v => v.IsActive);
        if (estado.HasValue) query = query.Where(v => v.Estado == estado.Value);
        var items = await query.OrderByDescending(v => v.FechaPublicacion).ToListAsync(cancellationToken);
        return items.Select(MapVacante).ToList();
    }

    public async Task<VacanteDto?> GetVacancyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var v = await _context.VacantesReclutamiento.AsNoTracking()
            .Include(x => x.Postulaciones)
            .Include(x => x.Requisicion)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
        return v is null ? null : MapVacante(v);
    }

    public async Task<IReadOnlyList<PostulacionDto>> ListPipelineAsync(Guid? vacanteId, CancellationToken cancellationToken = default)
    {
        var query = _context.Postulaciones.AsNoTracking()
            .Include(p => p.Candidato)
            .Include(p => p.Vacante)
            .Include(p => p.Entrevistas)
            .Include(p => p.Evaluaciones)
            .Where(p => p.IsActive);
        if (vacanteId.HasValue) query = query.Where(p => p.VacanteId == vacanteId.Value);
        var items = await query.OrderByDescending(p => p.FechaPostulacion).ToListAsync(cancellationToken);
        return items.Select(MapPostulacion).ToList();
    }

    public async Task<CandidatoDto?> GetCandidateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var c = await _context.Candidatos.AsNoTracking()
            .Include(x => x.Postulaciones)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
        return c is null ? null : new CandidatoDto(
            c.Id,
            $"{c.Nombre} {c.ApellidoPaterno} {c.ApellidoMaterno}".Trim(),
            c.Email,
            c.Telefono,
            c.Postulaciones.Count(p => p.IsActive));
    }

    public async Task<Result<PostulacionDto>> CreateCandidateAndApplyAsync(CrearCandidatoDto dto, CancellationToken cancellationToken = default)
    {
        var vacante = await _context.VacantesReclutamiento.FirstOrDefaultAsync(v => v.Id == dto.VacanteId && v.IsActive, cancellationToken);
        if (vacante is null) return Result<PostulacionDto>.Fail("Vacante no encontrada.");

        var now = DateTime.UtcNow;
        var candidatoId = Guid.NewGuid();
        var candidato = new Candidato
        {
            Id = candidatoId,
            Nombre = dto.Nombre,
            ApellidoPaterno = dto.ApellidoPaterno,
            ApellidoMaterno = dto.ApellidoMaterno,
            Email = dto.Email,
            Telefono = dto.Telefono,
            CreatedAt = now,
            IsActive = true,
        };

        var postulacion = new Postulacion
        {
            Id = Guid.NewGuid(),
            VacanteId = dto.VacanteId,
            CandidatoId = candidatoId,
            FechaPostulacion = now,
            Estado = EstadoPostulacion.Recibida,
            CreatedAt = now,
            IsActive = true,
        };

        _context.Candidatos.Add(candidato);
        _context.Postulaciones.Add(postulacion);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.Postulaciones.AsNoTracking()
            .Include(p => p.Candidato)
            .Include(p => p.Vacante)
            .Include(p => p.Entrevistas)
            .Include(p => p.Evaluaciones)
            .FirstAsync(p => p.Id == postulacion.Id, cancellationToken);
        return Result<PostulacionDto>.Ok(MapPostulacion(created));
    }

    public async Task<Result<PostulacionDto>> ChangeStageAsync(Guid postulacionId, CambiarEtapaDto dto, CancellationToken cancellationToken = default)
    {
        var p = await _context.Postulaciones
            .Include(x => x.Candidato)
            .Include(x => x.Vacante)
            .FirstOrDefaultAsync(x => x.Id == postulacionId && x.IsActive, cancellationToken);
        if (p is null) return Result<PostulacionDto>.Fail("Postulación no encontrada.");

        p.Estado = dto.Estado;
        p.Notas = dto.Notas ?? p.Notas;
        p.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        var updated = await _context.Postulaciones.AsNoTracking()
            .Include(x => x.Candidato).Include(x => x.Vacante).Include(x => x.Entrevistas).Include(x => x.Evaluaciones)
            .FirstAsync(x => x.Id == postulacionId, cancellationToken);
        return Result<PostulacionDto>.Ok(MapPostulacion(updated));
    }

    public async Task<Result<EntrevistaDto>> ScheduleInterviewAsync(Guid postulacionId, CrearEntrevistaDto dto, CancellationToken cancellationToken = default)
    {
        var p = await _context.Postulaciones.FirstOrDefaultAsync(x => x.Id == postulacionId && x.IsActive, cancellationToken);
        if (p is null) return Result<EntrevistaDto>.Fail("Postulación no encontrada.");

        var now = DateTime.UtcNow;
        var ent = new Entrevista
        {
            Id = Guid.NewGuid(),
            PostulacionId = postulacionId,
            FechaHora = dto.FechaHora,
            Tipo = dto.Tipo,
            Ubicacion = dto.Ubicacion,
            Notas = dto.Notas,
            CreatedAt = now,
            IsActive = true,
        };
        if (p.Estado == EstadoPostulacion.Recibida || p.Estado == EstadoPostulacion.EnRevision)
            p.Estado = EstadoPostulacion.Entrevista;

        _context.Entrevistas.Add(ent);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<EntrevistaDto>.Ok(new EntrevistaDto(ent.Id, ent.FechaHora, ent.Tipo, ent.Ubicacion, ent.Notas));
    }

    public async Task<Result<OfertaDto>> CreateOfferAsync(Guid postulacionId, CrearOfertaDto dto, CancellationToken cancellationToken = default)
    {
        var p = await _context.Postulaciones
            .Include(x => x.Candidato)
            .Include(x => x.Vacante)
            .Include(x => x.Oferta)
            .FirstOrDefaultAsync(x => x.Id == postulacionId && x.IsActive, cancellationToken);
        if (p is null) return Result<OfertaDto>.Fail("Postulación no encontrada.");
        if (p.Oferta != null) return Result<OfertaDto>.Fail("Ya existe oferta para esta postulación.");

        var now = DateTime.UtcNow;
        var oferta = new OfertaLaboral
        {
            Id = Guid.NewGuid(),
            PostulacionId = postulacionId,
            SalarioOfrecido = dto.SalarioOfrecido,
            Moneda = dto.Moneda,
            FechaOferta = now,
            Aceptada = false,
            CreatedAt = now,
            IsActive = true,
        };
        p.Estado = EstadoPostulacion.Oferta;
        _context.OfertasLaborales.Add(oferta);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<OfertaDto>.Ok(MapOferta(oferta, p));
    }

    public async Task<Result<EnviarOnboardingResultDto>> AcceptOfferAndSendToOnboardingAsync(Guid ofertaId, string performedBy, CancellationToken cancellationToken = default)
    {
        var oferta = await _context.OfertasLaborales
            .Include(o => o.Postulacion).ThenInclude(p => p.Candidato)
            .Include(o => o.Postulacion).ThenInclude(p => p.Vacante).ThenInclude(v => v!.Requisicion)
            .FirstOrDefaultAsync(o => o.Id == ofertaId && o.IsActive, cancellationToken);
        if (oferta is null) return Result<EnviarOnboardingResultDto>.Fail("Oferta no encontrada.");

        var post = oferta.Postulacion;
        var cand = post.Candidato;
        var now = DateTime.UtcNow;

        oferta.Aceptada = true;
        oferta.FechaRespuesta = now;
        post.Estado = EstadoPostulacion.Contratado;
        post.UpdatedAt = now;

        var colaborador = await BuildColaboradorFromCandidateAsync(cand, post.Vacante, cancellationToken);
        _context.Colaboradores.Add(colaborador);
        _context.MovimientosColaborador.Add(new MovimientoColaborador
        {
            Id = Guid.NewGuid(),
            ColaboradorId = colaborador.Id,
            TipoMovimiento = "Alta",
            FechaEfectiva = now.Date,
            ValorNuevo = $"Alta desde reclutamiento · {post.Vacante.Codigo}",
            Origen = "Reclutamiento",
            CreadoPor = "DH",
            CreatedAt = now,
            IsActive = true,
        });
        await _context.SaveChangesAsync(cancellationToken);

        var planResult = await _onboardingAdmin.CreatePlanAsync(new CrearPlanOnboardingDto(
            colaborador.Id,
            "ADMIN",
            now.Date,
            $"Onboarding — {post.Vacante.Titulo}"), performedBy, cancellationToken);

        if (!planResult.Success)
            return Result<EnviarOnboardingResultDto>.Fail(planResult.Error ?? "Error al crear plan de onboarding.");

        await _audit.LogEventoAsync("Reclutamiento", "EnviarOnboarding", "OfertaLaboral", ofertaId, performedBy, colaborador.NumeroEmpleado, cancellationToken);
        return Result<EnviarOnboardingResultDto>.Ok(new EnviarOnboardingResultDto(colaborador.Id, planResult.Data!.Id, colaborador.NumeroEmpleado));
    }

    public async Task<IReadOnlyList<OfertaDto>> ListOffersAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.OfertasLaborales.AsNoTracking()
            .Include(o => o.Postulacion).ThenInclude(p => p.Candidato)
            .Include(o => o.Postulacion).ThenInclude(p => p.Vacante)
            .Where(o => o.IsActive)
            .OrderByDescending(o => o.FechaOferta)
            .ToListAsync(cancellationToken);
        return items.Select(o => MapOferta(o, o.Postulacion)).ToList();
    }

    private async Task<Colaborador> BuildColaboradorFromCandidateAsync(Candidato cand, VacanteReclutamiento vacante, CancellationToken cancellationToken)
    {
        var template = await _context.Colaboradores.AsNoTracking().FirstAsync(c => c.NumeroEmpleado == "1003", cancellationToken);
        var maxNum = await _context.Colaboradores.MaxAsync(c => c.NumeroEmpleado, cancellationToken);
        var nextNum = (int.Parse(maxNum) + 1).ToString();
        var now = DateTime.UtcNow;
        var id = Guid.NewGuid();

        var colaborador = new Colaborador
        {
            Id = id,
            NumeroEmpleado = nextNum,
            Nombre = cand.Nombre,
            ApellidoPaterno = cand.ApellidoPaterno,
            ApellidoMaterno = cand.ApellidoMaterno,
            NombrePreferido = cand.Nombre,
            Email = cand.Email,
            Telefono = cand.Telefono,
            FechaIngreso = now.Date,
            PuestoId = template.PuestoId,
            DepartamentoId = template.DepartamentoId,
            SedeId = template.SedeId,
            CentroCostoId = template.CentroCostoId,
            RelacionLaboralId = template.RelacionLaboralId,
            JefeDirectoId = template.JefeDirectoId,
            ExternalSource = EmployeeExternalSource.Manual,
            SyncStatus = EmployeeSyncStatus.Pending,
            CreatedAt = now,
            IsActive = true,
        };
        return colaborador;
    }

    internal static VacanteDto MapVacante(VacanteReclutamiento v)
    {
        var posts = v.Postulaciones.Where(p => p.IsActive).ToList();
        var etapa = posts.GroupBy(p => p.Estado).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key.ToString();
        return new VacanteDto(
            v.Id,
            v.Codigo,
            v.Titulo,
            v.Estado.ToString(),
            v.FechaPublicacion,
            v.FechaCierre,
            v.RequisicionId,
            v.Requisicion?.Folio,
            posts.Count,
            etapa);
    }

    internal static PostulacionDto MapPostulacion(Postulacion p)
    {
        var score = p.Evaluaciones.Where(e => e.IsActive).Select(e => e.Puntuacion).DefaultIfEmpty().Average();
        var nextInterview = p.Entrevistas.Where(e => e.IsActive).OrderBy(e => e.FechaHora).FirstOrDefault(e => e.FechaHora >= DateTime.UtcNow);
        return new PostulacionDto(
            p.Id,
            p.VacanteId,
            p.Vacante.Titulo,
            p.CandidatoId,
            $"{p.Candidato.Nombre} {p.Candidato.ApellidoPaterno}",
            p.Estado.ToString(),
            p.FechaPostulacion,
            p.Evaluaciones.Any() ? score : null,
            nextInterview?.FechaHora,
            p.Notas);
    }

    internal static OfertaDto MapOferta(OfertaLaboral o, Postulacion p) => new(
        o.Id,
        p.Id,
        $"{p.Candidato.Nombre} {p.Candidato.ApellidoPaterno}",
        p.Vacante.Titulo,
        o.SalarioOfrecido,
        o.Moneda,
        o.FechaOferta,
        o.Aceptada,
        o.FechaRespuesta);
}
