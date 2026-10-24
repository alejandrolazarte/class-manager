using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NewPlanPricesAndLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "branches",
                column: "AddOnListPrice",
                value: 20m);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "brand",
                columns: new[] { "AddOnListPrice", "Currency", "IsAddOn" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "import-export",
                columns: new[] { "AddOnListPrice", "Currency", "IsAddOn" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "shop",
                columns: new[] { "AddOnListPrice", "Currency", "IsAddOn" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "branches", "enterprise" },
                column: "Limit",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "students", "lite" },
                column: "Limit",
                value: 100);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "branches", "pro" },
                column: "Limit",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "students", "pro" },
                column: "Limit",
                value: 300);

            migrationBuilder.InsertData(
                schema: "billing",
                table: "PlanFeatures",
                columns: new[] { "FeatureCode", "PlanCode", "Limit" },
                values: new object[] { "import-export", "free", null });

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "lite",
                column: "ListPrice",
                value: 19m);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "pro",
                column: "ListPrice",
                value: 39m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "import-export", "free" });

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "branches",
                column: "AddOnListPrice",
                value: 10m);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "brand",
                columns: new[] { "AddOnListPrice", "Currency", "IsAddOn" },
                values: new object[] { 5m, "EUR", true });

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "import-export",
                columns: new[] { "AddOnListPrice", "Currency", "IsAddOn" },
                values: new object[] { 3m, "EUR", true });

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "shop",
                columns: new[] { "AddOnListPrice", "Currency", "IsAddOn" },
                values: new object[] { 5m, "EUR", true });

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "branches", "enterprise" },
                column: "Limit",
                value: null);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "students", "lite" },
                column: "Limit",
                value: 150);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "branches", "pro" },
                column: "Limit",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "students", "pro" },
                column: "Limit",
                value: null);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "lite",
                column: "ListPrice",
                value: 9m);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "pro",
                column: "ListPrice",
                value: 19m);
        }
    }
}
