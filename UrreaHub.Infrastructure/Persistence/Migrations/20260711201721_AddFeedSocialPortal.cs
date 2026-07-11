using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedSocialPortal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comentarios",
                schema: "Portal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Contenido = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comentarios_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comentarios_Publicaciones_PublicacionId",
                        column: x => x.PublicacionId,
                        principalSchema: "Portal",
                        principalTable: "Publicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reacciones",
                schema: "Portal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PublicacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reacciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reacciones_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reacciones_Publicaciones_PublicacionId",
                        column: x => x.PublicacionId,
                        principalSchema: "Portal",
                        principalTable: "Publicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_ColaboradorId",
                schema: "Portal",
                table: "Comentarios",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_PublicacionId",
                schema: "Portal",
                table: "Comentarios",
                column: "PublicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Reacciones_ColaboradorId",
                schema: "Portal",
                table: "Reacciones",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reacciones_PublicacionId_ColaboradorId",
                schema: "Portal",
                table: "Reacciones",
                columns: new[] { "PublicacionId", "ColaboradorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comentarios",
                schema: "Portal");

            migrationBuilder.DropTable(
                name: "Reacciones",
                schema: "Portal");
        }
    }
}
