using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Asistencia_CambioHorario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesCambioHorario",
                schema: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TurnoActualId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TurnoSolicitadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComentarioAprobador = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AprobadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FechaDecision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesCambioHorario",
                schema: "Asistencia");
        }
    }
}
