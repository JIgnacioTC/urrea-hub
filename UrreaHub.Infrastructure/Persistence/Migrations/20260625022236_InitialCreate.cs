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
                name: "Beneficios");

            migrationBuilder.EnsureSchema(
                name: "Vacaciones");

            migrationBuilder.EnsureSchema(
                name: "Requisiciones");

            migrationBuilder.EnsureSchema(
                name: "CoreRH");

            migrationBuilder.EnsureSchema(
                name: "Auditoria");

            migrationBuilder.EnsureSchema(
                name: "Reclutamiento");

            migrationBuilder.EnsureSchema(
                name: "Onboarding");

            migrationBuilder.EnsureSchema(
                name: "Desempeno");

            migrationBuilder.EnsureSchema(
                name: "Capacitacion");

            migrationBuilder.EnsureSchema(
                name: "Documentos");

            migrationBuilder.EnsureSchema(
                name: "Organizacion");

            migrationBuilder.CreateTable(
                name: "Areas",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MontoMaximo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Moneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Modulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntidadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Detalle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpOrigen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntidadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstadoAnterior = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoNuevo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkedIn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Peso = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cursos",
                schema: "Capacitacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuracionHoras = table.Column<int>(type: "int", nullable: false),
                    Modalidad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Integraciones",
                schema: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SistemaExterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    UltimaEjecucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Integraciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificacionesEnviadas",
                schema: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Canal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Destinatario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Exitosa = table.Column<bool>(type: "bit", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaVigencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EsVigente = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organigramas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Politicas",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiasAnuales = table.Column<int>(type: "int", nullable: false),
                    AntiguedadMinimaMeses = table.Column<int>(type: "int", nullable: false),
                    Acumulable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Politicas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Puestos",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NivelJerarquico = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puestos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelacionesLaborales",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelacionesLaborales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sedes",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ciudad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pais = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sedes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposAusencia",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescuentaSaldo = table.Column<bool>(type: "bit", nullable: false),
                    RequiereAprobacion = table.Column<bool>(type: "bit", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAusencia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposDocumento",
                schema: "Documentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiereVigencia = table.Column<bool>(type: "bit", nullable: false),
                    RequiereFirma = table.Column<bool>(type: "bit", nullable: false),
                    NivelConfidencialidad = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Elegibilidades",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Criterio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BeneficioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resumen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCarga = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CandidatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "ErroresIntegracion",
                schema: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodigoError = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaError = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IntegracionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "CalendariosLaborales",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    SedeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Departamentos",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SedeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departamentos_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "CoreRH",
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Departamentos_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalSchema: "CoreRH",
                        principalTable: "Sedes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DiasInhabiles",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsOficial = table.Column<bool>(type: "bit", nullable: false),
                    CalendarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroEmpleado = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PuestoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SedeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CentroCostoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelacionLaboralId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JefeDirectoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Evaluaciones",
                schema: "Desempeno",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PuntuacionFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ComentarioGeneral = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CicloId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Inscripciones",
                schema: "Capacitacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCompletado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CursoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Objetivos",
                schema: "Desempeno",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Meta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Avance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Peso = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CicloId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Posiciones",
                schema: "Organizacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganigramaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PosicionPadreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Requisiciones",
                schema: "Requisiciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Folio = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VacantesSolicitadas = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SolicitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    DiasAsignados = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiasUsados = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PoliticaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Solicitudes",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontoSolicitado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Justificacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    BeneficioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiasSolicitados = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoAusenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Feedbacks",
                schema: "Desempeno",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvaluacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Calificacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recomendacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaResultado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EvaluacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VersionActual = table.Column<int>(type: "int", nullable: false),
                    TipoDocumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpedienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Folio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InscripcionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Puntuacion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Aprobado = table.Column<bool>(type: "bit", nullable: false),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InscripcionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InscripcionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Completado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCompletado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Completada = table.Column<bool>(type: "bit", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PosicionOrigenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PosicionDestinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaApertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PosicionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Aprobadores",
                schema: "Requisiciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: false),
                    FechaDecision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequisicionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AprobadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaAccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequisicionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImpactoNegocio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternativasConsideradas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequisicionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DescripcionPuesto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExperienciaRequerida = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EducacionRequerida = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompetenciasRequeridas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequisicionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MontoAutorizado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CentroCostoCodigo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequisicionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequisicionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Aprobaciones",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaDecision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AprobadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaDecision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AprobadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Firmas",
                schema: "Documentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Firmante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaFirma = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MetodoFirma = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroVersion = table.Column<int>(type: "int", nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComentarioCambio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaVersion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocumentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TareaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCompromiso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCumplimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TareaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TareaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Postulaciones",
                schema: "Reclutamiento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaPostulacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VacanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Canal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VacanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostulacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Evaluador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Puntuacion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostulacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalarioOfrecido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaOferta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aceptada = table.Column<bool>(type: "bit", nullable: false),
                    PostulacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "IX_Aprobaciones_AprobadorId",
                schema: "Vacaciones",
                table: "Aprobaciones",
                column: "AprobadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Aprobaciones_SolicitudId",
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
                name: "IX_CalendariosLaborales_SedeId",
                schema: "Vacaciones",
                table: "CalendariosLaborales",
                column: "SedeId");

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
                name: "IX_Constancias_InscripcionId",
                schema: "Capacitacion",
                table: "Constancias",
                column: "InscripcionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CVs_CandidatoId",
                schema: "Reclutamiento",
                table: "CVs",
                column: "CandidatoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_AreaId",
                schema: "CoreRH",
                table: "Departamentos",
                column: "AreaId");

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
                name: "IX_Firmas_DocumentoId",
                schema: "Documentos",
                table: "Firmas",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Historial_RequisicionId",
                schema: "Requisiciones",
                table: "Historial",
                column: "RequisicionId");

            migrationBuilder.CreateIndex(
                name: "IX_Historial_UsuarioId",
                schema: "Requisiciones",
                table: "Historial",
                column: "UsuarioId");

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
                name: "IX_Justificaciones_RequisicionId",
                schema: "Requisiciones",
                table: "Justificaciones",
                column: "RequisicionId",
                unique: true);

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
                name: "IX_Planes_ColaboradorId",
                schema: "Onboarding",
                table: "Planes",
                column: "ColaboradorId");

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
                name: "IX_Solicitudes_ColaboradorId",
                schema: "Vacaciones",
                table: "Solicitudes",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_TipoAusenciaId",
                schema: "Vacaciones",
                table: "Solicitudes",
                column: "TipoAusenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_PlanId",
                schema: "Onboarding",
                table: "Tareas",
                column: "PlanId");

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
                name: "Aprobaciones",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "Aprobaciones",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "Aprobadores",
                schema: "Requisiciones");

            migrationBuilder.DropTable(
                name: "BitacoraEventos",
                schema: "Auditoria");

            migrationBuilder.DropTable(
                name: "CambiosEstado",
                schema: "Auditoria");

            migrationBuilder.DropTable(
                name: "Checklists",
                schema: "Onboarding");

            migrationBuilder.DropTable(
                name: "Competencias",
                schema: "Desempeno");

            migrationBuilder.DropTable(
                name: "Constancias",
                schema: "Capacitacion");

            migrationBuilder.DropTable(
                name: "CVs",
                schema: "Reclutamiento");

            migrationBuilder.DropTable(
                name: "DiasInhabiles",
                schema: "Vacaciones");

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
                name: "Firmas",
                schema: "Documentos");

            migrationBuilder.DropTable(
                name: "Historial",
                schema: "Requisiciones");

            migrationBuilder.DropTable(
                name: "Justificaciones",
                schema: "Requisiciones");

            migrationBuilder.DropTable(
                name: "MovimientosOrganizacionales",
                schema: "Organizacion");

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
                name: "Presupuestos",
                schema: "Requisiciones");

            migrationBuilder.DropTable(
                name: "Publicaciones",
                schema: "Reclutamiento");

            migrationBuilder.DropTable(
                name: "Responsables",
                schema: "Onboarding");

            migrationBuilder.DropTable(
                name: "Resultados",
                schema: "Desempeno");

            migrationBuilder.DropTable(
                name: "Saldos",
                schema: "Vacaciones");

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
                name: "Solicitudes",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "Solicitudes",
                schema: "Vacaciones");

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
                name: "Postulaciones",
                schema: "Reclutamiento");

            migrationBuilder.DropTable(
                name: "Tareas",
                schema: "Onboarding");

            migrationBuilder.DropTable(
                name: "Evaluaciones",
                schema: "Desempeno");

            migrationBuilder.DropTable(
                name: "Politicas",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "Posiciones",
                schema: "Organizacion");

            migrationBuilder.DropTable(
                name: "Documentos",
                schema: "Documentos");

            migrationBuilder.DropTable(
                name: "Beneficios",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "TiposAusencia",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "Cursos",
                schema: "Capacitacion");

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
                name: "Areas",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "Sedes",
                schema: "CoreRH");
        }
    }
}
