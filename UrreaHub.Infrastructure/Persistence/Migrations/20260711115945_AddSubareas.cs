using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSubareas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departamentos_Areas_AreaId",
                schema: "CoreRH",
                table: "Departamentos");

            migrationBuilder.RenameColumn(
                name: "AreaId",
                schema: "CoreRH",
                table: "Departamentos",
                newName: "SubareaId");

            migrationBuilder.RenameIndex(
                name: "IX_Departamentos_AreaId",
                schema: "CoreRH",
                table: "Departamentos",
                newName: "IX_Departamentos_SubareaId");

            migrationBuilder.CreateTable(
                name: "Subareas",
                schema: "CoreRH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subareas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subareas_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "CoreRH",
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subareas_AreaId",
                schema: "CoreRH",
                table: "Subareas",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Subareas_Codigo",
                schema: "CoreRH",
                table: "Subareas",
                column: "Codigo",
                unique: true);

            migrationBuilder.Sql("INSERT INTO [CoreRH].[Subareas] (Id, Codigo, Nombre, AreaId, CreatedAt, IsActive) SELECT Id, Codigo + '-SA', Nombre + ' Subárea', Id, GETUTCDATE(), 1 FROM [CoreRH].[Areas];");

            migrationBuilder.AddForeignKey(
                name: "FK_Departamentos_Subareas_SubareaId",
                schema: "CoreRH",
                table: "Departamentos",
                column: "SubareaId",
                principalSchema: "CoreRH",
                principalTable: "Subareas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departamentos_Subareas_SubareaId",
                schema: "CoreRH",
                table: "Departamentos");

            migrationBuilder.DropTable(
                name: "Subareas",
                schema: "CoreRH");

            migrationBuilder.RenameColumn(
                name: "SubareaId",
                schema: "CoreRH",
                table: "Departamentos",
                newName: "AreaId");

            migrationBuilder.RenameIndex(
                name: "IX_Departamentos_SubareaId",
                schema: "CoreRH",
                table: "Departamentos",
                newName: "IX_Departamentos_AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departamentos_Areas_AreaId",
                schema: "CoreRH",
                table: "Departamentos",
                column: "AreaId",
                principalSchema: "CoreRH",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
