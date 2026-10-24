using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ClassPacksInEveryPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "billing",
                table: "PlanFeatures",
                columns: new[] { "FeatureCode", "PlanCode", "Limit" },
                values: new object[] { "class-packs", "free", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "billing",
                table: "PlanFeatures",
                keyColumns: new[] { "FeatureCode", "PlanCode" },
                keyValues: new object[] { "class-packs", "free" });
        }
    }
}
