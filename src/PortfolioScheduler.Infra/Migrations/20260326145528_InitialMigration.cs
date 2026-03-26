using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioScheduler.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetPrices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OpenPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ClosePrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MaxPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MinPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetPrices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MonthlyAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MasterAccountId = table.Column<long>(type: "bigint", nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MarketType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecommendedPortfolios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TerminationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendedPortfolios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrokerageAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokerageAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrokerageAccounts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioRebalances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    RebalanceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoldTicker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BoughtTicker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SaleAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RebalanceDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioRebalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioRebalances_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxEvents_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecommendedPortfolioId = table.Column<long>(type: "bigint", nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioItems_RecommendedPortfolios_RecommendedPortfolioId",
                        column: x => x.RecommendedPortfolioId,
                        principalTable: "RecommendedPortfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Custodies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrokerageAccountId = table.Column<long>(type: "bigint", nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    AveragePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Custodies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Custodies_BrokerageAccounts_BrokerageAccountId",
                        column: x => x.BrokerageAccountId,
                        principalTable: "BrokerageAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<long>(type: "bigint", nullable: false),
                    CustodyCustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deliveries_Custodies_CustodyCustomerId",
                        column: x => x.CustodyCustomerId,
                        principalTable: "Custodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deliveries_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetPrices_Ticker",
                table: "AssetPrices",
                column: "Ticker");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerageAccounts_AccountNumber",
                table: "BrokerageAccounts",
                column: "AccountNumber");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerageAccounts_CustomerId",
                table: "BrokerageAccounts",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Custodies_BrokerageAccountId",
                table: "Custodies",
                column: "BrokerageAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Active",
                table: "Customers",
                column: "Active",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Cpf",
                table: "Customers",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_CustodyCustomerId",
                table: "Deliveries",
                column: "CustodyCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_PurchaseOrderId",
                table: "Deliveries",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioItems_RecommendedPortfolioId",
                table: "PortfolioItems",
                column: "RecommendedPortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioRebalances_CustomerId",
                table: "PortfolioRebalances",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_ExecutionDate",
                table: "PurchaseOrders",
                column: "ExecutionDate");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_Ticker",
                table: "PurchaseOrders",
                column: "Ticker");

            migrationBuilder.CreateIndex(
                name: "IX_TaxEvents_CustomerId",
                table: "TaxEvents",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxEvents_NotSent",
                table: "TaxEvents",
                column: "IsSent",
                filter: "[IsSent] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetPrices");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "PortfolioItems");

            migrationBuilder.DropTable(
                name: "PortfolioRebalances");

            migrationBuilder.DropTable(
                name: "TaxEvents");

            migrationBuilder.DropTable(
                name: "Custodies");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "RecommendedPortfolios");

            migrationBuilder.DropTable(
                name: "BrokerageAccounts");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
