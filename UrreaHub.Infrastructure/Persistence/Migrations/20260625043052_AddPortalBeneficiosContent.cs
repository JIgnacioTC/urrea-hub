using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrreaHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPortalBeneficiosContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Portal");

            migrationBuilder.CreateTable(
                name: "ContenidosModulo",
                schema: "Portal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodigoModulo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subtitulo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Publicado = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContenidosModulo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConveniosProveedores",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Proveedor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descuento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vigencia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoPromocional = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConveniosProveedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiasFestivos",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiasFestivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosCorporativos",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Categoria = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Paginas = table.Column<int>(type: "int", nullable: true),
                    UrlDocumento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosCorporativos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductosTienda",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PuntosRequeridos = table.Column<int>(type: "int", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Icono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gradiente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Destacado = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductosTienda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Publicaciones",
                schema: "Portal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AutorNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AutorRol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AutorIniciales = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GradienteImagen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Likes = table.Column<int>(type: "int", nullable: false),
                    Comentarios = table.Column<int>(type: "int", nullable: false),
                    Compartidos = table.Column<int>(type: "int", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publicaciones_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SaldosPuntos",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Puntos = table.Column<int>(type: "int", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaldosPuntos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaldosPuntos_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CanjesTienda",
                schema: "Beneficios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PuntosUsados = table.Column<int>(type: "int", nullable: false),
                    FechaCanje = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanjesTienda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanjesTienda_Colaboradores_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalSchema: "CoreRH",
                        principalTable: "Colaboradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanjesTienda_ProductosTienda_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "Beneficios",
                        principalTable: "ProductosTienda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CanjesTienda_ColaboradorId",
                schema: "Beneficios",
                table: "CanjesTienda",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_CanjesTienda_ProductoId",
                schema: "Beneficios",
                table: "CanjesTienda",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContenidosModulo_CodigoModulo",
                schema: "Portal",
                table: "ContenidosModulo",
                column: "CodigoModulo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConveniosProveedores_Codigo",
                schema: "Beneficios",
                table: "ConveniosProveedores",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosCorporativos_Codigo",
                schema: "Beneficios",
                table: "DocumentosCorporativos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductosTienda_Codigo",
                schema: "Beneficios",
                table: "ProductosTienda",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_ColaboradorId",
                schema: "Portal",
                table: "Publicaciones",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_SaldosPuntos_ColaboradorId",
                schema: "Beneficios",
                table: "SaldosPuntos",
                column: "ColaboradorId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CanjesTienda",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "ContenidosModulo",
                schema: "Portal");

            migrationBuilder.DropTable(
                name: "ConveniosProveedores",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "DiasFestivos",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "DocumentosCorporativos",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "Publicaciones",
                schema: "Portal");

            migrationBuilder.DropTable(
                name: "SaldosPuntos",
                schema: "Beneficios");

            migrationBuilder.DropTable(
                name: "ProductosTienda",
                schema: "Beneficios");
        }
    }
}
