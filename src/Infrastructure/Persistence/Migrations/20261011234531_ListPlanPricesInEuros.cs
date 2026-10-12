using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ListPlanPricesInEuros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "branches",
                column: "Currency",
                value: "EUR");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "brand",
                column: "Currency",
                value: "EUR");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "document-storage",
                column: "Currency",
                value: "EUR");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "import-export",
                column: "Currency",
                value: "EUR");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "shop",
                column: "Currency",
                value: "EUR");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "enterprise",
                column: "Currency",
                value: "EUR");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "free",
                column: "Currency",
                value: "EUR");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "lite",
                column: "Currency",
                value: "EUR");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "pro",
                column: "Currency",
                value: "EUR");

            migrationBuilder.Sql(
                """
                UPDATE [billing].[Subscriptions]
                SET [Currency] = 'EUR'
                WHERE [Currency] = 'USD' AND [Price] = 0 AND [DeletedOn] IS NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "branches",
                column: "Currency",
                value: "USD");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "brand",
                column: "Currency",
                value: "USD");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "document-storage",
                column: "Currency",
                value: "USD");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "import-export",
                column: "Currency",
                value: "USD");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "shop",
                column: "Currency",
                value: "USD");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "enterprise",
                column: "Currency",
                value: "USD");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "free",
                column: "Currency",
                value: "USD");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "lite",
                column: "Currency",
                value: "USD");

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "pro",
                column: "Currency",
                value: "USD");
        }
    }
}
