using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameFamilyAppToStudentApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            RenameFeature(migrationBuilder, "family-app", "student-app");
            RenameRefreshTokenKind(migrationBuilder, "family", "student");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            RenameFeature(migrationBuilder, "student-app", "family-app");
            RenameRefreshTokenKind(migrationBuilder, "student", "family");
        }

        private static void RenameRefreshTokenKind(MigrationBuilder migrationBuilder, string oldKind, string newKind) =>
            migrationBuilder.Sql(
                $"IF OBJECT_ID(N'[identity].[RefreshTokens]') IS NOT NULL EXEC(N'UPDATE [identity].[RefreshTokens] SET [Kind] = ''{newKind}'' WHERE [Kind] = ''{oldKind}''')");

        private static void RenameFeature(MigrationBuilder migrationBuilder, string oldCode, string newCode)
        {
            migrationBuilder.InsertData(
                schema: "billing",
                table: "Features",
                columns: new[] { "Code", "AddOnListPrice", "Currency", "IsAddOn", "IsCounted" },
                values: new object[] { newCode, 5m, "USD", true, false });

            migrationBuilder.InsertData(
                schema: "billing",
                table: "PlanFeatures",
                columns: new[] { "FeatureCode", "PlanCode", "Limit" },
                values: new object[,]
                {
                    { newCode, "enterprise", null },
                    { newCode, "pro", null }
                });

            migrationBuilder.Sql($"UPDATE [billing].[SubscriptionFeatures] SET [FeatureCode] = '{newCode}' WHERE [FeatureCode] = '{oldCode}'");

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { oldCode, "enterprise" });

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { oldCode, "pro" });

            migrationBuilder.DeleteData(
                schema: "billing",
                table: "Features",
                keyColumn: "Code",
                keyValue: oldCode);
        }
    }
}
