using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceCatalogPhotosWithDocumentStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [billing].[SubscriptionFeatures] WHERE [FeatureCode] = N'catalog-photos';");

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "catalog-photos", "enterprise" });

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "catalog-photos", "free" });

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "catalog-photos", "lite" });

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "catalog-photos", "pro" });

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "catalog-photos");

            migrationBuilder.InsertData(
                schema: "billing",
                table: "Features",
                columns: new[] { "Code", "AddOnListPrice", "Currency", "IsAddOn", "IsCounted" },
                values: new object[] { "document-storage", 2m, "USD", true, true });

            migrationBuilder.InsertData(
                schema: "billing",
                table: "PlanFeatures",
                columns: new[] { "FeatureCode", "PlanCode", "Limit" },
                values: new object[,]
                {
                    { "document-storage", "enterprise", 10240 },
                    { "document-storage", "free", 100 },
                    { "document-storage", "lite", 1024 },
                    { "document-storage", "pro", 5120 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "document-storage", "enterprise" });

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "document-storage", "free" });

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "document-storage", "lite" });

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "document-storage", "pro" });

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: "document-storage");

            migrationBuilder.InsertData(
                schema: "billing",
                table: "Features",
                columns: new[] { "Code", "AddOnListPrice", "Currency", "IsAddOn", "IsCounted" },
                values: new object[] { "catalog-photos", 3m, "USD", true, true });

            migrationBuilder.InsertData(
                schema: "billing",
                table: "PlanFeatures",
                columns: new[] { "FeatureCode", "PlanCode", "Limit" },
                values: new object[,]
                {
                    { "catalog-photos", "enterprise", 5 },
                    { "catalog-photos", "free", 1 },
                    { "catalog-photos", "lite", 5 },
                    { "catalog-photos", "pro", 5 }
                });
        }
    }
}
