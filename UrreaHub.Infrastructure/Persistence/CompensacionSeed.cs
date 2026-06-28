using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.Beneficios;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Compensaciones;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Infrastructure.Persistence;

public static class CompensacionSeed
{
    public static async Task SeedAsync(UrreaHubDbContext context)
    {
        await SeedDatosLaboralesAsync(context);
        await SeedTabuladorAsync(context);
        await SeedConceptosAsync(context);
        await SeedBeneficiosAsync(context);
        await SeedDemoSolicitudesAsync(context);
        await context.SaveChangesAsync();
    }

    private static async Task SeedDatosLaboralesAsync(UrreaHubDbContext context)
    {
        if (await context.ColaboradoresDatosLaborales.AnyAsync())
            return;

        var cols = await context.Colaboradores.Where(c => c.IsActive).ToListAsync();
        var now = DateTime.UtcNow;
        var datos = new (string Num, string Grupo, string Nivel, string Jornada, string Turno, bool Sind, string Visibilidad)[]
        {
            ("1001", "DIR", "N7", "Administrativa", "Oficina", false, "Full"),
            ("1002", "OPS", "N5", "Administrativa", "Oficina", false, "Restricted"),
            ("1003", "OPS", "N4", "Administrativa", "Oficina", false, "Summary"),
            ("1004", "OPS", "N3", "Mixta", "Planta A", true, "Restricted"),
            ("1005", "RH", "N6", "Administrativa", "Oficina", false, "Full"),
        };

        foreach (var d in datos)
        {
            var col = cols.FirstOrDefault(c => c.NumeroEmpleado == d.Num);
            if (col is null) continue;
            context.ColaboradoresDatosLaborales.Add(new ColaboradorDatosLaborales
            {
                Id = Guid.NewGuid(),
                ColaboradorId = col.Id,
                GrupoNomina = d.Grupo,
                NivelSalarial = d.Nivel,
                Jornada = d.Jornada,
                Turno = d.Turno,
                Sindicalizado = d.Sind,
                NivelVisibilidadCompensacion = d.Visibilidad,
                CreatedAt = now,
                IsActive = true,
            });
        }
    }

    private static async Task SeedTabuladorAsync(UrreaHubDbContext context)
    {
        if (await context.Tabuladores.AnyAsync())
            return;

        var now = DateTime.UtcNow;
        var tabId = Guid.NewGuid();
        context.Tabuladores.Add(new Tabulador
        {
            Id = tabId,
            Codigo = "TAB-MX-2026",
            Nombre = "Tabulador corporativo MX 2026",
            Descripcion = "Bandas referenciales para niveles N1–N7",
            Moneda = "MXN",
            VigenciaDesde = new DateTime(2026, 1, 1),
            CreatedAt = now,
            IsActive = true,
        });

        var bandas = new (string Nivel, decimal Min, decimal Med, decimal Max)[]
        {
            ("N3", 18000, 22000, 26000),
            ("N4", 24000, 29000, 34000),
            ("N5", 32000, 38000, 44000),
            ("N6", 42000, 50000, 58000),
            ("N7", 55000, 65000, 75000),
        };
        foreach (var b in bandas)
        {
            context.BandasSalariales.Add(new BandaSalarial
            {
                Id = Guid.NewGuid(),
                TabuladorId = tabId,
                Nivel = b.Nivel,
                Minimo = b.Min,
                Medio = b.Med,
                Maximo = b.Max,
                CreatedAt = now,
                IsActive = true,
            });
        }
    }

    private static async Task SeedConceptosAsync(UrreaHubDbContext context)
    {
        if (await context.ConceptosCompensacion.AnyAsync())
            return;

        var now = DateTime.UtcNow;
        var conceptos = new (string Codigo, string Nombre, string Tipo, bool Impacta)[]
        {
            ("SUELDO-BASE", "Sueldo base", "Fijo", true),
            ("BONO-DESEMP", "Bono desempeño", "Variable", true),
            ("VIATICOS", "Viáticos", "Reembolso", false),
            ("PREMIO-PUNT", "Premio puntualidad", "Variable", true),
        };
        foreach (var c in conceptos)
        {
            context.ConceptosCompensacion.Add(new ConceptoCompensacion
            {
                Id = Guid.NewGuid(),
                Codigo = c.Codigo,
                Nombre = c.Nombre,
                Tipo = c.Tipo,
                ImpactaNomina = c.Impacta,
                CreatedAt = now,
                IsActive = true,
            });
        }
    }

    private static async Task SeedBeneficiosAsync(UrreaHubDbContext context)
    {
        if (await context.Beneficios.AnyAsync())
            return;

        var now = DateTime.UtcNow;
        var items = new (string Codigo, string Nombre, string? Desc, decimal? Max)[]
        {
            ("SGMM", "Seguro de gastos médicos mayores", "Cobertura familiar ampliada", null),
            ("VALES", "Vales de despensa", "Tarjeta mensual conforme política", 3000),
            ("AUTO", "Auto de la compañía", "Esquema ejecutivo — sujeto a elegibilidad", null),
            ("GUARD", "Guardería", "Apoyo guardería INFONAVIT", 2500),
            ("PTOS-WELL", "Puntos bienestar", "Canje en tienda interna", null),
        };
        foreach (var b in items)
        {
            context.Beneficios.Add(new Beneficio
            {
                Id = Guid.NewGuid(),
                Codigo = b.Codigo,
                Nombre = b.Nombre,
                Descripcion = b.Desc,
                MontoMaximo = b.Max,
                CreatedAt = now,
                IsActive = true,
            });
        }
    }

    private static async Task SeedDemoSolicitudesAsync(UrreaHubDbContext context)
    {
        if (await context.SolicitudesAjusteCompensacion.AnyAsync())
            return;

        var maria = await context.Colaboradores.FirstOrDefaultAsync(c => c.NumeroEmpleado == "1003");
        var patricia = await context.Colaboradores.FirstOrDefaultAsync(c => c.NumeroEmpleado == "1005");
        if (maria is null || patricia is null) return;

        var now = DateTime.UtcNow;
        var solId = Guid.NewGuid();
        var solicitud = new SolicitudAjusteCompensacion
        {
            Id = solId,
            ColaboradorId = maria.Id,
            SolicitanteId = patricia.Id,
            TipoAjuste = TipoAjusteCompensacion.CambioNivelSalarial,
            Estado = EstadoAjusteCompensacion.EnRevisionDh,
            ValorAnterior = "N4",
            ValorNuevo = "N5",
            Motivo = "Promoción por desempeño — revisión anual",
            FechaSolicitud = now.AddDays(-2),
            RequiereFinanzas = false,
            MontoReferencia = 38000,
            CreatedAt = now,
            IsActive = true,
        };
        solicitud.Aprobaciones.Add(new AprobacionAjusteCompensacion
        {
            Id = Guid.NewGuid(),
            SolicitudId = solId,
            AprobadorId = patricia.Id,
            Orden = 1,
            RolAprobador = "DH",
            Decision = EstadoAjusteCompensacion.EnRevisionDh,
            CreatedAt = now,
            IsActive = true,
        });
        solicitud.Historial.Add(new HistorialAjusteCompensacion
        {
            Id = Guid.NewGuid(),
            SolicitudId = solId,
            Accion = "Enviada",
            Detalle = "Demo Fase 4",
            FechaAccion = now.AddDays(-2),
            UsuarioId = patricia.Id,
            CreatedAt = now,
            IsActive = true,
        });
        context.SolicitudesAjusteCompensacion.Add(solicitud);

        var sgmm = await context.Beneficios.FirstOrDefaultAsync(b => b.Codigo == "SGMM");
        if (sgmm is not null && !await context.SolicitudesBeneficio.AnyAsync())
        {
            context.SolicitudesBeneficio.Add(new SolicitudBeneficio
            {
                Id = Guid.NewGuid(),
                ColaboradorId = maria.Id,
                BeneficioId = sgmm.Id,
                FechaSolicitud = now.AddDays(-5),
                Justificacion = "Alta familiar — cónyuge",
                Estado = EstadoSolicitud.Pendiente,
                CreatedAt = now,
                IsActive = true,
            });
        }
    }
}
