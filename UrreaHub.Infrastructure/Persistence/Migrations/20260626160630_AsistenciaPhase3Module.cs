using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AsistenciaPhase3Module : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Asistencia");

            migrationBuilder.CreateTable(
                name: "Registros",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraEntrada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraSalida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Fuente = table.Column<int>(type: "int", nullable: false),
                    TipoRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    LatitudEntrada = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LongitudEntrada = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LatitudSalida = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LongitudSalida = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClienteComercial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UbicacionComercial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Reglas",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SedeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinutosToleranciaRetardo = table.Column<int>(type: "int", nullable: false),
                    MinutosParaFalta = table.Column<int>(type: "int", nullable: false),
                    GeneraIncidenciaNominaRetardo = table.Column<bool>(type: "bit", nullable: false),
                    RequiereValidacionLider = table.Column<bool>(type: "bit", nullable: false),
                    PermitirRegistroMovil = table.Column<bool>(type: "bit", nullable: false),
                    RequiereGeolocalizacion = table.Column<bool>(type: "bit", nullable: false),
                    RadioMetrosSede = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoraEntrada = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraSalida = table.Column<TimeSpan>(type: "time", nullable: false),
                    MinutosToleranciaEntrada = table.Column<int>(type: "int", nullable: false),
                    MinutosToleranciaSalida = table.Column<int>(type: "int", nullable: false),
                    MinutosComida = table.Column<int>(type: "int", nullable: false),
                    AplicaLunes = table.Column<bool>(type: "bit", nullable: false),
                    AplicaMartes = table.Column<bool>(type: "bit", nullable: false),
                    AplicaMiercoles = table.Column<bool>(type: "bit", nullable: false),
                    AplicaJueves = table.Column<bool>(type: "bit", nullable: false),
                    AplicaViernes = table.Column<bool>(type: "bit", nullable: false),
                    AplicaSabado = table.Column<bool>(type: "bit", nullable: false),
                    AplicaDomingo = table.Column<bool>(type: "bit", nullable: false),
                    SedeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Incidencias",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistroId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Severidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiereValidacion = table.Column<bool>(type: "bit", nullable: false),
                    GeneraPrenomina = table.Column<bool>(type: "bit", nullable: false),
                    FechaDeteccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "AsignacionesTurno",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TurnoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Origen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Correcciones",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AprobadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TipoCorreccion = table.Column<int>(type: "int", nullable: false),
                    HoraEntradaSolicitada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraSalidaSolicitada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvidenciaRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDecision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ComentarioDecision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Periodo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoConcepto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEnvioNomina = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NominaSyncAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorNomina = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "IX_Registros_ColaboradorId_Fecha",
                schema: "Asistencia",
                table: "Registros",
                columns: new[] { "ColaboradorId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_Reglas_SedeId",
                schema: "Asistencia",
                table: "Reglas",
                column: "SedeId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignacionesTurno",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "Correcciones",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "IncidenciasNomina",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "Reglas",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "Turnos",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "Incidencias",
                schema: "Asistencia");

            migrationBuilder.DropTable(
                name: "Registros",
                schema: "Asistencia");
        }
    }
}
