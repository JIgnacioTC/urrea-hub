using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTiPlatformAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Plataforma");

            migrationBuilder.AddColumn<bool>(
                name: "EsTiAdmin",
                schema: "CoreRH",
                table: "CuentasAcceso",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Metadatos",
                schema: "Plataforma",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Origen = table.Column<int>(type: "int", nullable: false),
                    Etiqueta = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VersionTag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ContenidoJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MigracionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreadoPorColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metadatos", x => x.Id);
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Metadatos",
                schema: "Plataforma");

            migrationBuilder.DropColumn(
                name: "EsTiAdmin",
                schema: "CoreRH",
                table: "CuentasAcceso");
        }
    }
}
