using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VacacionesPhase2PayrollAdjustments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AjustesSaldo",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SaldoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiasAnteriores = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiasNuevos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Delta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RealizadoPor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "IncidenciasNomina",
                schema: "Vacaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoIncidencia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Dias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnviadaAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                        name: "FK_IncidenciasNomina_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalSchema: "Vacaciones",
                        principalTable: "Solicitudes",
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
                name: "IX_IncidenciasNomina_ColaboradorId",
                schema: "Vacaciones",
                table: "IncidenciasNomina",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasNomina_SolicitudId",
                schema: "Vacaciones",
                table: "IncidenciasNomina",
                column: "SolicitudId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AjustesSaldo",
                schema: "Vacaciones");

            migrationBuilder.DropTable(
                name: "IncidenciasNomina",
                schema: "Vacaciones");
        }
    }
}
