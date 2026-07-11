using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMercerFieldsToPuesto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comunicacion",
                schema: "CoreRH",
                table: "Puestos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducacionRequerida",
                schema: "CoreRH",
                table: "Puestos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExperienciaAnios",
                schema: "CoreRH",
                table: "Puestos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GradoMercer",
                schema: "CoreRH",
                table: "Puestos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Impacto",
                schema: "CoreRH",
                table: "Puestos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Innovacion",
                schema: "CoreRH",
                table: "Puestos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonalCargoDirecto",
                schema: "CoreRH",
                table: "Puestos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonalCargoIndirecto",
                schema: "CoreRH",
                table: "Puestos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PresupuestoAnual",
                schema: "CoreRH",
                table: "Puestos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comunicacion",
                schema: "CoreRH",
                table: "Puestos");

            migrationBuilder.DropColumn(
                name: "EducacionRequerida",
                schema: "CoreRH",
                table: "Puestos");

            migrationBuilder.DropColumn(
                name: "ExperienciaAnios",
                schema: "CoreRH",
                table: "Puestos");

            migrationBuilder.DropColumn(
                name: "GradoMercer",
                schema: "CoreRH",
                table: "Puestos");

            migrationBuilder.DropColumn(
                name: "Impacto",
                schema: "CoreRH",
                table: "Puestos");

            migrationBuilder.DropColumn(
                name: "Innovacion",
                schema: "CoreRH",
                table: "Puestos");

            migrationBuilder.DropColumn(
                name: "PersonalCargoDirecto",
                schema: "CoreRH",
                table: "Puestos");

            migrationBuilder.DropColumn(
                name: "PersonalCargoIndirecto",
                schema: "CoreRH",
                table: "Puestos");

            migrationBuilder.DropColumn(
                name: "PresupuestoAnual",
                schema: "CoreRH",
                table: "Puestos");
        }
    }
}
