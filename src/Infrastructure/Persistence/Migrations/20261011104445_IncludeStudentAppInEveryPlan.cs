using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IncludeStudentAppInEveryPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "student-app",
                columns: new[] { "AddOnListPrice", "Currency", "IsAddOn" },
                values: new object[] { null, null, false });

            migrationBuilder.InsertData(
                schema: "billing",
                table: "PlanFeatures",
                columns: new[] { "FeatureCode", "PlanCode", "Limit" },
                values: new object[,]
                {
                    { "student-app", "free", null },
                    { "student-app", "lite", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "student-app", "free" });

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "student-app", "lite" });

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "student-app",
                columns: new[] { "AddOnListPrice", "Currency", "IsAddOn" },
                values: new object[] { 5m, "USD", true });
        }
    }
}
