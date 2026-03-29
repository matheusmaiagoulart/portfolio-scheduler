using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioScheduler.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrokerageAccounts_Customers_CustomerId",
                table: "BrokerageAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Custodies_CustodyCustomerId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_PortfolioRebalances_Customers_CustomerId",
                table: "PortfolioRebalances");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxEvents_Customers_CustomerId",
                table: "TaxEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_BrokerageAccounts_Customers_CustomerId",
                table: "BrokerageAccounts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrokerageAccounts_Customers_CustomerId",
                table: "BrokerageAccounts");

            migrationBuilder.AddForeignKey(
                name: "FK_BrokerageAccounts_Customers_CustomerId",
                table: "BrokerageAccounts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Custodies_CustodyCustomerId",
                table: "Deliveries",
                column: "CustodyCustomerId",
                principalTable: "Custodies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PortfolioRebalances_Customers_CustomerId",
                table: "PortfolioRebalances",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxEvents_Customers_CustomerId",
                table: "TaxEvents",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
