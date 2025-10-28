using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AleStock.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventarios_Productos_ProductoId",
                table: "Inventarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Productos_ProductoId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Usuarios_UsuarioId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalles_Productos_ProductoId",
                table: "PedidoDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Usuarios_CreadoPorId",
                table: "Pedidos");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventarios_Productos_ProductoId",
                table: "Inventarios",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Productos_ProductoId",
                table: "Movimientos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Usuarios_UsuarioId",
                table: "Movimientos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalles_Productos_ProductoId",
                table: "PedidoDetalles",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Usuarios_CreadoPorId",
                table: "Pedidos",
                column: "CreadoPorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventarios_Productos_ProductoId",
                table: "Inventarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Productos_ProductoId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Usuarios_UsuarioId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalles_Productos_ProductoId",
                table: "PedidoDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Usuarios_CreadoPorId",
                table: "Pedidos");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventarios_Productos_ProductoId",
                table: "Inventarios",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Productos_ProductoId",
                table: "Movimientos",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Usuarios_UsuarioId",
                table: "Movimientos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalles_Productos_ProductoId",
                table: "PedidoDetalles",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Usuarios_CreadoPorId",
                table: "Pedidos",
                column: "CreadoPorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
