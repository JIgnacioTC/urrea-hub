using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFase1AuthAndNominaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NominaSyncAt",
                schema: "CoreRH",
                table: "Colaboradores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rfc",
                schema: "CoreRH",
                table: "Colaboradores",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CuentasAcceso",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DebeCambiarPassword = table.Column<bool>(type: "bit", nullable: false),
                    EsRhAdmin = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasAcceso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasAcceso_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Colaboradores_Rfc",
                schema: "CoreRH",
                table: "Colaboradores",
                column: "Rfc",
                unique: true,
                filter: "[Rfc] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasAcceso_ColaboradorId",
                schema: "CoreRH",
                table: "CuentasAcceso",
                column: "ColaboradorId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CuentasAcceso",
                schema: "CoreRH");

            migrationBuilder.DropIndex(
                name: "IX_Colaboradores_Rfc",
                schema: "CoreRH",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "NominaSyncAt",
                schema: "CoreRH",
                table: "Colaboradores");

            migrationBuilder.DropColumn(
                name: "Rfc",
                schema: "CoreRH",
                table: "Colaboradores");
        }
    }
}
