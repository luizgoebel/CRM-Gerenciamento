using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarMapeamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pedido_item_pedido_PedidoId1",
                table: "pedido_item");

            migrationBuilder.DropForeignKey(
                name: "FK_pedido_item_produto_ProdutoId1",
                table: "pedido_item");

            migrationBuilder.DropIndex(
                name: "IX_pedido_item_PedidoId1",
                table: "pedido_item");

            migrationBuilder.DropIndex(
                name: "IX_pedido_item_ProdutoId1",
                table: "pedido_item");

            migrationBuilder.DropColumn(
                name: "PedidoId1",
                table: "pedido_item");

            migrationBuilder.DropColumn(
                name: "ProdutoId1",
                table: "pedido_item");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PedidoId1",
                table: "pedido_item",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProdutoId1",
                table: "pedido_item",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_pedido_item_PedidoId1",
                table: "pedido_item",
                column: "PedidoId1");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_item_ProdutoId1",
                table: "pedido_item",
                column: "ProdutoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_pedido_item_pedido_PedidoId1",
                table: "pedido_item",
                column: "PedidoId1",
                principalTable: "pedido",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_pedido_item_produto_ProdutoId1",
                table: "pedido_item",
                column: "ProdutoId1",
                principalTable: "produto",
                principalColumn: "Id");
        }
    }
}
