using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationInDays",
                schema: "billing",
                table: "Plans",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "enterprise",
                column: "DurationInDays",
                value: null);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "free",
                column: "DurationInDays",
                value: 30);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "lite",
                column: "DurationInDays",
                value: null);

            migrationBuilder.UpdateData(
                schema: "billing",
                table: "Plans",
                keyColumn: "Code",
                keyValue: "pro",
                column: "DurationInDays",
                value: null);

            migrationBuilder.Sql("""
                UPDATE billing.Subscriptions
                SET EndsOn = DATEADD(day, 29, StartsOn)
                WHERE PlanCode = 'free' AND EndsOn IS NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInDays",
                schema: "billing",
                table: "Plans");
        }
    }
}
