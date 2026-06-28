using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPermisosLftModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaseLegalLft",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Categoria",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiasMaximosAnuales",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiasMaximosEvento",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsParcial",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Icono",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Orden",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteMultiDia",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Remunerado",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiereComprobante",
                schema: "Vacaciones",
                table: "TiposAusencia",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsDiaCompleto",
                schema: "Vacaciones",
                table: "Solicitudes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraFin",
                schema: "Vacaciones",
                table: "Solicitudes",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraInicio",
                schema: "Vacaciones",
                table: "Solicitudes",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseLegalLft",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "Categoria",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "DiasMaximosAnuales",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "DiasMaximosEvento",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "EsParcial",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "Icono",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "Orden",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "PermiteMultiDia",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "Remunerado",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "RequiereComprobante",
                schema: "Vacaciones",
                table: "TiposAusencia");

            migrationBuilder.DropColumn(
                name: "EsDiaCompleto",
                schema: "Vacaciones",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "HoraFin",
                schema: "Vacaciones",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                schema: "Vacaciones",
                table: "Solicitudes");
        }
    }
}
