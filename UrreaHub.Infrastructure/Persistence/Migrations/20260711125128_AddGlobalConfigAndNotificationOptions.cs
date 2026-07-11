using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalConfigAndNotificationOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotificarCorreo",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificarTeams",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ConfiguracionesGlobales",
                schema: "Plataforma",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Clave = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesGlobales", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesGlobales_Clave",
                schema: "Plataforma",
                table: "ConfiguracionesGlobales",
                column: "Clave",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesGlobales",
                schema: "Plataforma");

            migrationBuilder.DropColumn(
                name: "NotificarCorreo",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "NotificarTeams",
                schema: "Vacaciones",
                table: "TiposAusencia");
        }
    }
}
