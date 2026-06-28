using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHcmFoundationPhase0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalEmployeeId",
                schema: "CoreRH",
                table: "Colaboradores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalSource",
                schema: "CoreRH",
                table: "Colaboradores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalSystemCode",
                schema: "CoreRH",
                table: "Colaboradores",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsManualOverride",
                schema: "CoreRH",
                table: "Colaboradores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NombrePreferido",
                schema: "CoreRH",
                table: "Colaboradores",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncHash",
                schema: "CoreRH",
                table: "Colaboradores",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncStatus",
                schema: "CoreRH",
                table: "Colaboradores",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.CreateTable(
                name: "ColaboradoresDatosLaborales",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Jornada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Turno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrupoNomina = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sindicalizado = table.Column<bool>(type: "bit", nullable: false),
                    NivelSalarial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NivelVisibilidadCompensacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rfc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Curp = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Nss = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Genero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoCivil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Domicilio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodigoPostal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ciudad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pais = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Enmascarado = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "MovimientosColaborador",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaEfectiva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorAnterior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorNuevo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Origen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenciaExterna = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreadoPor = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                filter: "[Rfc] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosColaborador_ColaboradorId",
                schema: "CoreRH",
                table: "MovimientosColaborador",
                column: "ColaboradorId");

            migrationBuilder.Sql("""
                INSERT INTO CoreRH.ColaboradoresDatosSensibles (Id, ColaboradorId, Rfc, Enmascarado, CreatedAt, IsActive)
                SELECT NEWID(), c.Id, c.Rfc, 0, GETUTCDATE(), 1
                FROM CoreRH.Colaboradores c
                WHERE c.Rfc IS NOT NULL
                  AND NOT EXISTS (SELECT 1 FROM CoreRH.ColaboradoresDatosSensibles s WHERE s.ColaboradorId = c.Id);

                UPDATE CoreRH.Colaboradores
                SET SyncStatus = 'Synced', ExternalSource = COALESCE(ExternalSource, 'Nomina')
                WHERE NominaSyncAt IS NOT NULL AND SyncStatus = 'Pending';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColaboradoresDatosLaborales",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "ColaboradoresDatosSensibles",
                schema: "CoreRH");

            migrationBuilder.DropTable(
                name: "MovimientosColaborador",
                schema: "CoreRH");

            migrationBuilder.DropColumn(
                name: "ExternalEmployeeId",
                schema: "CoreRH",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "ExternalSource",
                schema: "CoreRH",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "ExternalSystemCode",
                schema: "CoreRH",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "IsManualOverride",
                schema: "CoreRH",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "NombrePreferido",
                schema: "CoreRH",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "SyncHash",
                schema: "CoreRH",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "SyncStatus",
                schema: "CoreRH",
                table: "Colaboradores");
        }
    }
}
