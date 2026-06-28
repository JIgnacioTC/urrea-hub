using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.Catalogos;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Persistence;

public static class CatalogSeed
{
    public static async Task SeedAsync(UrreaHubDbContext context)
    {
        if (await context.CatalogosEstados.AnyAsync())
            return;

        var now = DateTime.UtcNow;
        var estados = CreateEstados(now);
        context.CatalogosEstados.AddRange(estados);
        await context.SaveChangesAsync();

        var estadoByCode = estados.ToDictionary(e => e.Codigo);
        context.CatalogosMunicipios.AddRange(CreateMunicipios(estadoByCode, now));
        context.CatalogosEstadosCiviles.AddRange(CreateEstadosCiviles(now));
        context.RazonesTermino.AddRange(CreateRazonesTermino(now));
        context.CatalogosJerarquias.AddRange(CreateJerarquias(now));

        var nle = estadoByCode["NLE"];
        context.RegistrosPatronales.Add(new RegistroPatronal
        {
            Id = Guid.NewGuid(),
            Codigo = "RP-URREA-MTY",
            NumeroImss = "Y1234567890",
            RazonSocial = "Grupo URREA S.A. de C.V.",
            Rfc = "GUR850101ABC",
            EstadoId = nle.Id,
            Activo = true,
            CreatedAt = now,
            IsActive = true,
        });

        await context.SaveChangesAsync();
    }

    private static List<CatalogoEstado> CreateEstados(DateTime now) =>
        new[]
        {
            ("AGU", "Aguascalientes"), ("BCN", "Baja California"), ("BCS", "Baja California Sur"),
            ("CAM", "Campeche"), ("CHP", "Chiapas"), ("CHH", "Chihuahua"), ("CMX", "Ciudad de México"),
            ("COA", "Coahuila"), ("COL", "Colima"), ("DUR", "Durango"), ("GUA", "Guanajuato"),
            ("GRO", "Guerrero"), ("HID", "Hidalgo"), ("JAL", "Jalisco"), ("MEX", "México"),
            ("MIC", "Michoacán"), ("MOR", "Morelos"), ("NAY", "Nayarit"), ("NLE", "Nuevo León"),
            ("OAX", "Oaxaca"), ("PUE", "Puebla"), ("QUE", "Querétaro"), ("ROO", "Quintana Roo"),
            ("SLP", "San Luis Potosí"), ("SIN", "Sinaloa"), ("SON", "Sonora"), ("TAB", "Tabasco"),
            ("TAM", "Tamaulipas"), ("TLA", "Tlaxcala"), ("VER", "Veracruz"), ("YUC", "Yucatán"),
            ("ZAC", "Zacatecas"),
        }.Select(t => new CatalogoEstado
        {
            Id = Guid.NewGuid(),
            Codigo = t.Item1,
            Nombre = t.Item2,
            Pais = "MEX",
            CreatedAt = now,
            IsActive = true,
        }).ToList();

    private static IEnumerable<CatalogoMunicipio> CreateMunicipios(
        IReadOnlyDictionary<string, CatalogoEstado> estados,
        DateTime now)
    {
        var capitales = new (string Estado, string Codigo, string Nombre)[]
        {
            ("AGU", "001", "Aguascalientes"), ("BCN", "004", "Tijuana"), ("BCS", "003", "La Paz"),
            ("CAM", "002", "Campeche"), ("CHP", "101", "Tuxtla Gutiérrez"), ("CHH", "019", "Chihuahua"),
            ("CMX", "015", "Cuauhtémoc"), ("COA", "030", "Saltillo"), ("COL", "006", "Colima"),
            ("DUR", "005", "Durango"), ("GUA", "017", "León"), ("GRO", "001", "Acapulco de Juárez"),
            ("HID", "048", "Pachuca de Soto"), ("JAL", "039", "Guadalajara"), ("MEX", "106", "Toluca"),
            ("MIC", "053", "Morelia"), ("MOR", "007", "Cuernavaca"), ("NAY", "017", "Tepic"),
            ("NLE", "039", "Monterrey"), ("OAX", "067", "Oaxaca de Juárez"), ("PUE", "114", "Puebla"),
            ("QUE", "014", "Querétaro"), ("ROO", "005", "Benito Juárez"), ("SLP", "028", "San Luis Potosí"),
            ("SIN", "006", "Culiacán"), ("SON", "030", "Hermosillo"), ("TAB", "004", "Centro"),
            ("TAM", "041", "Victoria"), ("TLA", "033", "Tlaxcala"), ("VER", "193", "Veracruz"),
            ("YUC", "050", "Mérida"), ("ZAC", "056", "Zacatecas"),
        };

        foreach (var (estadoCodigo, codigo, nombre) in capitales)
        {
            yield return new CatalogoMunicipio
            {
                Id = Guid.NewGuid(),
                EstadoId = estados[estadoCodigo].Id,
                Codigo = codigo,
                Nombre = nombre,
                CreatedAt = now,
                IsActive = true,
            };
        }

        var nle = estados["NLE"];
        foreach (var (codigo, nombre) in new[] { ("046", "San Nicolás de los Garza"), ("020", "Guadalupe"), ("048", "Santa Catarina"), ("006", "Apodaca") })
        {
            yield return new CatalogoMunicipio
            {
                Id = Guid.NewGuid(),
                EstadoId = nle.Id,
                Codigo = codigo,
                Nombre = nombre,
                CreatedAt = now,
                IsActive = true,
            };
        }
    }

    private static IEnumerable<CatalogoEstadoCivil> CreateEstadosCiviles(DateTime now) =>
        new (string Codigo, string Nombre, int Orden)[]
        {
            ("SOL", "Soltero(a)", 1),
            ("CAS", "Casado(a)", 2),
            ("UL", "Unión libre", 3),
            ("DIV", "Divorciado(a)", 4),
            ("VIU", "Viudo(a)", 5),
            ("SEP", "Separado(a)", 6),
        }.Select(t => new CatalogoEstadoCivil
        {
            Id = Guid.NewGuid(),
            Codigo = t.Codigo,
            Nombre = t.Nombre,
            Orden = t.Orden,
            CreatedAt = now,
            IsActive = true,
        });

    private static IEnumerable<RazonTermino> CreateRazonesTermino(DateTime now) =>
        new (string Codigo, string Nombre, bool RequiereComprobante)[]
        {
            ("REN-VOL", "Renuncia voluntaria", false),
            ("DESP-JUS", "Despido justificado", true),
            ("DESP-INJ", "Despido injustificado", true),
            ("FIN-CONT", "Fin de contrato", false),
            ("JUB", "Jubilación", true),
            ("FALL", "Fallecimiento", true),
            ("ABAN", "Abandono de empleo", false),
            ("MUTUO", "Mutuo acuerdo", false),
        }.Select(t => new RazonTermino
        {
            Id = Guid.NewGuid(),
            Codigo = t.Codigo,
            Nombre = t.Nombre,
            RequiereComprobante = t.RequiereComprobante,
            Activo = true,
            CreatedAt = now,
            IsActive = true,
        });

    private static IEnumerable<CatalogoJerarquia> CreateJerarquias(DateTime now) =>
        new (string Codigo, string Nombre, int Nivel)[]
        {
            ("EJEC", "Ejecutivo", 1),
            ("DIR", "Director", 2),
            ("GER", "Gerente", 3),
            ("JEF", "Jefe", 4),
            ("COORD", "Coordinador", 5),
            ("ANAL", "Analista", 6),
            ("OPER", "Operativo", 7),
        }.Select(t => new CatalogoJerarquia
        {
            Id = Guid.NewGuid(),
            Codigo = t.Codigo,
            Nombre = t.Nombre,
            NivelOrden = t.Nivel,
            CreatedAt = now,
            IsActive = true,
        });
}
