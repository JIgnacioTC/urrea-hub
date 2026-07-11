using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Vacaciones");

            migrationBuilder.EnsureSchema(
                name: "Beneficios");

            migrationBuilder.EnsureSchema(
                name: "Compensaciones");

            migrationBuilder.EnsureSchema(
                name: "Requisiciones");

            migrationBuilder.EnsureSchema(
                name: "CoreRH");

            migrationBuilder.EnsureSchema(
                name: "Asistencia");

            migrationBuilder.EnsureSchema(
                name: "Auditoria");

            migrationBuilder.EnsureSchema(
                name: "Reclutamiento");

            migrationBuilder.EnsureSchema(
                name: "Onboarding");

            migrationBuilder.EnsureSchema(
                name: "Desempeno");

            migrationBuilder.EnsureSchema(
                name: "Seguridad");

            migrationBuilder.EnsureSchema(
                name: "Plataforma");

            migrationBuilder.EnsureSchema(
                name: "Capacitacion");

            migrationBuilder.EnsureSchema(
                name: "Portal");

            migrationBuilder.EnsureSchema(
                name: "Documentos");

            migrationBuilder.EnsureSchema(
                name: "Catalogos");

            migrationBuilder.EnsureSchema(
                name: "Organizacion");

            migrationBuilder.CreateTable(
                name: "Areas",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Beneficios",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    MontoMaximo = table.Column<decimal>(type: "numeric", nullable: true),
                    Moneda = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BitacoraEventos",
                schema: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Modulo = table.Column<string>(type: "text", nullable: false),
                    Accion = table.Column<string>(type: "text", nullable: false),
                    Entidad = table.Column<string>(type: "text", nullable: false),
                    EntidadId = table.Column<Guid>(type: "uuid", nullable: false),
                    Usuario = table.Column<string>(type: "text", nullable: true),
                    Detalle = table.Column<string>(type: "text", nullable: true),
                    FechaEvento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IpOrigen = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacoraEventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CambiosEstado",
                schema: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Entidad = table.Column<string>(type: "text", nullable: false),
                    EntidadId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstadoAnterior = table.Column<string>(type: "text", nullable: false),
                    EstadoNuevo = table.Column<string>(type: "text", nullable: false),
                    Usuario = table.Column<string>(type: "text", nullable: true),
                    FechaCambio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CambiosEstado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Candidatos",
                schema: "Reclutamiento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "text", nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    LinkedIn = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CentrosCosto",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentrosCosto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ciclos",
                schema: "Desempeno",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Anio = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ciclos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Competencias",
                schema: "Desempeno",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Peso = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Conceptos",
                schema: "Compensaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    ImpactaNomina = table.Column<bool>(type: "boolean", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conceptos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionesGlobales",
                schema: "Plataforma",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Clave = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Valor = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesGlobales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContenidosModulo",
                schema: "Portal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoModulo = table.Column<string>(type: "text", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Subtitulo = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Icono = table.Column<string>(type: "text", nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Publicado = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContenidosModulo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConveniosProveedores",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Proveedor = table.Column<string>(type: "text", nullable: false),
                    Categoria = table.Column<string>(type: "text", nullable: false),
                    Descuento = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Icono = table.Column<string>(type: "text", nullable: true),
                    Vigencia = table.Column<string>(type: "text", nullable: false),
                    CodigoPromocional = table.Column<string>(type: "text", nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConveniosProveedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cursos",
                schema: "Capacitacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    DuracionHoras = table.Column<int>(type: "integer", nullable: false),
                    Modalidad = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiasFestivos",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Anio = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiasFestivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosCorporativos",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Icono = table.Column<string>(type: "text", nullable: true),
                    Paginas = table.Column<int>(type: "integer", nullable: true),
                    UrlDocumento = table.Column<string>(type: "text", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosCorporativos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estados",
                schema: "Catalogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Pais = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "MEX"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCiviles",
                schema: "Catalogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCiviles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Integraciones",
                schema: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    SistemaExterno = table.Column<string>(type: "text", nullable: false),
                    Endpoint = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    UltimaEjecucion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Integraciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jerarquias",
                schema: "Catalogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NivelOrden = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jerarquias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Metadatos",
                schema: "Plataforma",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Origen = table.Column<int>(type: "integer", nullable: false),
                    Etiqueta = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    VersionTag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ContenidoJson = table.Column<string>(type: "text", nullable: false),
                    MigracionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreadoPorColaboradorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metadatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificacionesEnviadas",
                schema: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Canal = table.Column<string>(type: "text", nullable: false),
                    Destinatario = table.Column<string>(type: "text", nullable: false),
                    Asunto = table.Column<string>(type: "text", nullable: false),
                    Contenido = table.Column<string>(type: "text", nullable: true),
                    FechaEnvio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Exitosa = table.Column<bool>(type: "boolean", nullable: false),
                    Error = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionesEnviadas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organigramas",
                schema: "Organizacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    FechaVigencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EsVigente = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organigramas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                schema: "Seguridad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Modulo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Politicas",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    DiasAnuales = table.Column<int>(type: "integer", nullable: false),
                    AntiguedadMinimaMeses = table.Column<int>(type: "integer", nullable: false),
                    Acumulable = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Politicas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductosTienda",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Categoria = table.Column<string>(type: "text", nullable: false),
                    PuntosRequeridos = table.Column<int>(type: "integer", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    Icono = table.Column<string>(type: "text", nullable: true),
                    Gradiente = table.Column<string>(type: "text", nullable: true),
                    Destacado = table.Column<bool>(type: "boolean", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductosTienda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Puestos",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    NivelJerarquico = table.Column<int>(type: "integer", nullable: false),
                    GradoMercer = table.Column<int>(type: "integer", nullable: true),
                    Impacto = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Comunicacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Innovacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EducacionRequerida = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExperienciaAnios = table.Column<int>(type: "integer", nullable: true),
                    PresupuestoAnual = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    PersonalCargoDirecto = table.Column<int>(type: "integer", nullable: true),
                    PersonalCargoIndirecto = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puestos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RazonesTermino",
                schema: "Catalogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    RequiereComprobante = table.Column<bool>(type: "boolean", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazonesTermino", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelacionesLaborales",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelacionesLaborales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Seguridad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sedes",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Direccion = table.Column<string>(type: "text", nullable: true),
                    Ciudad = table.Column<string>(type: "text", nullable: true),
                    Pais = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sedes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tabuladores",
                schema: "Compensaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Moneda = table.Column<string>(type: "text", nullable: false),
                    VigenciaDesde = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    VigenciaHasta = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tabuladores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposDocumento",
                schema: "Documentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    RequiereVigencia = table.Column<bool>(type: "boolean", nullable: false),
                    RequiereFirma = table.Column<bool>(type: "boolean", nullable: false),
                    NivelConfidencialidad = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subareas",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    AreaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subareas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subareas_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "CoreRH",
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposAusencia",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    DescuentaSaldo = table.Column<bool>(type: "boolean", nullable: false),
                    RequiereAprobacion = table.Column<bool>(type: "boolean", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    EsParcial = table.Column<bool>(type: "boolean", nullable: false),
                    PermiteMultiDia = table.Column<bool>(type: "boolean", nullable: false),
                    DiasMaximosAnuales = table.Column<decimal>(type: "numeric", nullable: true),
                    DiasMaximosEvento = table.Column<decimal>(type: "numeric", nullable: true),
                    RequiereComprobante = table.Column<bool>(type: "boolean", nullable: false),
                    Remunerado = table.Column<bool>(type: "boolean", nullable: false),
                    BaseLegalLft = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Icono = table.Column<string>(type: "text", nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    IdLegacy = table.Column<int>(type: "integer", nullable: true),
                    WebhookUrl = table.Column<string>(type: "text", nullable: true),
                    AreaDestinoId = table.Column<Guid>(type: "uuid", nullable: true),
                    PermiteSolicitudEmpleado = table.Column<bool>(type: "boolean", nullable: false),
                    NotificarTeams = table.Column<bool>(type: "boolean", nullable: false),
                    NotificarCorreo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAusencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposAusencia_Areas_AreaDestinoId",
                        column: x => x.AreaDestinoId,
                        principalSchema: "CoreRH",
                        principalTable: "Areas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Elegibilidades",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Criterio = table.Column<string>(type: "text", nullable: false),
                    Valor = table.Column<string>(type: "text", nullable: false),
                    BeneficioId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elegibilidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Elegibilidades_Beneficios_BeneficioId",
                        column: x => x.BeneficioId,
                        principalSchema: "Beneficios",
                        principalTable: "Beneficios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CVs",
                schema: "Reclutamiento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NombreArchivo = table.Column<string>(type: "text", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: false),
                    Resumen = table.Column<string>(type: "text", nullable: true),
                    FechaCarga = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CandidatoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CVs_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalSchema: "Reclutamiento",
                        principalTable: "Candidatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Municipios",
                schema: "Catalogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EstadoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Municipios_Estados_EstadoId",
                        column: x => x.EstadoId,
                        principalSchema: "Catalogos",
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosPatronales",
                schema: "Catalogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    NumeroImss = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RazonSocial = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Rfc = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    EstadoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosPatronales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosPatronales_Estados_EstadoId",
                        column: x => x.EstadoId,
                        principalSchema: "Catalogos",
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ErroresIntegracion",
                schema: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoError = table.Column<string>(type: "text", nullable: false),
                    Mensaje = table.Column<string>(type: "text", nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: true),
                    FechaError = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IntegracionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErroresIntegracion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ErroresIntegracion_Integraciones_IntegracionId",
                        column: x => x.IntegracionId,
                        principalSchema: "Auditoria",
                        principalTable: "Integraciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolPermisos",
                schema: "Seguridad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RolId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermisoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolPermisos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolPermisos_Permisos_PermisoId",
                        column: x => x.PermisoId,
                        principalSchema: "Seguridad",
                        principalTable: "Permisos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolPermisos_Roles_RolId",
                        column: x => x.RolId,
                        principalSchema: "Seguridad",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CalendariosLaborales",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Anio = table.Column<int>(type: "integer", nullable: false),
                    SedeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendariosLaborales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendariosLaborales_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalSchema: "CoreRH",
                        principalTable: "Sedes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reglas",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SedeId = table.Column<Guid>(type: "uuid", nullable: true),
                    MinutosToleranciaRetardo = table.Column<int>(type: "integer", nullable: false),
                    MinutosParaFalta = table.Column<int>(type: "integer", nullable: false),
                    GeneraIncidenciaNominaRetardo = table.Column<bool>(type: "boolean", nullable: false),
                    RequiereValidacionLider = table.Column<bool>(type: "boolean", nullable: false),
                    PermitirRegistroMovil = table.Column<bool>(type: "boolean", nullable: false),
                    RequiereGeolocalizacion = table.Column<bool>(type: "boolean", nullable: false),
                    RadioMetrosSede = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reglas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reglas_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalSchema: "CoreRH",
                        principalTable: "Sedes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Turnos",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    HoraEntrada = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraSalida = table.Column<TimeSpan>(type: "interval", nullable: false),
                    MinutosToleranciaEntrada = table.Column<int>(type: "integer", nullable: false),
                    MinutosToleranciaSalida = table.Column<int>(type: "integer", nullable: false),
                    MinutosComida = table.Column<int>(type: "integer", nullable: false),
                    AplicaLunes = table.Column<bool>(type: "boolean", nullable: false),
                    AplicaMartes = table.Column<bool>(type: "boolean", nullable: false),
                    AplicaMiercoles = table.Column<bool>(type: "boolean", nullable: false),
                    AplicaJueves = table.Column<bool>(type: "boolean", nullable: false),
                    AplicaViernes = table.Column<bool>(type: "boolean", nullable: false),
                    AplicaSabado = table.Column<bool>(type: "boolean", nullable: false),
                    AplicaDomingo = table.Column<bool>(type: "boolean", nullable: false),
                    SedeId = table.Column<Guid>(type: "uuid", nullable: true),
                    AreaId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turnos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Turnos_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "CoreRH",
                        principalTable: "Areas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Turnos_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalSchema: "CoreRH",
                        principalTable: "Sedes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BandasSalariales",
                schema: "Compensaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nivel = table.Column<string>(type: "text", nullable: false),
                    Minimo = table.Column<decimal>(type: "numeric", nullable: false),
                    Medio = table.Column<decimal>(type: "numeric", nullable: false),
                    Maximo = table.Column<decimal>(type: "numeric", nullable: false),
                    TabuladorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BandasSalariales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BandasSalariales_Tabuladores_TabuladorId",
                        column: x => x.TabuladorId,
                        principalSchema: "Compensaciones",
                        principalTable: "Tabuladores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Departamentos",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    SubareaId = table.Column<Guid>(type: "uuid", nullable: false),
                    SedeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departamentos_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalSchema: "CoreRH",
                        principalTable: "Sedes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Departamentos_Subareas_SubareaId",
                        column: x => x.SubareaId,
                        principalSchema: "CoreRH",
                        principalTable: "Subareas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiasInhabiles",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    EsOficial = table.Column<bool>(type: "boolean", nullable: false),
                    CalendarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiasInhabiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiasInhabiles_CalendariosLaborales_CalendarioId",
                        column: x => x.CalendarioId,
                        principalSchema: "Vacaciones",
                        principalTable: "CalendariosLaborales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Colaboradores",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroEmpleado = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "text", nullable: true),
                    NombrePreferido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NominaSyncAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ExternalSource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ExternalEmployeeId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExternalSystemCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SyncStatus = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Pending"),
                    SyncHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    IsManualOverride = table.Column<bool>(type: "boolean", nullable: false),
                    PuestoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartamentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    SedeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CentroCostoId = table.Column<Guid>(type: "uuid", nullable: true),
                    RelacionLaboralId = table.Column<Guid>(type: "uuid", nullable: false),
                    JefeDirectoId = table.Column<Guid>(type: "uuid", nullable: true),
                    EsCuentaGenerica = table.Column<bool>(type: "boolean", nullable: false),
                    PuedenChecarRemotamente = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colaboradores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Colaboradores_CentrosCosto_CentroCostoId",
                        column: x => x.CentroCostoId,
                        principalSchema: "CoreRH",
                        principalTable: "CentrosCosto",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Colaboradores_Colaboradores_JefeDirectoId",
                        column: x => x.JefeDirectoId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Colaboradores_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalSchema: "CoreRH",
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Colaboradores_Puestos_PuestoId",
                        column: x => x.PuestoId,
                        principalSchema: "CoreRH",
                        principalTable: "Puestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Colaboradores_RelacionesLaborales_RelacionLaboralId",
                        column: x => x.RelacionLaboralId,
                        principalSchema: "CoreRH",
                        principalTable: "RelacionesLaborales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Colaboradores_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalSchema: "CoreRH",
                        principalTable: "Sedes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AsignacionesTurno",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TurnoId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Origen = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionesTurno", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignacionesTurno_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignacionesTurno_Turnos_TurnoId",
                        column: x => x.TurnoId,
                        principalSchema: "Asistencia",
                        principalTable: "Turnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CanjesTienda",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PuntosUsados = table.Column<int>(type: "integer", nullable: false),
                    FechaCanje = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanjesTienda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanjesTienda_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanjesTienda_ProductosTienda_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "Beneficios",
                        principalTable: "ProductosTienda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ColaboradoresDatosLaborales",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Jornada = table.Column<string>(type: "text", nullable: true),
                    Turno = table.Column<string>(type: "text", nullable: true),
                    GrupoNomina = table.Column<string>(type: "text", nullable: true),
                    Sindicalizado = table.Column<bool>(type: "boolean", nullable: false),
                    NivelSalarial = table.Column<string>(type: "text", nullable: true),
                    NivelVisibilidadCompensacion = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColaboradoresDatosLaborales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColaboradoresDatosLaborales_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ColaboradoresDatosSensibles",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rfc = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Curp = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Nss = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Genero = table.Column<string>(type: "text", nullable: true),
                    EstadoCivil = table.Column<string>(type: "text", nullable: true),
                    Domicilio = table.Column<string>(type: "text", nullable: true),
                    CodigoPostal = table.Column<string>(type: "text", nullable: true),
                    Ciudad = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Pais = table.Column<string>(type: "text", nullable: true),
                    Enmascarado = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColaboradoresDatosSensibles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColaboradoresDatosSensibles_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ColaboradorRoles",
                schema: "Seguridad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    RolId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColaboradorRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColaboradorRoles_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ColaboradorRoles_Roles_RolId",
                        column: x => x.RolId,
                        principalSchema: "Seguridad",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuentasAcceso",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UltimoAcceso = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DebeCambiarPassword = table.Column<bool>(type: "boolean", nullable: false),
                    EsRhAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    EsTiAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasAcceso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasAcceso_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evaluaciones",
                schema: "Desempeno",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PuntuacionFinal = table.Column<decimal>(type: "numeric", nullable: false),
                    ComentarioGeneral = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    CicloId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    EvaluadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Ciclos_CicloId",
                        column: x => x.CicloId,
                        principalSchema: "Desempeno",
                        principalTable: "Ciclos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Colaboradores_EvaluadorId",
                        column: x => x.EvaluadorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Expedientes",
                schema: "Documentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expedientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expedientes_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeedbacksEquipo",
                schema: "Desempeno",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AutorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbacksEquipo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbacksEquipo_Colaboradores_AutorId",
                        column: x => x.AutorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeedbacksEquipo_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inscripciones",
                schema: "Capacitacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCompletado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    CursoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscripciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inscripciones_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inscripciones_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalSchema: "Capacitacion",
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosColaborador",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FechaEfectiva = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ValorAnterior = table.Column<string>(type: "text", nullable: true),
                    ValorNuevo = table.Column<string>(type: "text", nullable: true),
                    Origen = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ReferenciaExterna = table.Column<string>(type: "text", nullable: true),
                    CreadoPor = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosColaborador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosColaborador_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Objetivos",
                schema: "Desempeno",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Meta = table.Column<decimal>(type: "numeric", nullable: false),
                    Avance = table.Column<decimal>(type: "numeric", nullable: false),
                    Peso = table.Column<decimal>(type: "numeric", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CicloId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objetivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Objetivos_Ciclos_CicloId",
                        column: x => x.CicloId,
                        principalSchema: "Desempeno",
                        principalTable: "Ciclos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Objetivos_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Planes",
                schema: "Onboarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Planes_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlanesAccion",
                schema: "Desempeno",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Avance = table.Column<decimal>(type: "numeric", nullable: false),
                    Prioridad = table.Column<string>(type: "text", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreadoPorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesAccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanesAccion_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanesAccion_Colaboradores_CreadoPorId",
                        column: x => x.CreadoPorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Posiciones",
                schema: "Organizacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    OrganigramaId = table.Column<Guid>(type: "uuid", nullable: false),
                    PosicionPadreId = table.Column<Guid>(type: "uuid", nullable: true),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posiciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posiciones_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posiciones_Organigramas_OrganigramaId",
                        column: x => x.OrganigramaId,
                        principalSchema: "Organizacion",
                        principalTable: "Organigramas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posiciones_Posiciones_PosicionPadreId",
                        column: x => x.PosicionPadreId,
                        principalSchema: "Organizacion",
                        principalTable: "Posiciones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Publicaciones",
                schema: "Portal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AutorNombre = table.Column<string>(type: "text", nullable: false),
                    AutorRol = table.Column<string>(type: "text", nullable: false),
                    AutorIniciales = table.Column<string>(type: "text", nullable: false),
                    Departamento = table.Column<string>(type: "text", nullable: false),
                    Contenido = table.Column<string>(type: "text", nullable: false),
                    GradienteImagen = table.Column<string>(type: "text", nullable: true),
                    Likes = table.Column<int>(type: "integer", nullable: false),
                    Comentarios = table.Column<int>(type: "integer", nullable: false),
                    Compartidos = table.Column<int>(type: "integer", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publicaciones_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Registros",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    HoraEntrada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraSalida = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Fuente = table.Column<int>(type: "integer", nullable: false),
                    TipoRegistro = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    LatitudEntrada = table.Column<decimal>(type: "numeric", nullable: true),
                    LongitudEntrada = table.Column<decimal>(type: "numeric", nullable: true),
                    LatitudSalida = table.Column<decimal>(type: "numeric", nullable: true),
                    LongitudSalida = table.Column<decimal>(type: "numeric", nullable: true),
                    ClienteComercial = table.Column<string>(type: "text", nullable: true),
                    UbicacionComercial = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registros_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Requisiciones",
                schema: "Requisiciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Folio = table.Column<string>(type: "text", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    VacantesSolicitadas = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SolicitanteId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartamentoId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requisiciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requisiciones_Colaboradores_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requisiciones_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalSchema: "CoreRH",
                        principalTable: "Departamentos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Saldos",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Anio = table.Column<int>(type: "integer", nullable: false),
                    DiasAsignados = table.Column<decimal>(type: "numeric", nullable: false),
                    DiasUsados = table.Column<decimal>(type: "numeric", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PoliticaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Saldos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Saldos_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Saldos_Politicas_PoliticaId",
                        column: x => x.PoliticaId,
                        principalSchema: "Vacaciones",
                        principalTable: "Politicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaldosPuntos",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Puntos = table.Column<int>(type: "integer", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaldosPuntos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaldosPuntos_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Solicitudes",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MontoSolicitado = table.Column<decimal>(type: "numeric", nullable: true),
                    Justificacion = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    BeneficioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitudes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solicitudes_Beneficios_BeneficioId",
                        column: x => x.BeneficioId,
                        principalSchema: "Beneficios",
                        principalTable: "Beneficios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Solicitudes_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Solicitudes",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Folio = table.Column<string>(type: "text", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DiasSolicitados = table.Column<decimal>(type: "numeric", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    EsDiaCompleto = table.Column<bool>(type: "boolean", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HoraFin = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoAusenciaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitudes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solicitudes_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Solicitudes_TiposAusencia_TipoAusenciaId",
                        column: x => x.TipoAusenciaId,
                        principalSchema: "Vacaciones",
                        principalTable: "TiposAusencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesAjuste",
                schema: "Compensaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SolicitanteId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoAjuste = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    ValorAnterior = table.Column<string>(type: "text", nullable: true),
                    ValorNuevo = table.Column<string>(type: "text", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaDecision = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RequiereFinanzas = table.Column<bool>(type: "boolean", nullable: false),
                    MontoReferencia = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesAjuste", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesAjuste_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesAjuste_Colaboradores_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesCambioHorario",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TurnoActualId = table.Column<Guid>(type: "uuid", nullable: false),
                    TurnoSolicitadoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    ComentarioAprobador = table.Column<string>(type: "text", nullable: true),
                    AprobadorId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaDecision = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesCambioHorario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesCambioHorario_Colaboradores_AprobadorId",
                        column: x => x.AprobadorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolicitudesCambioHorario_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesCambioHorario_Turnos_TurnoActualId",
                        column: x => x.TurnoActualId,
                        principalSchema: "Asistencia",
                        principalTable: "Turnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesCambioHorario_Turnos_TurnoSolicitadoId",
                        column: x => x.TurnoSolicitadoId,
                        principalSchema: "Asistencia",
                        principalTable: "Turnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                schema: "Desempeno",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    EvaluacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Evaluaciones_EvaluacionId",
                        column: x => x.EvaluacionId,
                        principalSchema: "Desempeno",
                        principalTable: "Evaluaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resultados",
                schema: "Desempeno",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Calificacion = table.Column<string>(type: "text", nullable: false),
                    Recomendacion = table.Column<string>(type: "text", nullable: true),
                    FechaResultado = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EvaluacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resultados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resultados_Evaluaciones_EvaluacionId",
                        column: x => x.EvaluacionId,
                        principalSchema: "Desempeno",
                        principalTable: "Evaluaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documentos",
                schema: "Documentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    NombreArchivo = table.Column<string>(type: "text", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: false),
                    VersionActual = table.Column<int>(type: "integer", nullable: false),
                    TipoDocumentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpedienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_Expedientes_ExpedienteId",
                        column: x => x.ExpedienteId,
                        principalSchema: "Documentos",
                        principalTable: "Expedientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalSchema: "Documentos",
                        principalTable: "TiposDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Constancias",
                schema: "Capacitacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Folio = table.Column<string>(type: "text", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InscripcionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Constancias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Constancias_Inscripciones_InscripcionId",
                        column: x => x.InscripcionId,
                        principalSchema: "Capacitacion",
                        principalTable: "Inscripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evaluaciones",
                schema: "Capacitacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Puntuacion = table.Column<decimal>(type: "numeric", nullable: false),
                    Aprobado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaEvaluacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Comentarios = table.Column<string>(type: "text", nullable: true),
                    InscripcionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Inscripciones_InscripcionId",
                        column: x => x.InscripcionId,
                        principalSchema: "Capacitacion",
                        principalTable: "Inscripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evidencias",
                schema: "Capacitacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NombreArchivo = table.Column<string>(type: "text", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InscripcionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evidencias_Inscripciones_InscripcionId",
                        column: x => x.InscripcionId,
                        principalSchema: "Capacitacion",
                        principalTable: "Inscripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Checklists",
                schema: "Onboarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Item = table.Column<string>(type: "text", nullable: false),
                    Completado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCompletado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checklists_Planes_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "Onboarding",
                        principalTable: "Planes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tareas",
                schema: "Onboarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Completada = table.Column<bool>(type: "boolean", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tareas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tareas_Planes_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "Onboarding",
                        principalTable: "Planes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosOrganizacionales",
                schema: "Organizacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PosicionOrigenId = table.Column<Guid>(type: "uuid", nullable: true),
                    PosicionDestinoId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosOrganizacionales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosOrganizacionales_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosOrganizacionales_Posiciones_PosicionDestinoId",
                        column: x => x.PosicionDestinoId,
                        principalSchema: "Organizacion",
                        principalTable: "Posiciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MovimientosOrganizacionales_Posiciones_PosicionOrigenId",
                        column: x => x.PosicionOrigenId,
                        principalSchema: "Organizacion",
                        principalTable: "Posiciones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Vacantes",
                schema: "Organizacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaApertura = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PosicionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacantes_Posiciones_PosicionId",
                        column: x => x.PosicionId,
                        principalSchema: "Organizacion",
                        principalTable: "Posiciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Incidencias",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistroId = table.Column<Guid>(type: "uuid", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Severidad = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    RequiereValidacion = table.Column<bool>(type: "boolean", nullable: false),
                    GeneraPrenomina = table.Column<bool>(type: "boolean", nullable: false),
                    FechaDeteccion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidencias_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidencias_Registros_RegistroId",
                        column: x => x.RegistroId,
                        principalSchema: "Asistencia",
                        principalTable: "Registros",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Aprobadores",
                schema: "Requisiciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Decision = table.Column<int>(type: "integer", nullable: false),
                    FechaDecision = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Comentario = table.Column<string>(type: "text", nullable: true),
                    RequisicionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AprobadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aprobadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aprobadores_Colaboradores_AprobadorId",
                        column: x => x.AprobadorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Aprobadores_Requisiciones_RequisicionId",
                        column: x => x.RequisicionId,
                        principalSchema: "Requisiciones",
                        principalTable: "Requisiciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Historial",
                schema: "Requisiciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Accion = table.Column<string>(type: "text", nullable: false),
                    Detalle = table.Column<string>(type: "text", nullable: true),
                    FechaAccion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RequisicionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Historial_Colaboradores_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Historial_Requisiciones_RequisicionId",
                        column: x => x.RequisicionId,
                        principalSchema: "Requisiciones",
                        principalTable: "Requisiciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Justificaciones",
                schema: "Requisiciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: false),
                    ImpactoNegocio = table.Column<string>(type: "text", nullable: true),
                    AlternativasConsideradas = table.Column<string>(type: "text", nullable: true),
                    RequisicionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Justificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Justificaciones_Requisiciones_RequisicionId",
                        column: x => x.RequisicionId,
                        principalSchema: "Requisiciones",
                        principalTable: "Requisiciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Perfiles",
                schema: "Requisiciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DescripcionPuesto = table.Column<string>(type: "text", nullable: false),
                    ExperienciaRequerida = table.Column<string>(type: "text", nullable: true),
                    EducacionRequerida = table.Column<string>(type: "text", nullable: true),
                    CompetenciasRequeridas = table.Column<string>(type: "text", nullable: true),
                    RequisicionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Perfiles_Requisiciones_RequisicionId",
                        column: x => x.RequisicionId,
                        principalSchema: "Requisiciones",
                        principalTable: "Requisiciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Presupuestos",
                schema: "Requisiciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MontoAutorizado = table.Column<decimal>(type: "numeric", nullable: false),
                    Moneda = table.Column<string>(type: "text", nullable: false),
                    CentroCostoCodigo = table.Column<string>(type: "text", nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    RequisicionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presupuestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presupuestos_Requisiciones_RequisicionId",
                        column: x => x.RequisicionId,
                        principalSchema: "Requisiciones",
                        principalTable: "Requisiciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vacantes",
                schema: "Reclutamiento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RequisicionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacantes_Requisiciones_RequisicionId",
                        column: x => x.RequisicionId,
                        principalSchema: "Requisiciones",
                        principalTable: "Requisiciones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AjustesSaldo",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SaldoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiasAnteriores = table.Column<decimal>(type: "numeric", nullable: false),
                    DiasNuevos = table.Column<decimal>(type: "numeric", nullable: false),
                    Delta = table.Column<decimal>(type: "numeric", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: false),
                    RealizadoPor = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AjustesSaldo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AjustesSaldo_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AjustesSaldo_Saldos_SaldoId",
                        column: x => x.SaldoId,
                        principalSchema: "Vacaciones",
                        principalTable: "Saldos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Aprobaciones",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Decision = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: true),
                    FechaDecision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "uuid", nullable: false),
                    AprobadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aprobaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aprobaciones_Colaboradores_AprobadorId",
                        column: x => x.AprobadorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Aprobaciones_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalSchema: "Beneficios",
                        principalTable: "Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Aprobaciones",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Decision = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: true),
                    FechaDecision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "uuid", nullable: false),
                    AprobadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aprobaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aprobaciones_Colaboradores_AprobadorId",
                        column: x => x.AprobadorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Aprobaciones_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalSchema: "Vacaciones",
                        principalTable: "Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncidenciasNomina",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroEmpleado = table.Column<string>(type: "text", nullable: false),
                    TipoIncidencia = table.Column<string>(type: "text", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Dias = table.Column<decimal>(type: "numeric", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    PayloadJson = table.Column<string>(type: "text", nullable: true),
                    EnviadaAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidenciasNomina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidenciasNomina_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidenciasNomina_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalSchema: "Vacaciones",
                        principalTable: "Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Aprobaciones",
                schema: "Compensaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Decision = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: true),
                    FechaDecision = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RolAprobador = table.Column<string>(type: "text", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "uuid", nullable: false),
                    AprobadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aprobaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aprobaciones_Colaboradores_AprobadorId",
                        column: x => x.AprobadorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Aprobaciones_SolicitudesAjuste_SolicitudId",
                        column: x => x.SolicitudId,
                        principalSchema: "Compensaciones",
                        principalTable: "SolicitudesAjuste",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Historial",
                schema: "Compensaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Accion = table.Column<string>(type: "text", nullable: false),
                    Detalle = table.Column<string>(type: "text", nullable: true),
                    FechaAccion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Historial_Colaboradores_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Historial_SolicitudesAjuste_SolicitudId",
                        column: x => x.SolicitudId,
                        principalSchema: "Compensaciones",
                        principalTable: "SolicitudesAjuste",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Firmas",
                schema: "Documentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Firmante = table.Column<string>(type: "text", nullable: false),
                    FechaFirma = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MetodoFirma = table.Column<string>(type: "text", nullable: true),
                    DocumentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Firmas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Firmas_Documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalSchema: "Documentos",
                        principalTable: "Documentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Versiones",
                schema: "Documentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroVersion = table.Column<int>(type: "integer", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: false),
                    ComentarioCambio = table.Column<string>(type: "text", nullable: true),
                    FechaVersion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DocumentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Versiones_Documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalSchema: "Documentos",
                        principalTable: "Documentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vigencias",
                schema: "Documentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DocumentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vigencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vigencias_Documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalSchema: "Documentos",
                        principalTable: "Documentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evidencias",
                schema: "Onboarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NombreArchivo = table.Column<string>(type: "text", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: true),
                    TareaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evidencias_Tareas_TareaId",
                        column: x => x.TareaId,
                        principalSchema: "Onboarding",
                        principalTable: "Tareas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FechasCompromiso",
                schema: "Onboarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaCompromiso = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCumplimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    TareaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FechasCompromiso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FechasCompromiso_Tareas_TareaId",
                        column: x => x.TareaId,
                        principalSchema: "Onboarding",
                        principalTable: "Tareas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Responsables",
                schema: "Onboarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Rol = table.Column<string>(type: "text", nullable: false),
                    TareaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responsables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Responsables_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Responsables_Tareas_TareaId",
                        column: x => x.TareaId,
                        principalSchema: "Onboarding",
                        principalTable: "Tareas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Correcciones",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IncidenciaId = table.Column<Guid>(type: "uuid", nullable: false),
                    SolicitanteId = table.Column<Guid>(type: "uuid", nullable: false),
                    AprobadorId = table.Column<Guid>(type: "uuid", nullable: true),
                    TipoCorreccion = table.Column<int>(type: "integer", nullable: false),
                    HoraEntradaSolicitada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HoraSalidaSolicitada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Motivo = table.Column<string>(type: "text", nullable: false),
                    EvidenciaRef = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaDecision = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ComentarioDecision = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Correcciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Correcciones_Colaboradores_AprobadorId",
                        column: x => x.AprobadorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Correcciones_Colaboradores_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Correcciones_Incidencias_IncidenciaId",
                        column: x => x.IncidenciaId,
                        principalSchema: "Asistencia",
                        principalTable: "Incidencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncidenciasNomina",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IncidenciaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Periodo = table.Column<string>(type: "text", nullable: false),
                    TipoConcepto = table.Column<string>(type: "text", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric", nullable: false),
                    Unidad = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaEnvioNomina = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NominaSyncAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ErrorNomina = table.Column<string>(type: "text", nullable: true),
                    ValidadoPor = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidenciasNomina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidenciasNomina_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidenciasNomina_Incidencias_IncidenciaId",
                        column: x => x.IncidenciaId,
                        principalSchema: "Asistencia",
                        principalTable: "Incidencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Postulaciones",
                schema: "Reclutamiento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaPostulacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    VacanteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidatoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postulaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Postulaciones_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalSchema: "Reclutamiento",
                        principalTable: "Candidatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Postulaciones_Vacantes_VacanteId",
                        column: x => x.VacanteId,
                        principalSchema: "Reclutamiento",
                        principalTable: "Vacantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Publicaciones",
                schema: "Reclutamiento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Canal = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    FechaPublicacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    VacanteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publicaciones_Vacantes_VacanteId",
                        column: x => x.VacanteId,
                        principalSchema: "Reclutamiento",
                        principalTable: "Vacantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Entrevistas",
                schema: "Reclutamiento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Ubicacion = table.Column<string>(type: "text", nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    PostulacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entrevistas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entrevistas_Postulaciones_PostulacionId",
                        column: x => x.PostulacionId,
                        principalSchema: "Reclutamiento",
                        principalTable: "Postulaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evaluaciones",
                schema: "Reclutamiento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Evaluador = table.Column<string>(type: "text", nullable: false),
                    Puntuacion = table.Column<decimal>(type: "numeric", nullable: false),
                    Comentarios = table.Column<string>(type: "text", nullable: true),
                    FechaEvaluacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PostulacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Postulaciones_PostulacionId",
                        column: x => x.PostulacionId,
                        principalSchema: "Reclutamiento",
                        principalTable: "Postulaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ofertas",
                schema: "Reclutamiento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalarioOfrecido = table.Column<decimal>(type: "numeric", nullable: false),
                    Moneda = table.Column<string>(type: "text", nullable: false),
                    FechaOferta = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Aceptada = table.Column<bool>(type: "boolean", nullable: false),
                    PostulacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ofertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ofertas_Postulaciones_PostulacionId",
                        column: x => x.PostulacionId,
                        principalSchema: "Reclutamiento",
                        principalTable: "Postulaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AjustesSaldo_ColaboradorId",
                schema: "Vacaciones",
                table: "AjustesSaldo",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_AjustesSaldo_SaldoId",
                schema: "Vacaciones",
                table: "AjustesSaldo",
                column: "SaldoId");

            migrationBuilder.CreateIndex(
                name: "IX_Aprobaciones_AprobadorId",
                schema: "Beneficios",
                table: "Aprobaciones",
                column: "AprobadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Aprobaciones_SolicitudId",
                schema: "Beneficios",
                table: "Aprobaciones",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_Aprobaciones_AprobadorId1",
                schema: "Compensaciones",
                table: "Aprobaciones",
                column: "AprobadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Aprobaciones_SolicitudId1",
                schema: "Compensaciones",
                table: "Aprobaciones",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_Aprobaciones_AprobadorId2",
                schema: "Vacaciones",
                table: "Aprobaciones",
                column: "AprobadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Aprobaciones_SolicitudId2",
                schema: "Vacaciones",
                table: "Aprobaciones",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_Aprobadores_AprobadorId",
                schema: "Requisiciones",
                table: "Aprobadores",
                column: "AprobadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Aprobadores_RequisicionId",
                schema: "Requisiciones",
                table: "Aprobadores",
                column: "RequisicionId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_Codigo",
                schema: "CoreRH",
                table: "Areas",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesTurno_ColaboradorId",
                schema: "Asistencia",
                table: "AsignacionesTurno",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesTurno_TurnoId",
                schema: "Asistencia",
                table: "AsignacionesTurno",
                column: "TurnoId");

            migrationBuilder.CreateIndex(
                name: "IX_BandasSalariales_TabuladorId",
                schema: "Compensaciones",
                table: "BandasSalariales",
                column: "TabuladorId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendariosLaborales_SedeId",
                schema: "Vacaciones",
                table: "CalendariosLaborales",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_CanjesTienda_ColaboradorId",
                schema: "Beneficios",
                table: "CanjesTienda",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_CanjesTienda_ProductoId",
                schema: "Beneficios",
                table: "CanjesTienda",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_CentrosCosto_Codigo",
                schema: "CoreRH",
                table: "CentrosCosto",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_PlanId",
                schema: "Onboarding",
                table: "Checklists",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_CentroCostoId",
                schema: "CoreRH",
                table: "Colaboradores",
                column: "CentroCostoId");

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_DepartamentoId",
                schema: "CoreRH",
                table: "Colaboradores",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_Email",
                schema: "CoreRH",
                table: "Colaboradores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_ExternalSource_ExternalEmployeeId",
                schema: "CoreRH",
                table: "Colaboradores",
                columns: new[] { "ExternalSource", "ExternalEmployeeId" },
                unique: true,
                filter: "\"ExternalSource\" IS NOT NULL AND \"ExternalEmployeeId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_JefeDirectoId",
                schema: "CoreRH",
                table: "Colaboradores",
                column: "JefeDirectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_NumeroEmpleado",
                schema: "CoreRH",
                table: "Colaboradores",
                column: "NumeroEmpleado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_PuestoId",
                schema: "CoreRH",
                table: "Colaboradores",
                column: "PuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_RelacionLaboralId",
                schema: "CoreRH",
                table: "Colaboradores",
                column: "RelacionLaboralId");

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_SedeId",
                schema: "CoreRH",
                table: "Colaboradores",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_ColaboradoresDatosLaborales_ColaboradorId",
                schema: "CoreRH",
                table: "ColaboradoresDatosLaborales",
                column: "ColaboradorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColaboradoresDatosSensibles_ColaboradorId",
                schema: "CoreRH",
                table: "ColaboradoresDatosSensibles",
                column: "ColaboradorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColaboradoresDatosSensibles_Rfc",
                schema: "CoreRH",
                table: "ColaboradoresDatosSensibles",
                column: "Rfc",
                unique: true,
                filter: "\"Rfc\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ColaboradorRoles_ColaboradorId_RolId",
                schema: "Seguridad",
                table: "ColaboradorRoles",
                columns: new[] { "ColaboradorId", "RolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ColaboradorRoles_RolId",
                schema: "Seguridad",
                table: "ColaboradorRoles",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesGlobales_Clave",
                schema: "Plataforma",
                table: "ConfiguracionesGlobales",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Constancias_InscripcionId",
                schema: "Capacitacion",
                table: "Constancias",
                column: "InscripcionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContenidosModulo_CodigoModulo",
                schema: "Portal",
                table: "ContenidosModulo",
                column: "CodigoModulo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConveniosProveedores_Codigo",
                schema: "Beneficios",
                table: "ConveniosProveedores",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Correcciones_AprobadorId",
                schema: "Asistencia",
                table: "Correcciones",
                column: "AprobadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Correcciones_IncidenciaId",
                schema: "Asistencia",
                table: "Correcciones",
                column: "IncidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Correcciones_SolicitanteId",
                schema: "Asistencia",
                table: "Correcciones",
                column: "SolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasAcceso_ColaboradorId",
                schema: "CoreRH",
                table: "CuentasAcceso",
                column: "ColaboradorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CVs_CandidatoId",
                schema: "Reclutamiento",
                table: "CVs",
                column: "CandidatoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_Codigo",
                schema: "CoreRH",
                table: "Departamentos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_SedeId",
                schema: "CoreRH",
                table: "Departamentos",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_SubareaId",
                schema: "CoreRH",
                table: "Departamentos",
                column: "SubareaId");

            migrationBuilder.CreateIndex(
                name: "IX_DiasInhabiles_CalendarioId",
                schema: "Vacaciones",
                table: "DiasInhabiles",
                column: "CalendarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_ExpedienteId",
                schema: "Documentos",
                table: "Documentos",
                column: "ExpedienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_TipoDocumentoId",
                schema: "Documentos",
                table: "Documentos",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosCorporativos_Codigo",
                schema: "Beneficios",
                table: "DocumentosCorporativos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Elegibilidades_BeneficioId",
                schema: "Beneficios",
                table: "Elegibilidades",
                column: "BeneficioId");

            migrationBuilder.CreateIndex(
                name: "IX_Entrevistas_PostulacionId",
                schema: "Reclutamiento",
                table: "Entrevistas",
                column: "PostulacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ErroresIntegracion_IntegracionId",
                schema: "Auditoria",
                table: "ErroresIntegracion",
                column: "IntegracionId");

            migrationBuilder.CreateIndex(
                name: "IX_Estados_Codigo",
                schema: "Catalogos",
                table: "Estados",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosCiviles_Codigo",
                schema: "Catalogos",
                table: "EstadosCiviles",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_InscripcionId",
                schema: "Capacitacion",
                table: "Evaluaciones",
                column: "InscripcionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_CicloId",
                schema: "Desempeno",
                table: "Evaluaciones",
                column: "CicloId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_ColaboradorId",
                schema: "Desempeno",
                table: "Evaluaciones",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_EvaluadorId",
                schema: "Desempeno",
                table: "Evaluaciones",
                column: "EvaluadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_PostulacionId",
                schema: "Reclutamiento",
                table: "Evaluaciones",
                column: "PostulacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidencias_InscripcionId",
                schema: "Capacitacion",
                table: "Evidencias",
                column: "InscripcionId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidencias_TareaId",
                schema: "Onboarding",
                table: "Evidencias",
                column: "TareaId");

            migrationBuilder.CreateIndex(
                name: "IX_Expedientes_ColaboradorId",
                schema: "Documentos",
                table: "Expedientes",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_FechasCompromiso_TareaId",
                schema: "Onboarding",
                table: "FechasCompromiso",
                column: "TareaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_EvaluacionId",
                schema: "Desempeno",
                table: "Feedbacks",
                column: "EvaluacionId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbacksEquipo_AutorId",
                schema: "Desempeno",
                table: "FeedbacksEquipo",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbacksEquipo_ColaboradorId",
                schema: "Desempeno",
                table: "FeedbacksEquipo",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Firmas_DocumentoId",
                schema: "Documentos",
                table: "Firmas",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Historial_SolicitudId",
                schema: "Compensaciones",
                table: "Historial",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_Historial_UsuarioId",
                schema: "Compensaciones",
                table: "Historial",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Historial_RequisicionId",
                schema: "Requisiciones",
                table: "Historial",
                column: "RequisicionId");

            migrationBuilder.CreateIndex(
                name: "IX_Historial_UsuarioId1",
                schema: "Requisiciones",
                table: "Historial",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_ColaboradorId",
                schema: "Asistencia",
                table: "Incidencias",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_RegistroId",
                schema: "Asistencia",
                table: "Incidencias",
                column: "RegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasNomina_ColaboradorId",
                schema: "Asistencia",
                table: "IncidenciasNomina",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasNomina_IncidenciaId",
                schema: "Asistencia",
                table: "IncidenciasNomina",
                column: "IncidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasNomina_ColaboradorId1",
                schema: "Vacaciones",
                table: "IncidenciasNomina",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasNomina_SolicitudId",
                schema: "Vacaciones",
                table: "IncidenciasNomina",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_ColaboradorId",
                schema: "Capacitacion",
                table: "Inscripciones",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_CursoId",
                schema: "Capacitacion",
                table: "Inscripciones",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Jerarquias_Codigo",
                schema: "Catalogos",
                table: "Jerarquias",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Justificaciones_RequisicionId",
                schema: "Requisiciones",
                table: "Justificaciones",
                column: "RequisicionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Metadatos_CreatedAt",
                schema: "Plataforma",
                table: "Metadatos",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Metadatos_Tipo_Origen",
                schema: "Plataforma",
                table: "Metadatos",
                columns: new[] { "Tipo", "Origen" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosColaborador_ColaboradorId",
                schema: "CoreRH",
                table: "MovimientosColaborador",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosOrganizacionales_ColaboradorId",
                schema: "Organizacion",
                table: "MovimientosOrganizacionales",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosOrganizacionales_PosicionDestinoId",
                schema: "Organizacion",
                table: "MovimientosOrganizacionales",
                column: "PosicionDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosOrganizacionales_PosicionOrigenId",
                schema: "Organizacion",
                table: "MovimientosOrganizacionales",
                column: "PosicionOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_EstadoId_Codigo",
                schema: "Catalogos",
                table: "Municipios",
                columns: new[] { "EstadoId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Objetivos_CicloId",
                schema: "Desempeno",
                table: "Objetivos",
                column: "CicloId");

            migrationBuilder.CreateIndex(
                name: "IX_Objetivos_ColaboradorId",
                schema: "Desempeno",
                table: "Objetivos",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_PostulacionId",
                schema: "Reclutamiento",
                table: "Ofertas",
                column: "PostulacionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_RequisicionId",
                schema: "Requisiciones",
                table: "Perfiles",
                column: "RequisicionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_Codigo",
                schema: "Seguridad",
                table: "Permisos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Planes_ColaboradorId",
                schema: "Onboarding",
                table: "Planes",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesAccion_ColaboradorId",
                schema: "Desempeno",
                table: "PlanesAccion",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesAccion_CreadoPorId",
                schema: "Desempeno",
                table: "PlanesAccion",
                column: "CreadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posiciones_ColaboradorId",
                schema: "Organizacion",
                table: "Posiciones",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posiciones_OrganigramaId",
                schema: "Organizacion",
                table: "Posiciones",
                column: "OrganigramaId");

            migrationBuilder.CreateIndex(
                name: "IX_Posiciones_PosicionPadreId",
                schema: "Organizacion",
                table: "Posiciones",
                column: "PosicionPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_Postulaciones_CandidatoId",
                schema: "Reclutamiento",
                table: "Postulaciones",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Postulaciones_VacanteId",
                schema: "Reclutamiento",
                table: "Postulaciones",
                column: "VacanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuestos_RequisicionId",
                schema: "Requisiciones",
                table: "Presupuestos",
                column: "RequisicionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductosTienda_Codigo",
                schema: "Beneficios",
                table: "ProductosTienda",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_ColaboradorId",
                schema: "Portal",
                table: "Publicaciones",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_VacanteId",
                schema: "Reclutamiento",
                table: "Publicaciones",
                column: "VacanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_Codigo",
                schema: "CoreRH",
                table: "Puestos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RazonesTermino_Codigo",
                schema: "Catalogos",
                table: "RazonesTermino",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registros_ColaboradorId_Fecha",
                schema: "Asistencia",
                table: "Registros",
                columns: new[] { "ColaboradorId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosPatronales_Codigo",
                schema: "Catalogos",
                table: "RegistrosPatronales",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosPatronales_EstadoId",
                schema: "Catalogos",
                table: "RegistrosPatronales",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosPatronales_NumeroImss",
                schema: "Catalogos",
                table: "RegistrosPatronales",
                column: "NumeroImss",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reglas_SedeId",
                schema: "Asistencia",
                table: "Reglas",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_RelacionesLaborales_Codigo",
                schema: "CoreRH",
                table: "RelacionesLaborales",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requisiciones_DepartamentoId",
                schema: "Requisiciones",
                table: "Requisiciones",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Requisiciones_Folio",
                schema: "Requisiciones",
                table: "Requisiciones",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requisiciones_SolicitanteId",
                schema: "Requisiciones",
                table: "Requisiciones",
                column: "SolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Responsables_ColaboradorId",
                schema: "Onboarding",
                table: "Responsables",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Responsables_TareaId",
                schema: "Onboarding",
                table: "Responsables",
                column: "TareaId");

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_EvaluacionId",
                schema: "Desempeno",
                table: "Resultados",
                column: "EvaluacionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Codigo",
                schema: "Seguridad",
                table: "Roles",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolPermisos_PermisoId",
                schema: "Seguridad",
                table: "RolPermisos",
                column: "PermisoId");

            migrationBuilder.CreateIndex(
                name: "IX_RolPermisos_RolId_PermisoId",
                schema: "Seguridad",
                table: "RolPermisos",
                columns: new[] { "RolId", "PermisoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Saldos_ColaboradorId",
                schema: "Vacaciones",
                table: "Saldos",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Saldos_PoliticaId",
                schema: "Vacaciones",
                table: "Saldos",
                column: "PoliticaId");

            migrationBuilder.CreateIndex(
                name: "IX_SaldosPuntos_ColaboradorId",
                schema: "Beneficios",
                table: "SaldosPuntos",
                column: "ColaboradorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_Codigo",
                schema: "CoreRH",
                table: "Sedes",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_BeneficioId",
                schema: "Beneficios",
                table: "Solicitudes",
                column: "BeneficioId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_ColaboradorId",
                schema: "Beneficios",
                table: "Solicitudes",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_ColaboradorId1",
                schema: "Vacaciones",
                table: "Solicitudes",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_TipoAusenciaId",
                schema: "Vacaciones",
                table: "Solicitudes",
                column: "TipoAusenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAjuste_ColaboradorId",
                schema: "Compensaciones",
                table: "SolicitudesAjuste",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAjuste_SolicitanteId",
                schema: "Compensaciones",
                table: "SolicitudesAjuste",
                column: "SolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCambioHorario_AprobadorId",
                schema: "Asistencia",
                table: "SolicitudesCambioHorario",
                column: "AprobadorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCambioHorario_ColaboradorId",
                schema: "Asistencia",
                table: "SolicitudesCambioHorario",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCambioHorario_TurnoActualId",
                schema: "Asistencia",
                table: "SolicitudesCambioHorario",
                column: "TurnoActualId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCambioHorario_TurnoSolicitadoId",
                schema: "Asistencia",
                table: "SolicitudesCambioHorario",
                column: "TurnoSolicitadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Subareas_AreaId",
                schema: "CoreRH",
                table: "Subareas",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Subareas_Codigo",
                schema: "CoreRH",
                table: "Subareas",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_PlanId",
                schema: "Onboarding",
                table: "Tareas",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposAusencia_AreaDestinoId",
                schema: "Vacaciones",
                table: "TiposAusencia",
                column: "AreaDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_AreaId",
                schema: "Asistencia",
                table: "Turnos",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_Codigo",
                schema: "Asistencia",
                table: "Turnos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_SedeId",
                schema: "Asistencia",
                table: "Turnos",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacantes_PosicionId",
                schema: "Organizacion",
                table: "Vacantes",
                column: "PosicionId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacantes_RequisicionId",
                schema: "Reclutamiento",
                table: "Vacantes",
                column: "RequisicionId");

            migrationBuilder.CreateIndex(
                name: "IX_Versiones_DocumentoId",
                schema: "Documentos",
                table: "Versiones",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vigencias_DocumentoId",
                schema: "Documentos",
                table: "Vigencias",
                column: "DocumentoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AjustesSaldo",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "Aprobaciones",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "Aprobaciones",
                schema: "Compensaciones");

            migrationBuilder.DropTable(
                name: "Aprobaciones",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "Aprobadores",
                schema: "Requisiciones");

            migrationBuilder.DropTable(
                name: "AsignacionesTurno",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "BandasSalariales",
                schema: "Compensaciones");

            migrationBuilder.DropTable(
                name: "BitacoraEventos",
                schema: "Auditoria");

            migrationBuilder.DropTable(
                name: "CambiosEstado",
                schema: "Auditoria");

            migrationBuilder.DropTable(
                name: "CanjesTienda",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "Checklists",
                schema: "Onboarding");

            migrationBuilder.DropTable(
                name: "ColaboradoresDatosLaborales",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "ColaboradoresDatosSensibles",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "ColaboradorRoles",
                schema: "Seguridad");

            migrationBuilder.DropTable(
                name: "Competencias",
                schema: "Desempeno");

            migrationBuilder.DropTable(
                name: "Conceptos",
                schema: "Compensaciones");

            migrationBuilder.DropTable(
                name: "ConfiguracionesGlobales",
                schema: "Plataforma");

            migrationBuilder.DropTable(
                name: "Constancias",
                schema: "Capacitacion");

            migrationBuilder.DropTable(
                name: "ContenidosModulo",
                schema: "Portal");

            migrationBuilder.DropTable(
                name: "ConveniosProveedores",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "Correcciones",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "CuentasAcceso",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "CVs",
                schema: "Reclutamiento");

            migrationBuilder.DropTable(
                name: "DiasFestivos",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "DiasInhabiles",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "DocumentosCorporativos",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "Elegibilidades",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "Entrevistas",
                schema: "Reclutamiento");

            migrationBuilder.DropTable(
                name: "ErroresIntegracion",
                schema: "Auditoria");

            migrationBuilder.DropTable(
                name: "EstadosCiviles",
                schema: "Catalogos");

            migrationBuilder.DropTable(
                name: "Evaluaciones",
                schema: "Capacitacion");

            migrationBuilder.DropTable(
                name: "Evaluaciones",
                schema: "Reclutamiento");

            migrationBuilder.DropTable(
                name: "Evidencias",
                schema: "Capacitacion");

            migrationBuilder.DropTable(
                name: "Evidencias",
                schema: "Onboarding");

            migrationBuilder.DropTable(
                name: "FechasCompromiso",
                schema: "Onboarding");

            migrationBuilder.DropTable(
                name: "Feedbacks",
                schema: "Desempeno");

            migrationBuilder.DropTable(
                name: "FeedbacksEquipo",
                schema: "Desempeno");

            migrationBuilder.DropTable(
                name: "Firmas",
                schema: "Documentos");

            migrationBuilder.DropTable(
                name: "Historial",
                schema: "Compensaciones");

            migrationBuilder.DropTable(
                name: "Historial",
                schema: "Requisiciones");

            migrationBuilder.DropTable(
                name: "IncidenciasNomina",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "IncidenciasNomina",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "Jerarquias",
                schema: "Catalogos");

            migrationBuilder.DropTable(
                name: "Justificaciones",
                schema: "Requisiciones");

            migrationBuilder.DropTable(
                name: "Metadatos",
                schema: "Plataforma");

            migrationBuilder.DropTable(
                name: "MovimientosColaborador",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "MovimientosOrganizacionales",
                schema: "Organizacion");

            migrationBuilder.DropTable(
                name: "Municipios",
                schema: "Catalogos");

            migrationBuilder.DropTable(
                name: "NotificacionesEnviadas",
                schema: "Auditoria");

            migrationBuilder.DropTable(
                name: "Objetivos",
                schema: "Desempeno");

            migrationBuilder.DropTable(
                name: "Ofertas",
                schema: "Reclutamiento");

            migrationBuilder.DropTable(
                name: "Perfiles",
                schema: "Requisiciones");

            migrationBuilder.DropTable(
                name: "PlanesAccion",
                schema: "Desempeno");

            migrationBuilder.DropTable(
                name: "Presupuestos",
                schema: "Requisiciones");

            migrationBuilder.DropTable(
                name: "Publicaciones",
                schema: "Portal");

            migrationBuilder.DropTable(
                name: "Publicaciones",
                schema: "Reclutamiento");

            migrationBuilder.DropTable(
                name: "RazonesTermino",
                schema: "Catalogos");

            migrationBuilder.DropTable(
                name: "RegistrosPatronales",
                schema: "Catalogos");

            migrationBuilder.DropTable(
                name: "Reglas",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "Responsables",
                schema: "Onboarding");

            migrationBuilder.DropTable(
                name: "Resultados",
                schema: "Desempeno");

            migrationBuilder.DropTable(
                name: "RolPermisos",
                schema: "Seguridad");

            migrationBuilder.DropTable(
                name: "SaldosPuntos",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "SolicitudesCambioHorario",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "Vacantes",
                schema: "Organizacion");

            migrationBuilder.DropTable(
                name: "Versiones",
                schema: "Documentos");

            migrationBuilder.DropTable(
                name: "Vigencias",
                schema: "Documentos");

            migrationBuilder.DropTable(
                name: "Saldos",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "Solicitudes",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "Tabuladores",
                schema: "Compensaciones");

            migrationBuilder.DropTable(
                name: "ProductosTienda",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "CalendariosLaborales",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "Integraciones",
                schema: "Auditoria");

            migrationBuilder.DropTable(
                name: "Inscripciones",
                schema: "Capacitacion");

            migrationBuilder.DropTable(
                name: "SolicitudesAjuste",
                schema: "Compensaciones");

            migrationBuilder.DropTable(
                name: "Incidencias",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "Solicitudes",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "Postulaciones",
                schema: "Reclutamiento");

            migrationBuilder.DropTable(
                name: "Estados",
                schema: "Catalogos");

            migrationBuilder.DropTable(
                name: "Tareas",
                schema: "Onboarding");

            migrationBuilder.DropTable(
                name: "Evaluaciones",
                schema: "Desempeno");

            migrationBuilder.DropTable(
                name: "Permisos",
                schema: "Seguridad");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Seguridad");

            migrationBuilder.DropTable(
                name: "Turnos",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "Posiciones",
                schema: "Organizacion");

            migrationBuilder.DropTable(
                name: "Documentos",
                schema: "Documentos");

            migrationBuilder.DropTable(
                name: "Politicas",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "Beneficios",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "Cursos",
                schema: "Capacitacion");

            migrationBuilder.DropTable(
                name: "Registros",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "TiposAusencia",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "Candidatos",
                schema: "Reclutamiento");

            migrationBuilder.DropTable(
                name: "Vacantes",
                schema: "Reclutamiento");

            migrationBuilder.DropTable(
                name: "Planes",
                schema: "Onboarding");

            migrationBuilder.DropTable(
                name: "Ciclos",
                schema: "Desempeno");

            migrationBuilder.DropTable(
                name: "Organigramas",
                schema: "Organizacion");

            migrationBuilder.DropTable(
                name: "Expedientes",
                schema: "Documentos");

            migrationBuilder.DropTable(
                name: "TiposDocumento",
                schema: "Documentos");

            migrationBuilder.DropTable(
                name: "Requisiciones",
                schema: "Requisiciones");

            migrationBuilder.DropTable(
                name: "Colaboradores",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "CentrosCosto",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "Departamentos",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "Puestos",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "RelacionesLaborales",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "Sedes",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "Subareas",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "Areas",
                schema: "CoreRH");
        }
    }
}
