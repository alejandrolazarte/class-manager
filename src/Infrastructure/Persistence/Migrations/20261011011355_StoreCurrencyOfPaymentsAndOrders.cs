using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StoreCurrencyOfPaymentsAndOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Payments",
                type: "nchar(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Orders",
                type: "nchar(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "ClassPackPurchases",
                type: "nchar(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            foreach (var table in new[] { "Payments", "Orders", "ClassPackPurchases" })
            {
                migrationBuilder.Sql(
                    $"""
                    UPDATE [Records]
                    SET [CurrencyCode] = [Businesses].[CurrencyCode]
                    FROM [{table}] AS [Records]
                    INNER JOIN [Businesses] ON [Businesses].[Id] = [Records].[TenantId];
                    """);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "ClassPackPurchases");
        }
    }
}
