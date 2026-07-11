using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.CoreRH;
using UrreaHub.Domain.Vacaciones;

namespace UrreaHub.Infrastructure.Persistence;

/// <summary>
/// Puebla la base con una plantilla de empresa realista: ~500-700 colaboradores ficticios
/// repartidos en áreas, subáreas, departamentos, sedes y niveles jerárquicos variados.
/// </summary>
public static class MegaEmployeeSeed
{
    private const string MarkerAreaCodigo = "MFG";
    private const int Seed = 20260711;

    private static readonly string[] NombresPool =
    {
        "Ana", "Luis", "María", "Carlos", "Patricia", "José", "Laura", "Miguel", "Sofía", "Jorge",
        "Fernanda", "Roberto", "Daniela", "Alejandro", "Claudia", "Ricardo", "Gabriela", "Francisco", "Verónica", "Eduardo",
        "Adriana", "Javier", "Mónica", "Sergio", "Paola", "Raúl", "Andrea", "Arturo", "Diana", "Héctor",
        "Karla", "Emilio", "Cynthia", "Rodrigo", "Elena", "Manuel", "Silvia", "Guillermo", "Alejandra", "Iván",
    };

    private static readonly string[] ApellidosPool =
    {
        "García", "Martínez", "López", "Hernández", "Ruiz", "González", "Rodríguez", "Pérez", "Sánchez", "Ramírez",
        "Torres", "Flores", "Rivera", "Gómez", "Díaz", "Cruz", "Morales", "Reyes", "Jiménez", "Ortiz",
        "Gutiérrez", "Chávez", "Ramos", "Vázquez", "Castillo", "Romero", "Álvarez", "Mendoza", "Vargas", "Castro",
        "Ortega", "Delgado", "Guerrero", "Medina", "Aguilar", "Salazar", "Contreras", "Herrera", "Domínguez", "Silva",
        "Cervantes", "Cortés", "Rojas", "Fuentes", "Peña", "Núñez", "Luna", "Campos", "Vega", "Soto",
    };

    private sealed record DepartamentoDef(string Codigo, string Nombre);
    private sealed record SubareaDef(string Codigo, string Nombre, List<DepartamentoDef> Departamentos);
    private sealed record AreaDef(string Codigo, string Nombre, string Perfil, List<SubareaDef> Subareas);

    // Perfil "Operativo": la base de niveles inferiores se concentra en Operador/Auxiliar/Supervisor.
    // Perfil "Oficina": la base de niveles inferiores se concentra en Analista/Analista Senior/Coordinador.
    private static readonly List<AreaDef> OrgTree = new()
    {
        new("DG", "Dirección General", "Oficina", new()
        {
            new("DG-EJE", "Dirección Ejecutiva", new() { new("DG-CEO", "Oficina del CEO"), new("DG-PLA", "Planeación Estratégica") }),
            new("DG-AUD", "Auditoría Interna", new() { new("DG-AUD1", "Auditoría Interna"), new("DG-AUD2", "Control Interno") }),
        }),
        new("MFG", "Manufactura", "Operativo", new()
        {
            new("MFG-PRD", "Producción", new() { new("MFG-ENS1", "Línea de Ensamble 1"), new("MFG-ENS2", "Línea de Ensamble 2"), new("MFG-MAQ", "Maquinado"), new("MFG-EMP", "Empaque") }),
            new("MFG-MTO", "Mantenimiento", new() { new("MFG-MTOE", "Mantenimiento Eléctrico"), new("MFG-MTOM", "Mantenimiento Mecánico") }),
            new("MFG-ING", "Ingeniería de Manufactura", new() { new("MFG-ING1", "Ingeniería de Procesos"), new("MFG-ING2", "Mejora Continua") }),
        }),
        new("SCM", "Cadena de Suministro", "Operativo", new()
        {
            new("SCM-LOG", "Logística", new() { new("SCM-ALM", "Almacén"), new("SCM-DIS", "Distribución"), new("SCM-IMP", "Importaciones") }),
            new("SCM-COM", "Compras", new() { new("SCM-COMN", "Compras Nacionales"), new("SCM-COMI", "Compras Internacionales") }),
            new("SCM-PLD", "Planeación de Demanda", new() { new("SCM-PLD1", "Planeación de Demanda"), new("SCM-PLD2", "Inventarios") }),
        }),
        new("QA", "Calidad", "Operativo", new()
        {
            new("QA-ASE", "Aseguramiento de Calidad", new() { new("QA-CTL", "Control de Calidad"), new("QA-LAB", "Laboratorio") }),
            new("QA-SIS", "Sistemas de Gestión", new() { new("QA-CERT", "Certificaciones"), new("QA-MEJ", "Mejora de Procesos") }),
        }),
        new("VYM", "Ventas y Marketing", "Oficina", new()
        {
            new("VYM-VEN", "Ventas", new() { new("VYM-VEN1", "Ventas Nacionales"), new("VYM-VEN2", "Ventas Internacionales"), new("VYM-VEN3", "Cuentas Clave") }),
            new("VYM-MKT", "Marketing", new() { new("VYM-MKT1", "Marketing Digital"), new("VYM-MKT2", "Trade Marketing") }),
            new("VYM-SAC", "Servicio al Cliente", new() { new("VYM-SAC1", "Atención a Clientes"), new("VYM-SAC2", "Postventa") }),
        }),
        new("FIN", "Finanzas", "Oficina", new()
        {
            new("FIN-CTB", "Contabilidad", new() { new("FIN-CTB1", "Contabilidad General"), new("FIN-CTB2", "Impuestos") }),
            new("FIN-TES", "Tesorería", new() { new("FIN-TES1", "Tesorería"), new("FIN-TES2", "Crédito y Cobranza") }),
            new("FIN-FPA", "Planeación Financiera", new() { new("FIN-FPA1", "FP&A"), new("FIN-FPA2", "Costos") }),
        }),
        new("RH", "Recursos Humanos", "Oficina", new()
        {
            new("RH-TAL", "Gestión de Talento", new() { new("RH-REC", "Reclutamiento"), new("RH-CAP", "Capacitación"), new("RH-DOR", "Desarrollo Organizacional") }),
            new("RH-ADM", "Administración de Personal", new() { new("RH-NOM", "Nómina"), new("RH-REL", "Relaciones Laborales") }),
        }),
        new("TI", "Tecnología de Información", "Oficina", new()
        {
            new("TI-INF", "Infraestructura", new() { new("TI-INF1", "Redes y Servidores"), new("TI-INF2", "Soporte Técnico") }),
            new("TI-DEV", "Desarrollo", new() { new("TI-DEV1", "Desarrollo de Software"), new("TI-DEV2", "Datos y BI") }),
        }),
        new("LEG", "Legal y Cumplimiento", "Oficina", new()
        {
            new("LEG-COR", "Legal Corporativo", new() { new("LEG-CON", "Contratos"), new("LEG-LIT", "Litigios") }),
            new("LEG-CMP", "Cumplimiento", new() { new("LEG-CMP1", "Cumplimiento Normativo") }),
        }),
    };

    private static readonly (int Nivel, string Nombre, string Abrev)[] NivelesBase =
    {
        (4, "Coordinador", "COORD"), (5, "Supervisor", "SUPER"), (6, "Analista Senior", "ANSR"),
        (7, "Analista", "ANAL"), (8, "Auxiliar", "AUXI"), (9, "Operador", "OPER"),
    };

    // Pesos de nivel base (4..9) por perfil de área — controla qué tan "de piso" u "oficina" es la mayoría de la plantilla.
    private static readonly Dictionary<string, int[]> PesosPorPerfil = new()
    {
        ["Operativo"] = new[] { 8, 12, 6, 10, 20, 44 }, // Coordinador..Operador
        ["Oficina"] = new[] { 14, 8, 22, 34, 16, 6 },
    };

    public static async Task SeedAsync(UrreaHubDbContext context)
    {
        if (await context.Areas.AnyAsync(a => a.Codigo == MarkerAreaCodigo))
            return;

        var politica = await context.PoliticasVacaciones.FirstOrDefaultAsync();
        if (politica is null)
            return;

        var random = new Random(Seed);
        var now = DateTime.UtcNow;
        // Pre-cargados con lo ya existente (ej. los 5 colaboradores demo de DbSeeder) para no colisionar.
        var usedRfc = new HashSet<string>(await context.ColaboradoresDatosSensibles
            .Where(d => d.Rfc != null)
            .Select(d => d.Rfc!)
            .ToListAsync());
        var usedEmail = new HashSet<string>(await context.Colaboradores.Select(c => c.Email).ToListAsync());
        var numeroEmpleado = 2001;

        // --- Catálogos base ---
        var sedes = new[]
        {
            new Sede { Id = Guid.NewGuid(), Codigo = "MTY-PLT", Nombre = "Monterrey Planta", Ciudad = "Monterrey", Pais = "México", CreatedAt = now, IsActive = true },
            new Sede { Id = Guid.NewGuid(), Codigo = "CDMX-COR", Nombre = "CDMX Corporativo", Ciudad = "Ciudad de México", Pais = "México", CreatedAt = now, IsActive = true },
            new Sede { Id = Guid.NewGuid(), Codigo = "GDL", Nombre = "Guadalajara", Ciudad = "Guadalajara", Pais = "México", CreatedAt = now, IsActive = true },
            new Sede { Id = Guid.NewGuid(), Codigo = "QRO", Nombre = "Querétaro", Ciudad = "Querétaro", Pais = "México", CreatedAt = now, IsActive = true },
            new Sede { Id = Guid.NewGuid(), Codigo = "SAL", Nombre = "Saltillo", Ciudad = "Saltillo", Pais = "México", CreatedAt = now, IsActive = true },
        };
        context.Sedes.AddRange(sedes);

        var relaciones = new[]
        {
            new RelacionLaboral { Id = Guid.NewGuid(), Codigo = "PLANTA-M", Nombre = "Planta", CreatedAt = now, IsActive = true },
            new RelacionLaboral { Id = Guid.NewGuid(), Codigo = "CONFIANZA-M", Nombre = "Confianza", CreatedAt = now, IsActive = true },
            new RelacionLaboral { Id = Guid.NewGuid(), Codigo = "SIND-M", Nombre = "Sindicalizado", CreatedAt = now, IsActive = true },
            new RelacionLaboral { Id = Guid.NewGuid(), Codigo = "TEMP-M", Nombre = "Temporal", CreatedAt = now, IsActive = true },
            new RelacionLaboral { Id = Guid.NewGuid(), Codigo = "HON-M", Nombre = "Honorarios", CreatedAt = now, IsActive = true },
        };
        context.RelacionesLaborales.AddRange(relaciones);

        await context.SaveChangesAsync();

        // --- Áreas / Subáreas / Departamentos ---
        var areas = new List<Area>();
        var subareas = new List<Subarea>();
        var departamentos = new List<Departamento>();
        var centrosCosto = new List<CentroCosto>();

        var departamentosPorArea = new Dictionary<string, List<Departamento>>();
        var puestoDirectorPorArea = new Dictionary<string, Puesto>();
        var puestoGerentePorSubarea = new Dictionary<string, Puesto>();
        var puestoJefePorDepartamento = new Dictionary<string, Puesto>();
        var puestosNivelPorArea = new Dictionary<string, List<Puesto>>();

        var puestos = new List<Puesto>();

        foreach (var areaDef in OrgTree)
        {
            var area = new Area { Id = Guid.NewGuid(), Codigo = areaDef.Codigo, Nombre = areaDef.Nombre, CreatedAt = now, IsActive = true };
            areas.Add(area);
            departamentosPorArea[areaDef.Codigo] = new List<Departamento>();

            var puestoDirector = new Puesto { Id = Guid.NewGuid(), Codigo = $"DIR-{areaDef.Codigo}", Nombre = $"Director de {areaDef.Nombre}", NivelJerarquico = 1, CreatedAt = now, IsActive = true };
            puestos.Add(puestoDirector);
            puestoDirectorPorArea[areaDef.Codigo] = puestoDirector;

            var centroCostoArea = new CentroCosto { Id = Guid.NewGuid(), Codigo = $"CC-{areaDef.Codigo}", Nombre = $"CeCo {areaDef.Nombre}", CreatedAt = now, IsActive = true };
            centrosCosto.Add(centroCostoArea);

            var nivelesArea = new List<Puesto>();
            foreach (var (nivel, nombreNivel, abrev) in NivelesBase)
            {
                var puestoNivel = new Puesto { Id = Guid.NewGuid(), Codigo = $"{abrev}-{areaDef.Codigo}", Nombre = $"{nombreNivel} de {areaDef.Nombre}", NivelJerarquico = nivel, CreatedAt = now, IsActive = true };
                nivelesArea.Add(puestoNivel);
                puestos.Add(puestoNivel);
            }
            puestosNivelPorArea[areaDef.Codigo] = nivelesArea;

            foreach (var subareaDef in areaDef.Subareas)
            {
                var subarea = new Subarea { Id = Guid.NewGuid(), Codigo = subareaDef.Codigo, Nombre = subareaDef.Nombre, AreaId = area.Id, CreatedAt = now, IsActive = true };
                subareas.Add(subarea);

                var puestoGerente = new Puesto { Id = Guid.NewGuid(), Codigo = $"GTE-{subareaDef.Codigo}", Nombre = $"Gerente de {subareaDef.Nombre}", NivelJerarquico = 2, CreatedAt = now, IsActive = true };
                puestos.Add(puestoGerente);
                puestoGerentePorSubarea[subareaDef.Codigo] = puestoGerente;

                var centroCostoSub = new CentroCosto { Id = Guid.NewGuid(), Codigo = $"CC-{subareaDef.Codigo}", Nombre = $"CeCo {subareaDef.Nombre}", CreatedAt = now, IsActive = true };
                centrosCosto.Add(centroCostoSub);

                foreach (var deptoDef in subareaDef.Departamentos)
                {
                    var sede = sedes[random.Next(sedes.Length)];
                    var depto = new Departamento { Id = Guid.NewGuid(), Codigo = deptoDef.Codigo, Nombre = deptoDef.Nombre, SubareaId = subarea.Id, SedeId = sede.Id, CreatedAt = now, IsActive = true };
                    departamentos.Add(depto);
                    departamentosPorArea[areaDef.Codigo].Add(depto);

                    var puestoJefe = new Puesto { Id = Guid.NewGuid(), Codigo = $"JEFE-{deptoDef.Codigo}", Nombre = $"Jefe de {deptoDef.Nombre}", NivelJerarquico = 3, CreatedAt = now, IsActive = true };
                    puestos.Add(puestoJefe);
                    puestoJefePorDepartamento[deptoDef.Codigo] = puestoJefe;
                }
            }
        }

        context.Areas.AddRange(areas);
        context.Subareas.AddRange(subareas);
        context.Departamentos.AddRange(departamentos);
        context.CentrosCosto.AddRange(centrosCosto);
        context.Puestos.AddRange(puestos);
        await context.SaveChangesAsync();

        // --- Colaboradores ---
        var totalObjetivo = random.Next(500, 701);
        var totalDepartamentos = OrgTree.Sum(a => a.Subareas.Sum(s => s.Departamentos.Count));
        var totalLiderazgo = OrgTree.Count + OrgTree.Sum(a => a.Subareas.Count) + totalDepartamentos; // directores + gerentes + jefes
        var baseObjetivo = Math.Max(totalDepartamentos * 3, totalObjetivo - totalLiderazgo);
        var cuotaBase = Math.Max(3, (int)Math.Ceiling(baseObjetivo / (double)totalDepartamentos));
        var colaboradoresPorArea = new List<(Colaborador Colaborador, string Rfc, string AreaCodigo)>();
        var directorGeneral = default(Colaborador);

        Colaborador CrearColaborador(Puesto puesto, Departamento depto, Sede? sede, CentroCosto cc, RelacionLaboral relacion, Guid? jefeId, DateTime fechaIngreso)
        {
            var nombre = NombresPool[random.Next(NombresPool.Length)];
            var apellidoPaterno = ApellidosPool[random.Next(ApellidosPool.Length)];
            var apellidoMaterno = ApellidosPool[random.Next(ApellidosPool.Length)];
            var numero = (numeroEmpleado++).ToString(CultureInfo.InvariantCulture);
            var email = BuildUniqueEmail(nombre, apellidoPaterno, numero, usedEmail);
            var rfc = BuildUniqueRfc(nombre, apellidoPaterno, apellidoMaterno, fechaIngreso, random, usedRfc);

            var esBaja = random.Next(100) < 4;
            DateTime? fechaBaja = esBaja ? fechaIngreso.AddDays(random.Next(90, Math.Max(91, (int)(now - fechaIngreso).TotalDays))) : null;
            if (fechaBaja > now) fechaBaja = now.AddDays(-random.Next(1, 30));

            var colaborador = new Colaborador
            {
                Id = Guid.NewGuid(),
                NumeroEmpleado = numero,
                Nombre = nombre,
                ApellidoPaterno = apellidoPaterno,
                ApellidoMaterno = apellidoMaterno,
                Email = email,
                FechaIngreso = fechaIngreso,
                FechaBaja = fechaBaja,
                PuestoId = puesto.Id,
                DepartamentoId = depto.Id,
                SedeId = sede?.Id ?? depto.SedeId,
                CentroCostoId = cc.Id,
                RelacionLaboralId = relacion.Id,
                JefeDirectoId = jefeId,
                NominaSyncAt = now,
                ExternalSource = EmployeeExternalSource.SapCdm,
                ExternalEmployeeId = $"SAP-{numero}",
                ExternalSystemCode = "SAP",
                SyncStatus = EmployeeSyncStatus.Synced,
                NombrePreferido = nombre,
                CreatedAt = now,
                IsActive = !esBaja,
            };

            colaboradoresPorArea.Add((colaborador, rfc, puesto.Codigo));
            return colaborador;
        }

        var confianza = relaciones[1];
        var sindicalizado = relaciones[2];
        var planta = relaciones[0];
        var temporal = relaciones[3];

        var creados = 0;
        foreach (var areaDef in OrgTree)
        {
            var centroCostoArea = centrosCosto.First(c => c.Codigo == $"CC-{areaDef.Codigo}");
            var director = CrearColaborador(
                puestoDirectorPorArea[areaDef.Codigo],
                departamentosPorArea[areaDef.Codigo][0],
                null, centroCostoArea, confianza,
                jefeId: null,
                fechaIngreso: RandomFechaIngreso(random, now, antiguo: true));

            if (areaDef.Codigo == "DG")
                directorGeneral = director;

            var gerentesPorSubarea = new Dictionary<string, Colaborador>();
            foreach (var subareaDef in areaDef.Subareas)
            {
                var deptoGerente = departamentos.First(d => subareaDef.Departamentos.Select(x => x.Codigo).Contains(d.Codigo));
                var centroCostoSub = centrosCosto.First(c => c.Codigo == $"CC-{subareaDef.Codigo}");
                var gerente = CrearColaborador(
                    puestoGerentePorSubarea[subareaDef.Codigo],
                    deptoGerente,
                    null, centroCostoSub, confianza,
                    jefeId: director.Id,
                    fechaIngreso: RandomFechaIngreso(random, now, antiguo: true));
                gerentesPorSubarea[subareaDef.Codigo] = gerente;

                foreach (var deptoDef in subareaDef.Departamentos)
                {
                    var depto = departamentos.First(d => d.Codigo == deptoDef.Codigo);
                    var jefe = CrearColaborador(
                        puestoJefePorDepartamento[deptoDef.Codigo],
                        depto, null, centroCostoSub, confianza,
                        jefeId: gerente.Id,
                        fechaIngreso: RandomFechaIngreso(random, now, antiguo: false));

                    // Base operativa/administrativa del departamento
                    var pesos = PesosPorPerfil[areaDef.Perfil];
                    var niveles = puestosNivelPorArea[areaDef.Codigo];

                    for (var i = 0; i < cuotaBase && creados < baseObjetivo; i++)
                    {
                        var puestoNivel = niveles[PickWeighted(random, pesos)];
                        var relacion = areaDef.Perfil == "Operativo" && random.Next(100) < 55 ? sindicalizado
                            : random.Next(100) < 10 ? temporal
                            : random.Next(100) < 40 ? planta
                            : confianza;

                        var empleado = CrearColaborador(
                            puestoNivel, depto, null, centroCostoSub, relacion,
                            jefeId: jefe.Id,
                            fechaIngreso: RandomFechaIngreso(random, now, antiguo: false));
                        creados++;

                        context.ColaboradoresDatosLaborales.Add(new ColaboradorDatosLaborales
                        {
                            Id = Guid.NewGuid(),
                            ColaboradorId = empleado.Id,
                            Jornada = areaDef.Perfil == "Operativo" ? PickOne(random, "Diurna", "Mixta", "Nocturna") : "Diurna",
                            Turno = areaDef.Perfil == "Operativo" ? PickOne(random, "Matutino", "Vespertino", "Nocturno") : "Matutino",
                            GrupoNomina = relacion.Codigo == "SIND-M" ? "Semanal" : "Quincenal",
                            Sindicalizado = relacion.Codigo == "SIND-M",
                            NivelSalarial = $"N{puestoNivel.NivelJerarquico}",
                            CreatedAt = now,
                            IsActive = true,
                        });
                    }
                }
            }

            if (director.JefeDirectoId is null && directorGeneral is not null && director.Id != directorGeneral.Id)
                director.JefeDirectoId = directorGeneral.Id;
        }

        context.AddRange(colaboradoresPorArea.Select(x => x.Colaborador));
        await context.SaveChangesAsync();

        // --- Datos sensibles + saldos de vacaciones, en lotes ---
        var anio = now.Year;
        var batch = 0;
        foreach (var (colaborador, rfc, _) in colaboradoresPorArea)
        {
            context.ColaboradoresDatosSensibles.Add(new ColaboradorDatosSensibles
            {
                Id = Guid.NewGuid(),
                ColaboradorId = colaborador.Id,
                Rfc = rfc,
                Enmascarado = false,
                CreatedAt = now,
                IsActive = true,
            });

            context.SaldosVacaciones.Add(new SaldoVacaciones
            {
                Id = Guid.NewGuid(),
                ColaboradorId = colaborador.Id,
                PoliticaId = politica.Id,
                Anio = anio,
                DiasAsignados = politica.DiasAnuales,
                DiasUsados = random.Next(0, politica.DiasAnuales + 1),
                CreatedAt = now,
                IsActive = true,
            });

            if (++batch % 200 == 0)
                await context.SaveChangesAsync();
        }

        await context.SaveChangesAsync();
    }

    private static int PickWeighted(Random random, int[] pesos)
    {
        var total = pesos.Sum();
        var roll = random.Next(total);
        var acumulado = 0;
        for (var i = 0; i < pesos.Length; i++)
        {
            acumulado += pesos[i];
            if (roll < acumulado)
                return i;
        }
        return pesos.Length - 1;
    }

    private static string PickOne(Random random, params string[] opciones) => opciones[random.Next(opciones.Length)];

    private static DateTime RandomFechaIngreso(Random random, DateTime now, bool antiguo)
    {
        // Más densidad en los últimos 2-3 años; posiciones de liderazgo tienden a ser más antiguas.
        var maxAniosAtras = antiguo ? 10 : 8;
        var minAniosAtras = antiguo ? 2 : 0;
        var pesoReciente = random.NextDouble();
        var aniosAtras = antiguo
            ? minAniosAtras + (int)(pesoReciente * (maxAniosAtras - minAniosAtras))
            : (int)(Math.Pow(pesoReciente, 1.8) * maxAniosAtras);

        var fecha = now.AddYears(-aniosAtras).AddDays(-random.Next(0, 365));
        return fecha > now ? now.AddDays(-1) : fecha;
    }

    private static string BuildUniqueEmail(string nombre, string apellido, string numero, HashSet<string> used)
    {
        var baseEmail = $"{StripDiacritics(nombre).ToLowerInvariant()}.{StripDiacritics(apellido).ToLowerInvariant()}";
        var candidate = $"{baseEmail}@urrea.com";
        if (used.Add(candidate))
            return candidate;

        candidate = $"{baseEmail}.{numero}@urrea.com";
        used.Add(candidate);
        return candidate;
    }

    private static string BuildUniqueRfc(string nombre, string apellidoPaterno, string apellidoMaterno, DateTime fechaIngreso, Random random, HashSet<string> used)
    {
        var edadAlIngreso = 22 + random.Next(0, 38);
        var fechaNacimiento = fechaIngreso.AddYears(-edadAlIngreso).AddDays(-random.Next(0, 365));
        var iniciales = $"{StripDiacritics(apellidoPaterno)[..1]}{(apellidoPaterno.Length > 1 ? StripDiacritics(apellidoPaterno)[1..2] : "X")}{StripDiacritics(apellidoMaterno)[..1]}{StripDiacritics(nombre)[..1]}".ToUpperInvariant();
        var fechaTag = fechaNacimiento.ToString("yyMMdd", CultureInfo.InvariantCulture);

        for (var attempt = 0; attempt < 50; attempt++)
        {
            var homoclave = new string(new[] { RandomAlnum(random), RandomAlnum(random), RandomAlnum(random) });
            var rfc = $"{iniciales}{fechaTag}{homoclave}";
            if (used.Add(rfc))
                return rfc;
        }

        var fallback = $"{iniciales}{fechaTag}{used.Count:D3}";
        used.Add(fallback);
        return fallback;
    }

    private static char RandomAlnum(Random random)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return chars[random.Next(chars.Length)];
    }

    private static string StripDiacritics(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
