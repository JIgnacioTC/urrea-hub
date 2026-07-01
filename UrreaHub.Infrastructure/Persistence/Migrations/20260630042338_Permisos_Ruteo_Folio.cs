using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Permisos_Ruteo_Folio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AreaDestinoId",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdLegacy",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebhookUrl",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folio",
                schema: "Vacaciones",
                table: "Solicitudes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TiposAusencia_AreaDestinoId",
                schema: "Vacaciones",
                table: "TiposAusencia",
                column: "AreaDestinoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TiposAusencia_Areas_AreaDestinoId",
                schema: "Vacaciones",
                table: "TiposAusencia",
                column: "AreaDestinoId",
                principalSchema: "CoreRH",
                principalTable: "Areas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TiposAusencia_Areas_AreaDestinoId",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropIndex(
                name: "IX_TiposAusencia_AreaDestinoId",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "AreaDestinoId",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "IdLegacy",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "WebhookUrl",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "Folio",
                schema: "Vacaciones",
                table: "Solicitudes");
        }
    }
}
