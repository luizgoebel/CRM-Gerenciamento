using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarPrecisaoPreco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "pedido");

            migrationBuilder.RenameColumn(
                name: "Subtotal",
                table: "pedido_item",
                newName: "PrecoUnitario");

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "produto",
                type: "decimal(6,3)",
                precision: 6,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AddColumn<int>(
                name: "PedidoId1",
                table: "pedido_item",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_pedido_item_PedidoId1",
                table: "pedido_item",
                column: "PedidoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_pedido_item_pedido_PedidoId1",
                table: "pedido_item",
                column: "PedidoId1",
                principalTable: "pedido",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pedido_item_pedido_PedidoId1",
                table: "pedido_item");

            migrationBuilder.DropIndex(
                name: "IX_pedido_item_PedidoId1",
                table: "pedido_item");

            migrationBuilder.DropColumn(
                name: "PedidoId1",
                table: "pedido_item");

            migrationBuilder.RenameColumn(
                name: "PrecoUnitario",
                table: "pedido_item",
                newName: "Subtotal");

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "produto",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)",
                oldPrecision: 6,
                oldScale: 3);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "pedido",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
