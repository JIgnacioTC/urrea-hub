using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HardeningCoreRhSecurityRfc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE s SET s.Rfc = c.Rfc
                FROM CoreRH.ColaboradoresDatosSensibles s
                INNER JOIN CoreRH.Colaboradores c ON c.Id = s.ColaboradorId
                WHERE s.Rfc IS NULL AND c.Rfc IS NOT NULL;

                INSERT INTO CoreRH.ColaboradoresDatosSensibles (Id, ColaboradorId, Rfc, Enmascarado, CreatedAt, IsActive)
                SELECT NEWID(), c.Id, c.Rfc, 0, GETUTCDATE(), 1
                FROM CoreRH.Colaboradores c
                WHERE c.Rfc IS NOT NULL
                  AND NOT EXISTS (SELECT 1 FROM CoreRH.ColaboradoresDatosSensibles s WHERE s.ColaboradorId = c.Id);
                """);

            migrationBuilder.DropIndex(
                name: "IX_Colaboradores_Rfc",
                schema: "CoreRH",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "Rfc",
                schema: "CoreRH",
                table: "Colaboradores");

            migrationBuilder.EnsureSchema(
                name: "Seguridad");

            migrationBuilder.CreateTable(
                name: "Permisos",
                schema: "Seguridad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Modulo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Seguridad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ColaboradorRoles",
                schema: "Seguridad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "RolPermisos",
                schema: "Seguridad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermisoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_ExternalSource_ExternalEmployeeId",
                schema: "CoreRH",
                table: "Colaboradores",
                columns: new[] { "ExternalSource", "ExternalEmployeeId" },
                unique: true,
                filter: "[ExternalSource] IS NOT NULL AND [ExternalEmployeeId] IS NOT NULL");

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
                name: "IX_Permisos_Codigo",
                schema: "Seguridad",
                table: "Permisos",
                column: "Codigo",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColaboradorRoles",
                schema: "Seguridad");

            migrationBuilder.DropTable(
                name: "RolPermisos",
                schema: "Seguridad");

            migrationBuilder.DropTable(
                name: "Permisos",
                schema: "Seguridad");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Seguridad");

            migrationBuilder.DropIndex(
                name: "IX_Colaboradores_ExternalSource_ExternalEmployeeId",
                schema: "CoreRH",
                table: "Colaboradores");

            migrationBuilder.AddColumn<string>(
                name: "Rfc",
                schema: "CoreRH",
                table: "Colaboradores",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_Rfc",
                schema: "CoreRH",
                table: "Colaboradores",
                column: "Rfc",
                unique: true,
                filter: "[Rfc] IS NOT NULL");

            migrationBuilder.Sql("""
                UPDATE c SET c.Rfc = s.Rfc
                FROM CoreRH.Colaboradores c
                INNER JOIN CoreRH.ColaboradoresDatosSensibles s ON s.ColaboradorId = c.Id
                WHERE c.Rfc IS NULL AND s.Rfc IS NOT NULL;
                """);
        }
    }
}
