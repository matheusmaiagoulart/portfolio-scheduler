using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioScheduler.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIndexCustomerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Active",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Active",
                table: "Customers",
                column: "Active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Active",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Active",
                table: "Customers",
                column: "Active",
                unique: true);
        }
    }
}
