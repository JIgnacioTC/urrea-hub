using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCadenaAprobacionAusencias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aprobaciones_Colaboradores_AprobadorId",
                schema: "Vacaciones",
                table: "Aprobaciones");

            migrationBuilder.AddColumn<bool>(
                name: "RequiereAprobacionDH",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiereAprobacionJefe",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiereAprobacionNominas",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDecision",
                schema: "Vacaciones",
                table: "Aprobaciones",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<Guid>(
                name: "AprobadorId",
                schema: "Vacaciones",
                table: "Aprobaciones",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "Nivel",
                schema: "Vacaciones",
                table: "Aprobaciones",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Orden",
                schema: "Vacaciones",
                table: "Aprobaciones",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Aprobaciones_Colaboradores_AprobadorId",
                schema: "Vacaciones",
                table: "Aprobaciones",
                column: "AprobadorId",
                principalSchema: "CoreRH",
                principalTable: "Colaboradores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aprobaciones_Colaboradores_AprobadorId",
                schema: "Vacaciones",
                table: "Aprobaciones");

            migrationBuilder.DropColumn(
                name: "RequiereAprobacionDH",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "RequiereAprobacionJefe",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "RequiereAprobacionNominas",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "Nivel",
                schema: "Vacaciones",
                table: "Aprobaciones");

            migrationBuilder.DropColumn(
                name: "Orden",
                schema: "Vacaciones",
                table: "Aprobaciones");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDecision",
                schema: "Vacaciones",
                table: "Aprobaciones",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AprobadorId",
                schema: "Vacaciones",
                table: "Aprobaciones",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Aprobaciones_Colaboradores_AprobadorId",
                schema: "Vacaciones",
                table: "Aprobaciones",
                column: "AprobadorId",
                principalSchema: "CoreRH",
                principalTable: "Colaboradores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
