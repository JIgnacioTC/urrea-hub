using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Auth;
using UrreaHub.Domain.Auditoria;
using UrreaHub.Domain.CoreRH;
using UrreaHub.Domain.Vacaciones;
using UrreaHub.Infrastructure.Services;

namespace UrreaHub.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(UrreaHubDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Colaboradores.AnyAsync())
            return;

        var relacion = new RelacionLaboral { Id = Guid.NewGuid(), Codigo = "PLANTA", Nombre = "Planta", CreatedAt = DateTime.UtcNow, IsActive = true };
        var area = new Area { Id = Guid.NewGuid(), Codigo = "ADM", Nombre = "Administración", CreatedAt = DateTime.UtcNow, IsActive = true };
        var deptoRh = new Departamento { Id = Guid.NewGuid(), Codigo = "RH", Nombre = "Recursos Humanos", AreaId = area.Id, CreatedAt = DateTime.UtcNow, IsActive = true };
        var deptoOps = new Departamento { Id = Guid.NewGuid(), Codigo = "OPS", Nombre = "Operaciones", AreaId = area.Id, CreatedAt = DateTime.UtcNow, IsActive = true };
        var sede = new Sede { Id = Guid.NewGuid(), Codigo = "MTY", Nombre = "Monterrey", Ciudad = "Monterrey", CreatedAt = DateTime.UtcNow, IsActive = true };
        var cc = new CentroCosto { Id = Guid.NewGuid(), Codigo = "CC001", Nombre = "Corporativo", CreatedAt = DateTime.UtcNow, IsActive = true };
        var puestoDir = new Puesto { Id = Guid.NewGuid(), Codigo = "DIR", Nombre = "Director", NivelJerarquico = 1, CreatedAt = DateTime.UtcNow, IsActive = true };
        var puestoJefe = new Puesto { Id = Guid.NewGuid(), Codigo = "JEFE", Nombre = "Jefe de área", NivelJerarquico = 2, CreatedAt = DateTime.UtcNow, IsActive = true };
        var puestoAnalista = new Puesto { Id = Guid.NewGuid(), Codigo = "ANAL", Nombre = "Analista", NivelJerarquico = 3, CreatedAt = DateTime.UtcNow, IsActive = true };

        context.AddRange(relacion, area, deptoRh, deptoOps, sede, cc, puestoDir, puestoJefe, puestoAnalista);

        var politica = new PoliticaVacaciones { Id = Guid.NewGuid(), Nombre = "Estándar URREA", DiasAnuales = 12, AntiguedadMinimaMeses = 0, Acumulable = false, CreatedAt = DateTime.UtcNow, IsActive = true };
        var calendario = new CalendarioLaboral { Id = Guid.NewGuid(), Nombre = "México 2026", Anio = 2026, SedeId = sede.Id, CreatedAt = DateTime.UtcNow, IsActive = true };
        var diasInhabiles = new[]
        {
            new DiaInhabil { Id = Guid.NewGuid(), CalendarioId = calendario.Id, Fecha = new DateTime(2026, 1, 1), Descripcion = "Año Nuevo", EsOficial = true, CreatedAt = DateTime.UtcNow, IsActive = true },
            new DiaInhabil { Id = Guid.NewGuid(), CalendarioId = calendario.Id, Fecha = new DateTime(2026, 2, 2), Descripcion = "Constitución", EsOficial = true, CreatedAt = DateTime.UtcNow, IsActive = true },
            new DiaInhabil { Id = Guid.NewGuid(), CalendarioId = calendario.Id, Fecha = new DateTime(2026, 12, 25), Descripcion = "Navidad", EsOficial = true, CreatedAt = DateTime.UtcNow, IsActive = true },
        };
        context.AddRange(politica, calendario);
        context.AddRange(diasInhabiles);
        await context.SaveChangesAsync();
        await PermisosCatalog.UpsertTiposAsync(context);

        var colaboradoresData = new[]
        {
            CreateColaborador("1001", "Ana", "García", "ana.garcia@urrea.com", "GARA800101ABC", puestoDir, deptoRh, sede, cc, relacion, null),
            CreateColaborador("1002", "Luis", "Martínez", "luis.martinez@urrea.com", "MARL850215DEF", puestoJefe, deptoOps, sede, cc, relacion, director: null),
            CreateColaborador("1003", "María", "López", "maria.lopez@urrea.com", "LOPM900320GHI", puestoAnalista, deptoOps, sede, cc, relacion, director: null),
            CreateColaborador("1004", "Carlos", "Hernández", "carlos.hernandez@urrea.com", "HERC920410JKL", puestoAnalista, deptoOps, sede, cc, relacion, director: null),
            CreateColaborador("1005", "Patricia", "Ruiz", "patricia.ruiz@urrea.com", "RUIP880505MNO", puestoJefe, deptoRh, sede, cc, relacion, director: null),
        };

        var director = colaboradoresData[0].Colaborador;
        var jefe = colaboradoresData[1].Colaborador;
        jefe.JefeDirectoId = director.Id;
        colaboradoresData[2].Colaborador.JefeDirectoId = jefe.Id;
        colaboradoresData[3].Colaborador.JefeDirectoId = jefe.Id;
        colaboradoresData[4].Colaborador.JefeDirectoId = director.Id;

        context.AddRange(colaboradoresData.Select(x => x.Colaborador));

        foreach (var entry in colaboradoresData)
        {
            var c = entry.Colaborador;
            c.ExternalSource = EmployeeExternalSource.SapCdm;
            c.ExternalEmployeeId = $"SAP-{c.NumeroEmpleado}";
            c.ExternalSystemCode = "SAP";
            c.SyncStatus = EmployeeSyncStatus.Synced;
            c.NombrePreferido = c.Nombre;

            context.ColaboradoresDatosSensibles.Add(new ColaboradorDatosSensibles
            {
                Id = Guid.NewGuid(),
                ColaboradorId = c.Id,
                Rfc = entry.Rfc,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Enmascarado = false,
            });
        }

        var colaboradores = colaboradoresData.Select(x => x.Colaborador).ToArray();
        foreach (var c in colaboradores)
        {
            context.CuentasAcceso.Add(new CuentaAcceso
            {
                Id = Guid.NewGuid(),
                ColaboradorId = c.Id,
                PasswordHash = passwordHasher.Hash("Urrea2026!"),
                DebeCambiarPassword = false,
                EsRhAdmin = c.NumeroEmpleado == "1005",
                EsTiAdmin = c.NumeroEmpleado == "1005",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            context.SaldosVacaciones.Add(new SaldoVacaciones
            {
                Id = Guid.NewGuid(),
                ColaboradorId = c.Id,
                PoliticaId = politica.Id,
                Anio = 2026,
                DiasAsignados = 12,
                DiasUsados = 0,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });
        }

        context.Integraciones.Add(new Integracion
        {
            Id = Guid.NewGuid(),
            Nombre = "Sync Nómina",
            SistemaExterno = "Stub",
            Estado = Domain.Common.EstadoIntegracion.Exitosa,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });

        foreach (var c in colaboradores)
        {
            context.MovimientosColaborador.Add(new MovimientoColaborador
            {
                Id = Guid.NewGuid(),
                ColaboradorId = c.Id,
                TipoMovimiento = "Alta",
                FechaEfectiva = c.FechaIngreso,
                ValorNuevo = $"Ingreso · {c.NumeroEmpleado}",
                Origen = EmployeeExternalSource.SapCdm,
                ReferenciaExterna = c.ExternalEmployeeId,
                CreadoPor = "CDM",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            });
        }

        await context.SaveChangesAsync();
    }

    private static (Colaborador Colaborador, string Rfc) CreateColaborador(
        string numero, string nombre, string apellido, string email, string rfc,
        Puesto puesto, Departamento depto, Sede sede, CentroCosto cc, RelacionLaboral relacion, Guid? director)
    {
        return (new Colaborador
        {
            Id = Guid.NewGuid(),
            NumeroEmpleado = numero,
            Nombre = nombre,
            ApellidoPaterno = apellido,
            Email = email,
            FechaIngreso = new DateTime(2020, 1, 15),
            PuestoId = puesto.Id,
            DepartamentoId = depto.Id,
            SedeId = sede.Id,
            CentroCostoId = cc.Id,
            RelacionLaboralId = relacion.Id,
            JefeDirectoId = director,
            NominaSyncAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        }, rfc);
    }
}
