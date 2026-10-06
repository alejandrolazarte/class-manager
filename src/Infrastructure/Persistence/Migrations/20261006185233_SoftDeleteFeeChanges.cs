using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteFeeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DefaultMonthlyFeeChanges_TenantId_EffectiveFrom",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropIndex(
                name: "IX_ClientBillingPlanChanges_TenantId_ClientId_EffectiveFrom",
                table: "ClientBillingPlanChanges");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "DefaultMonthlyFeeChanges",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "ClientBillingPlanChanges",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DefaultMonthlyFeeChanges_TenantId_EffectiveFrom",
                table: "DefaultMonthlyFeeChanges",
                columns: new[] { "TenantId", "EffectiveFrom" },
                unique: true,
                filter: "[DeletedOn] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_TenantId_ClientId_EffectiveFrom",
                table: "ClientBillingPlanChanges",
                columns: new[] { "TenantId", "ClientId", "EffectiveFrom" },
                unique: true,
                filter: "[DeletedOn] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DefaultMonthlyFeeChanges_TenantId_EffectiveFrom",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropIndex(
                name: "IX_ClientBillingPlanChanges_TenantId_ClientId_EffectiveFrom",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ClientBillingPlanChanges");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultMonthlyFeeChanges_TenantId_EffectiveFrom",
                table: "DefaultMonthlyFeeChanges",
                columns: new[] { "TenantId", "EffectiveFrom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_TenantId_ClientId_EffectiveFrom",
                table: "ClientBillingPlanChanges",
                columns: new[] { "TenantId", "ClientId", "EffectiveFrom" },
                unique: true);
        }
    }
}
