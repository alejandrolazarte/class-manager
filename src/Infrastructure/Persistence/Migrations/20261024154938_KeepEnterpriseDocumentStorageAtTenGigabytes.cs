using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class KeepEnterpriseDocumentStorageAtTenGigabytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "document-storage", "enterprise" },
                column: "Limit",
                value: 10240);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "document-storage", "enterprise" },
                column: "Limit",
                value: 20480);
        }
    }
}
