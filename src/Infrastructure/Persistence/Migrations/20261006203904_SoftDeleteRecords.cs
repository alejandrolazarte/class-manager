using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteRecords : Migration
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

            migrationBuilder.DropIndex(
                name: "IX_ClassPackPurchases_TenantId_TrialLessonId",
                table: "ClassPackPurchases");

            migrationBuilder.DropIndex(
                name: "IX_ClassFeedbacks_TenantId_ClassSessionId_StudentId",
                table: "ClassFeedbacks");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "Payments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DefaultMonthlyFeeChanges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ClientBillingPlanChanges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "ClassPackPurchases",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ClassPackPurchases",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "ClassFeedbacks",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ClassFeedbacks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                table: "Announcements",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Announcements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE [DefaultMonthlyFeeChanges] SET [IsDeleted] = 1 WHERE [DeletedOn] IS NOT NULL");

            migrationBuilder.Sql("UPDATE [ClientBillingPlanChanges] SET [IsDeleted] = 1 WHERE [DeletedOn] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_IsDeleted_DeletedOn",
                table: "Payments",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultMonthlyFeeChanges_TenantId_EffectiveFrom",
                table: "DefaultMonthlyFeeChanges",
                columns: new[] { "TenantId", "EffectiveFrom" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DefaultMonthlyFeeChanges_IsDeleted_DeletedOn",
                table: "DefaultMonthlyFeeChanges",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_TenantId_ClientId_EffectiveFrom",
                table: "ClientBillingPlanChanges",
                columns: new[] { "TenantId", "ClientId", "EffectiveFrom" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClientBillingPlanChanges_IsDeleted_DeletedOn",
                table: "ClientBillingPlanChanges",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_TenantId_TrialLessonId",
                table: "ClassPackPurchases",
                columns: new[] { "TenantId", "TrialLessonId" },
                unique: true,
                filter: "[TrialLessonId] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClassPackPurchases_IsDeleted_DeletedOn",
                table: "ClassPackPurchases",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_ClassFeedbacks_TenantId_ClassSessionId_StudentId",
                table: "ClassFeedbacks",
                columns: new[] { "TenantId", "ClassSessionId", "StudentId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClassFeedbacks_IsDeleted_DeletedOn",
                table: "ClassFeedbacks",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Announcements_IsDeleted_DeletedOn",
                table: "Announcements",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_IsDeleted_DeletedOn",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_DefaultMonthlyFeeChanges_TenantId_EffectiveFrom",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DefaultMonthlyFeeChanges_IsDeleted_DeletedOn",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropIndex(
                name: "IX_ClientBillingPlanChanges_TenantId_ClientId_EffectiveFrom",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClientBillingPlanChanges_IsDeleted_DeletedOn",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropIndex(
                name: "IX_ClassPackPurchases_TenantId_TrialLessonId",
                table: "ClassPackPurchases");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClassPackPurchases_IsDeleted_DeletedOn",
                table: "ClassPackPurchases");

            migrationBuilder.DropIndex(
                name: "IX_ClassFeedbacks_TenantId_ClassSessionId_StudentId",
                table: "ClassFeedbacks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClassFeedbacks_IsDeleted_DeletedOn",
                table: "ClassFeedbacks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Announcements_IsDeleted_DeletedOn",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ClassPackPurchases");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ClassPackPurchases");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ClassFeedbacks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ClassFeedbacks");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Announcements");

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

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_TenantId_TrialLessonId",
                table: "ClassPackPurchases",
                columns: new[] { "TenantId", "TrialLessonId" },
                unique: true,
                filter: "[TrialLessonId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClassFeedbacks_TenantId_ClassSessionId_StudentId",
                table: "ClassFeedbacks",
                columns: new[] { "TenantId", "ClassSessionId", "StudentId" },
                unique: true);
        }
    }
}
