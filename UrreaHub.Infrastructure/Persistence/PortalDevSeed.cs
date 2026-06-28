using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.Auditoria;
using UrreaHub.Domain.Beneficios;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Portal;

namespace UrreaHub.Infrastructure.Persistence;

public static class PortalDevSeed
{
    public const int PuntosIniciales = 850;

    public static async Task SeedAsync(UrreaHubDbContext context)
    {
        if (await context.PublicacionesPortal.AnyAsync())
            return;

        var now = DateTime.UtcNow;
        SeedPublicaciones(context, now);
        SeedDocumentos(context, now);
        SeedFestivos(context, now);
        SeedConvenios(context, now);
        SeedProductos(context, now);
        SeedModulos(context, now);
        await SeedIntegracionesAsync(context, now);
        await SeedSaldosPuntosAsync(context, now);
        await context.SaveChangesAsync();
    }

    private static void SeedPublicaciones(UrreaHubDbContext context, DateTime now)
    {
        context.PublicacionesPortal.AddRange(
            Post("RH", "Recursos Humanos", "Comunicación interna", "Corporativo",
                "¡Bienvenidos al nuevo URREA Hub! Aquí encontrarán vacaciones, permisos, beneficios y todo lo que necesitan en un solo lugar. Compartan sus ideas en los comentarios.",
                "from-urrea-primary to-urrea-secondary", 48, 12, 5, new DateTime(2026, 6, 24, 9, 0, 0, DateTimeKind.Utc),
                TipoPublicacionPortal.Announcement, now),
            Post("PR", "Patricia Ruiz", "Jefe de RH", "Recursos Humanos",
                "Reconocemos al equipo de Operaciones MTY por superar el objetivo de seguridad del trimestre. ¡Gracias por cuidar a la familia URREA!",
                null, 86, 23, 8, new DateTime(2026, 6, 23, 14, 30, 0, DateTimeKind.Utc),
                TipoPublicacionPortal.Recognition, now),
            Post("LM", "Luis Martínez", "Jefe de área", "Operaciones",
                "El viernes tendremos junta de equipo híbrida: presencial en sala B y enlace por Teams. Confirmen asistencia antes del jueves.",
                null, 15, 9, 2, new DateTime(2026, 6, 22, 11, 15, 0, DateTimeKind.Utc),
                TipoPublicacionPortal.Event, now),
            Post("CC", "Comité de Cultura", "Eventos", "Corporativo",
                "Registro abierto para el Día de la Familia URREA 2026. Revisa Procesos Internos para el reglamento y confirma tu participación.",
                "from-teal-600 to-urrea-secondary", 34, 18, 11, new DateTime(2026, 6, 21, 8, 0, 0, DateTimeKind.Utc),
                TipoPublicacionPortal.Event, now),
            Post("AG", "Ana García", "Directora", "Dirección General",
                "La innovación no es solo producto: también es cómo nos organizamos y nos cuidamos. Sigan compartiendo buenas prácticas en este espacio.",
                null, 62, 7, 4, new DateTime(2026, 6, 20, 16, 45, 0, DateTimeKind.Utc),
                TipoPublicacionPortal.General, now));
    }

    private static PublicacionPortal Post(
        string iniciales, string nombre, string rol, string depto, string contenido,
        string? gradiente, int likes, int comments, int shares, DateTime fecha,
        TipoPublicacionPortal tipo, DateTime now) => new()
    {
        Id = Guid.NewGuid(),
        AutorIniciales = iniciales,
        AutorNombre = nombre,
        AutorRol = rol,
        Departamento = depto,
        Contenido = contenido,
        GradienteImagen = gradiente,
        Likes = likes,
        Comentarios = comments,
        Compartidos = shares,
        FechaPublicacion = fecha,
        Tipo = tipo,
        CreatedAt = now,
        IsActive = true,
    };

    private static void SeedDocumentos(UrreaHubDbContext context, DateTime now)
    {
        var docs = new (string codigo, CategoriaDocumentoCorporativo cat, string titulo, string desc, string icono, DateTime fecha, int? paginas, int orden)[]
        {
            ("reg-interno", CategoriaDocumentoCorporativo.Reglamentos, "Reglamento Interior de Trabajo", "Normas de conducta, horarios, permisos y obligaciones en URREA.", "📜", new DateTime(2026, 1, 15), 42, 1),
            ("reg-hig", CategoriaDocumentoCorporativo.Reglamentos, "Reglamento de Seguridad e Higiene", "Protocolos de seguridad industrial y prevención de riesgos.", "🦺", new DateTime(2025, 11, 20), 28, 2),
            ("pol-vac", CategoriaDocumentoCorporativo.Politicas, "Política de Vacaciones y Permisos", "Lineamientos conforme a LFT y política interna URREA.", "🏖", new DateTime(2026, 3, 1), 12, 3),
            ("pol-home", CategoriaDocumentoCorporativo.Politicas, "Política de Home Office", "Modalidades de teletrabajo, elegibilidad y acuerdos.", "🏠", new DateTime(2025, 9, 10), 8, 4),
            ("proc-onb", CategoriaDocumentoCorporativo.Politicas, "Procedimiento de Incorporación", "Flujo de onboarding, inducción y alta de colaboradores.", "📋", new DateTime(2026, 2, 1), 16, 5),
            ("etica", CategoriaDocumentoCorporativo.Etica, "Código de Ética URREA", "Principios de integridad, confidencialidad y relación con terceros.", "⚖️", new DateTime(2025, 6, 1), 24, 6),
            ("etica-reg", CategoriaDocumentoCorporativo.Etica, "Reglamento de Comité de Ética", "Canales de denuncia, investigación y sanciones.", "🛡", new DateTime(2025, 6, 1), 10, 7),
            ("prev-imss", CategoriaDocumentoCorporativo.Prevision, "Previsión Social · IMSS e INFONAVIT", "Cobertura de seguridad social, guarderías y crédito INFONAVIT.", "🏥", new DateTime(2026, 1, 1), null, 8),
            ("prev-fondo", CategoriaDocumentoCorporativo.Prevision, "Fondo de Ahorro URREA", "Esquema de ahorro con aportación patronal y retiro anual.", "💰", new DateTime(2026, 1, 1), null, 9),
            ("prest-vales", CategoriaDocumentoCorporativo.Prestaciones, "Vales de Despensa y Restaurante", "Montos, periodicidad y proveedores autorizados.", "🛒", new DateTime(2026, 1, 1), null, 10),
            ("prest-seg", CategoriaDocumentoCorporativo.Prestaciones, "Seguro de Gastos Médicos Mayores", "Cobertura, deducibles y beneficiarios elegibles.", "❤️", new DateTime(2025, 12, 1), null, 11),
            ("prest-auto", CategoriaDocumentoCorporativo.Prestaciones, "Esquema de Automóvil", "Política de vehículo de la empresa para mandos medios.", "🚗", new DateTime(2025, 8, 15), null, 12),
            ("punt-reg", CategoriaDocumentoCorporativo.Puntualidad, "Política de Puntualidad y Asistencia", "Tolerancias, faltas, retardos y consecuencias conforme a LFT.", "⏱", new DateTime(2026, 1, 1), 6, 13),
            ("punt-inc", CategoriaDocumentoCorporativo.Puntualidad, "Incentivo por Puntualidad", "Bono trimestral por asistencia perfecta y puntualidad.", "⭐", new DateTime(2026, 1, 1), null, 14),
        };

        foreach (var d in docs)
        {
            context.DocumentosCorporativos.Add(new DocumentoCorporativo
            {
                Id = Guid.NewGuid(),
                Codigo = d.codigo,
                Categoria = d.cat,
                Titulo = d.titulo,
                Descripcion = d.desc,
                Icono = d.icono,
                FechaActualizacion = d.fecha,
                Paginas = d.paginas,
                Orden = d.orden,
                CreatedAt = now,
                IsActive = true,
            });
        }
    }

    private static void SeedFestivos(UrreaHubDbContext context, DateTime now)
    {
        var festivos = new (DateTime fecha, string nombre, TipoDiaFestivo tipo)[]
        {
            (new DateTime(2026, 1, 1), "Año Nuevo", TipoDiaFestivo.Oficial),
            (new DateTime(2026, 2, 2), "Día de la Constitución", TipoDiaFestivo.Oficial),
            (new DateTime(2026, 3, 16), "Natalicio de Benito Juárez", TipoDiaFestivo.Oficial),
            (new DateTime(2026, 5, 1), "Día del Trabajo", TipoDiaFestivo.Oficial),
            (new DateTime(2026, 9, 16), "Independencia de México", TipoDiaFestivo.Oficial),
            (new DateTime(2026, 11, 16), "Revolución Mexicana", TipoDiaFestivo.Oficial),
            (new DateTime(2026, 12, 25), "Navidad", TipoDiaFestivo.Oficial),
            (new DateTime(2026, 4, 30), "Día del Niño URREA (medio día)", TipoDiaFestivo.Empresa),
            (new DateTime(2026, 12, 24), "Nochebuena (salida temprana)", TipoDiaFestivo.Empresa),
        };

        foreach (var f in festivos)
        {
            context.DiasFestivosCorporativos.Add(new DiaFestivoCorporativo
            {
                Id = Guid.NewGuid(),
                Fecha = f.fecha,
                Nombre = f.nombre,
                Tipo = f.tipo,
                Anio = 2026,
                CreatedAt = now,
                IsActive = true,
            });
        }
    }

    private static void SeedConvenios(UrreaHubDbContext context, DateTime now)
    {
        var convenios = new (string codigo, string proveedor, string cat, string descuento, string desc, string icono, string vigencia, string? promo, int orden)[]
        {
            ("c1", "Costco", "Retail", "5% adicional", "Membresía corporativa con descuento en compras personales.", "🏬", "Dic 2026", "URREA-COSTCO", 1),
            ("c2", "Sams Club", "Retail", "Membresía gratis", "Acceso preferente para colaboradores URREA.", "🛍", "Dic 2026", null, 2),
            ("c3", "Office Depot", "Papelería", "15% descuento", "Material de oficina y tecnología para uso personal.", "📎", "Jun 2026", "URR15", 3),
            ("c4", "Smart Fit", "Bienestar", "30% membresía", "Plan anual con tarifa preferencial corporativa.", "💪", "Dic 2026", "URREAFIT", 4),
            ("c5", "OXXO Gas", "Automotriz", "2% cashback", "Acumulación en tarjeta de nómina al cargar combustible.", "⛽", "Dic 2026", null, 5),
            ("c6", "Cinépolis", "Entretenimiento", "2x1 boletos", "Válido martes y miércoles en complejos participantes.", "🎬", "Sep 2026", "URREACINE", 6),
        };

        foreach (var c in convenios)
        {
            context.ConveniosProveedores.Add(new ConvenioProveedor
            {
                Id = Guid.NewGuid(),
                Codigo = c.codigo,
                Proveedor = c.proveedor,
                Categoria = c.cat,
                Descuento = c.descuento,
                Descripcion = c.desc,
                Icono = c.icono,
                Vigencia = c.vigencia,
                CodigoPromocional = c.promo,
                Orden = c.orden,
                CreatedAt = now,
                IsActive = true,
            });
        }
    }

    private static void SeedProductos(UrreaHubDbContext context, DateTime now)
    {
        var productos = new (string codigo, string nombre, string cat, int puntos, int stock, string icono, string gradient, bool dest, int orden)[]
        {
            ("p1", "Termo URREA 750ml", "Hogar", 120, 45, "🥤", "from-slate-600 to-urrea-primary", true, 1),
            ("p2", "Paraguas corporativo", "Accesorios", 80, 30, "☂️", "from-urrea-secondary to-teal-600", false, 2),
            ("p3", "Playera polo URREA", "Ropa", 200, 60, "👕", "from-urrea-primary to-indigo-700", true, 3),
            ("p4", "Sudadera con logo", "Ropa", 350, 25, "🧥", "from-slate-700 to-urrea-secondary", false, 4),
            ("p5", "Mochila ejecutiva", "Accesorios", 420, 18, "🎒", "from-urrea-primary to-slate-800", false, 5),
            ("p6", "Gorra URREA", "Ropa", 90, 50, "🧢", "from-teal-600 to-urrea-secondary", false, 6),
            ("p7", "Taza cerámica", "Hogar", 60, 80, "☕", "from-urrea-accent-sand to-urrea-secondary", false, 7),
            ("p8", "Botella deportiva", "Hogar", 100, 40, "🍶", "from-sky-600 to-urrea-primary", false, 8),
            ("p9", "Kit de herramientas mini", "Especial", 280, 15, "🔧", "from-urrea-primary to-urrea-secondary", true, 9),
            ("p10", "Power bank URREA", "Tecnología", 180, 22, "🔋", "from-indigo-700 to-urrea-secondary", false, 10),
        };

        foreach (var p in productos)
        {
            context.ProductosTiendaInterna.Add(new ProductoTiendaInterna
            {
                Id = Guid.NewGuid(),
                Codigo = p.codigo,
                Nombre = p.nombre,
                Categoria = p.cat,
                PuntosRequeridos = p.puntos,
                Stock = p.stock,
                Icono = p.icono,
                Gradiente = p.gradient,
                Destacado = p.dest,
                Orden = p.orden,
                CreatedAt = now,
                IsActive = true,
            });
        }
    }

    private static void SeedModulos(UrreaHubDbContext context, DateTime now)
    {
        var modulos = new (string codigo, string titulo, string? subtitulo, string desc, string icono, int orden)[]
        {
            ("encuestas", "Mis encuestas", "Clima y pulso organizacional", "Responde encuestas de clima, pulso organizacional y evaluaciones de servicio interno.", "📊", 1),
            ("capacitaciones", "Mis capacitaciones", "Desarrollo y certificaciones", "Consulta tus cursos inscritos, constancias y rutas de aprendizaje asignadas.", "🎓", 2),
            ("evaluaciones", "Mis evaluaciones", "Desempeño y objetivos", "Revisa tus evaluaciones de desempeño, metas del periodo y retroalimentación recibida.", "📈", 3),
            ("reconocimientos", "Reconocimientos", "Muro de logros", "Consulta reconocimientos recibidos y celebra los logros de tu equipo.", "🏆", 4),
            ("procesos", "Procesos Internos", "Manuales y flujos", "Accede a manuales, flujos de aprobación y documentación operativa de URREA.", "📁", 5),
            ("adn_objetivos", "ADN Objetivos", "Metas estratégicas", "Alinea tus objetivos individuales con la estrategia corporativa URREA.", "🎯", 6),
        };

        foreach (var m in modulos)
        {
            context.ContenidosModuloPortal.Add(new ContenidoModuloPortal
            {
                Id = Guid.NewGuid(),
                CodigoModulo = m.codigo,
                Titulo = m.titulo,
                Subtitulo = m.subtitulo,
                Descripcion = m.desc,
                Icono = m.icono,
                Orden = m.orden,
                Publicado = true,
                CreatedAt = now,
                IsActive = true,
            });
        }
    }

    private static async Task SeedIntegracionesAsync(UrreaHubDbContext context, DateTime now)
    {
        if (await context.Integraciones.AnyAsync())
            return;

        context.Integraciones.AddRange(
            new Integracion
            {
                Id = Guid.NewGuid(),
                Nombre = "Sync nómina",
                SistemaExterno = "NominaCSV / ERP producción",
                Endpoint = "Adapter interno (dev: CSV)",
                Estado = EstadoIntegracion.Exitosa,
                UltimaEjecucion = now.AddHours(-2),
                CreatedAt = now,
                IsActive = true,
            },
            new Integracion
            {
                Id = Guid.NewGuid(),
                Nombre = "Notificaciones",
                SistemaExterno = "Microsoft Graph",
                Endpoint = "https://graph.microsoft.com/v1.0",
                Estado = EstadoIntegracion.Pendiente,
                CreatedAt = now,
                IsActive = true,
            },
            new Integracion
            {
                Id = Guid.NewGuid(),
                Nombre = "Directorio activo",
                SistemaExterno = "Azure AD",
                Endpoint = "Producción (pendiente)",
                Estado = EstadoIntegracion.Pendiente,
                CreatedAt = now,
                IsActive = true,
            });
    }

    private static async Task SeedSaldosPuntosAsync(UrreaHubDbContext context, DateTime now)
    {
        var colaboradores = await context.Colaboradores.Where(c => c.IsActive).ToListAsync();
        foreach (var c in colaboradores)
        {
            context.SaldosPuntosColaboradores.Add(new SaldoPuntosColaborador
            {
                Id = Guid.NewGuid(),
                ColaboradorId = c.Id,
                Puntos = PuntosIniciales,
                CreatedAt = now,
                IsActive = true,
            });
        }
    }
}
