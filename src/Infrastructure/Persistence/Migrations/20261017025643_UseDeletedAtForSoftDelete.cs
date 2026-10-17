using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UseDeletedAtForSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_SubscriberId",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionFeatures_SubscriptionId_FeatureCode",
                schema: "billing",
                table: "SubscriptionFeatures");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_IsDeleted_DeletedOn",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrganizationMembers_IsDeleted_DeletedOn",
                table: "OrganizationMembers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MeasurementRecords_IsDeleted_DeletedOn",
                table: "MeasurementRecords");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MeasurementDefinitions_IsDeleted_DeletedOn",
                table: "MeasurementDefinitions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DefaultMonthlyFeeChanges_IsDeleted_DeletedOn",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CustomRoles_IsDeleted_DeletedOn",
                table: "CustomRoles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClientBillingPlanChanges_IsDeleted_DeletedOn",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClassPackPurchases_IsDeleted_DeletedOn",
                table: "ClassPackPurchases");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClassFeedbacks_IsDeleted_DeletedOn",
                table: "ClassFeedbacks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BusinessMembers_IsDeleted_DeletedOn",
                table: "BusinessMembers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Announcements_IsDeleted_DeletedOn",
                table: "Announcements");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                schema: "billing",
                table: "Subscriptions",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                schema: "billing",
                table: "SubscriptionFeatures",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                table: "Payments",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                table: "OrganizationMembers",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                table: "MeasurementRecords",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                table: "MeasurementDefinitions",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                table: "DefaultMonthlyFeeChanges",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                table: "CustomRoles",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                table: "ClientBillingPlanChanges",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                table: "ClassPackPurchases",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                table: "ClassFeedbacks",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                table: "BusinessMembers",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedOn",
                table: "Announcements",
                newName: "DeletedAt");

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                schema: "billing",
                table: "Subscriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "billing",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                schema: "billing",
                table: "SubscriptionFeatures",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "billing",
                table: "SubscriptionFeatures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "OrganizationMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "MeasurementRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "MeasurementDefinitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "DefaultMonthlyFeeChanges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "CustomRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "ClientBillingPlanChanges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "ClassPackPurchases",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "ClassFeedbacks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "BusinessMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "Announcements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("UPDATE [billing].[Subscriptions] SET [IsDeleted] = 1 WHERE [DeletedAt] IS NOT NULL");

            migrationBuilder.Sql("UPDATE [billing].[SubscriptionFeatures] SET [IsDeleted] = 1 WHERE [DeletedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriberId",
                schema: "billing",
                table: "Subscriptions",
                column: "SubscriberId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Subscriptions_IsDeleted_DeletedAt",
                schema: "billing",
                table: "Subscriptions",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionFeatures_SubscriptionId_FeatureCode",
                schema: "billing",
                table: "SubscriptionFeatures",
                columns: new[] { "SubscriptionId", "FeatureCode" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SubscriptionFeatures_IsDeleted_DeletedAt",
                schema: "billing",
                table: "SubscriptionFeatures",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_IsDeleted_DeletedAt",
                table: "Payments",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrganizationMembers_IsDeleted_DeletedAt",
                table: "OrganizationMembers",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MeasurementRecords_IsDeleted_DeletedAt",
                table: "MeasurementRecords",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MeasurementDefinitions_IsDeleted_DeletedAt",
                table: "MeasurementDefinitions",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DefaultMonthlyFeeChanges_IsDeleted_DeletedAt",
                table: "DefaultMonthlyFeeChanges",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CustomRoles_IsDeleted_DeletedAt",
                table: "CustomRoles",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClientBillingPlanChanges_IsDeleted_DeletedAt",
                table: "ClientBillingPlanChanges",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClassPackPurchases_IsDeleted_DeletedAt",
                table: "ClassPackPurchases",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClassFeedbacks_IsDeleted_DeletedAt",
                table: "ClassFeedbacks",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BusinessMembers_IsDeleted_DeletedAt",
                table: "BusinessMembers",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Announcements_IsDeleted_DeletedAt",
                table: "Announcements",
                sql: "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_SubscriberId",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Subscriptions_IsDeleted_DeletedAt",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionFeatures_SubscriptionId_FeatureCode",
                schema: "billing",
                table: "SubscriptionFeatures");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SubscriptionFeatures_IsDeleted_DeletedAt",
                schema: "billing",
                table: "SubscriptionFeatures");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_IsDeleted_DeletedAt",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrganizationMembers_IsDeleted_DeletedAt",
                table: "OrganizationMembers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MeasurementRecords_IsDeleted_DeletedAt",
                table: "MeasurementRecords");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MeasurementDefinitions_IsDeleted_DeletedAt",
                table: "MeasurementDefinitions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DefaultMonthlyFeeChanges_IsDeleted_DeletedAt",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CustomRoles_IsDeleted_DeletedAt",
                table: "CustomRoles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClientBillingPlanChanges_IsDeleted_DeletedAt",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClassPackPurchases_IsDeleted_DeletedAt",
                table: "ClassPackPurchases");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ClassFeedbacks_IsDeleted_DeletedAt",
                table: "ClassFeedbacks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BusinessMembers_IsDeleted_DeletedAt",
                table: "BusinessMembers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Announcements_IsDeleted_DeletedAt",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "billing",
                table: "SubscriptionFeatures");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "billing",
                table: "SubscriptionFeatures");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "MeasurementRecords");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "MeasurementDefinitions");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "CustomRoles");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ClassPackPurchases");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ClassFeedbacks");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "BusinessMembers");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Announcements");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "billing",
                table: "Subscriptions",
                newName: "DeletedOn");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "billing",
                table: "SubscriptionFeatures",
                newName: "DeletedOn");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Payments",
                newName: "DeletedOn");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "OrganizationMembers",
                newName: "DeletedOn");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "MeasurementRecords",
                newName: "DeletedOn");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "MeasurementDefinitions",
                newName: "DeletedOn");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "DefaultMonthlyFeeChanges",
                newName: "DeletedOn");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "CustomRoles",
                newName: "DeletedOn");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "ClientBillingPlanChanges",
                newName: "DeletedOn");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "ClassPackPurchases",
                newName: "DeletedOn");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "ClassFeedbacks",
                newName: "DeletedOn");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "BusinessMembers",
                newName: "DeletedOn");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Announcements",
                newName: "DeletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriberId",
                schema: "billing",
                table: "Subscriptions",
                column: "SubscriberId",
                unique: true,
                filter: "[DeletedOn] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionFeatures_SubscriptionId_FeatureCode",
                schema: "billing",
                table: "SubscriptionFeatures",
                columns: new[] { "SubscriptionId", "FeatureCode" },
                unique: true,
                filter: "[DeletedOn] IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_IsDeleted_DeletedOn",
                table: "Payments",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrganizationMembers_IsDeleted_DeletedOn",
                table: "OrganizationMembers",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MeasurementRecords_IsDeleted_DeletedOn",
                table: "MeasurementRecords",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MeasurementDefinitions_IsDeleted_DeletedOn",
                table: "MeasurementDefinitions",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DefaultMonthlyFeeChanges_IsDeleted_DeletedOn",
                table: "DefaultMonthlyFeeChanges",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CustomRoles_IsDeleted_DeletedOn",
                table: "CustomRoles",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClientBillingPlanChanges_IsDeleted_DeletedOn",
                table: "ClientBillingPlanChanges",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClassPackPurchases_IsDeleted_DeletedOn",
                table: "ClassPackPurchases",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ClassFeedbacks_IsDeleted_DeletedOn",
                table: "ClassFeedbacks",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BusinessMembers_IsDeleted_DeletedOn",
                table: "BusinessMembers",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Announcements_IsDeleted_DeletedOn",
                table: "Announcements",
                sql: "([IsDeleted] = 0 AND [DeletedOn] IS NULL) OR ([IsDeleted] = 1 AND [DeletedOn] IS NOT NULL)");
        }
    }
}
