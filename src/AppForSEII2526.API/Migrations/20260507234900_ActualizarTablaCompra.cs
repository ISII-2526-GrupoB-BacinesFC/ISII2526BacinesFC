using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppForSEII2526.API.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarTablaCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "PriceAtPurchase",
                table: "PurchaseItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "PriceAtPurchase",
                table: "PurchaseItems");
        }
    }
}
