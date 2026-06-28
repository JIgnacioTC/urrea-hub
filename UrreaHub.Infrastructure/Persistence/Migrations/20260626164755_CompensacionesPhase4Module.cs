using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompensacionesPhase4Module : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Compensaciones");

            migrationBuilder.CreateTable(
                name: "Conceptos",
                schema: "Compensaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImpactaNomina = table.Column<bool>(type: "bit", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conceptos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesAjuste",
                schema: "Compensaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoAjuste = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ValorAnterior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorNuevo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDecision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiereFinanzas = table.Column<bool>(type: "bit", nullable: false),
                    MontoReferencia = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Tabuladores",
                schema: "Compensaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Moneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VigenciaDesde = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VigenciaHasta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tabuladores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Aprobaciones",
                schema: "Compensaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaDecision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RolAprobador = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaAccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                        name: "FK_Historial_SolicitudesAjuste_SolicitudId",
                        column: x => x.SolicitudId,
                        principalSchema: "Compensaciones",
                        principalTable: "SolicitudesAjuste",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BandasSalariales",
                schema: "Compensaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nivel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Minimo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Medio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Maximo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TabuladorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Aprobaciones_AprobadorId",
                schema: "Compensaciones",
                table: "Aprobaciones",
                column: "AprobadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Aprobaciones_SolicitudId",
                schema: "Compensaciones",
                table: "Aprobaciones",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_BandasSalariales_TabuladorId",
                schema: "Compensaciones",
                table: "BandasSalariales",
                column: "TabuladorId");

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
                name: "IX_SolicitudesAjuste_ColaboradorId",
                schema: "Compensaciones",
                table: "SolicitudesAjuste",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAjuste_SolicitanteId",
                schema: "Compensaciones",
                table: "SolicitudesAjuste",
                column: "SolicitanteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aprobaciones_Colaboradores_AprobadorId",
                schema: "Beneficios",
                table: "Aprobaciones");

            migrationBuilder.DropTable(
                name: "Aprobaciones",
                schema: "Compensaciones");

            migrationBuilder.DropTable(
                name: "BandasSalariales",
                schema: "Compensaciones");

            migrationBuilder.DropTable(
                name: "Conceptos",
                schema: "Compensaciones");

            migrationBuilder.DropTable(
                name: "Historial",
                schema: "Compensaciones");

            migrationBuilder.DropTable(
                name: "Tabuladores",
                schema: "Compensaciones");

            migrationBuilder.DropTable(
                name: "SolicitudesAjuste",
                schema: "Compensaciones");
        }
    }
}
