using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMinimumCatalogsBuk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Catalogos");

            migrationBuilder.CreateTable(
                name: "Estados",
                schema: "Catalogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "MEX"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCiviles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jerarquias",
                schema: "Catalogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NivelOrden = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jerarquias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RazonesTermino",
                schema: "Catalogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiereComprobante = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RazonesTermino", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Municipios",
                schema: "Catalogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EstadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NumeroImss = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RazonSocial = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Rfc = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    EstadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "IX_Jerarquias_Codigo",
                schema: "Catalogos",
                table: "Jerarquias",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_EstadoId_Codigo",
                schema: "Catalogos",
                table: "Municipios",
                columns: new[] { "EstadoId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RazonesTermino_Codigo",
                schema: "Catalogos",
                table: "RazonesTermino",
                column: "Codigo",
                unique: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstadosCiviles",
                schema: "Catalogos");

            migrationBuilder.DropTable(
                name: "Jerarquias",
                schema: "Catalogos");

            migrationBuilder.DropTable(
                name: "Municipios",
                schema: "Catalogos");

            migrationBuilder.DropTable(
                name: "RazonesTermino",
                schema: "Catalogos");

            migrationBuilder.DropTable(
                name: "RegistrosPatronales",
                schema: "Catalogos");

            migrationBuilder.DropTable(
                name: "Estados",
                schema: "Catalogos");
        }
    }
}
