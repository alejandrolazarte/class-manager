using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PointWhoDidWhatToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "InvitedByUserId",
                table: "MemberInvitations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "InvitedByUserId",
                table: "ClientInvitations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.Sql(WhoDidWhatBackfill.AccountsToUsersOfTheBranch);
            migrationBuilder.Sql(WhoDidWhatBackfill.AccountsOutsideBranches);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedByUserId",
                table: "Users",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_DeletedByUserId",
                schema: "billing",
                table: "Subscriptions",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionFeatures_DeletedByUserId",
                schema: "billing",
                table: "SubscriptionFeatures",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_RecordedByUserId",
                table: "StockMovements",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_DeletedByUserId",
                table: "Payments",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RecordedByUserId",
                table: "Payments",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMembers_DeletedByUserId",
                table: "OrganizationMembers",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CancelledByUserId",
                table: "Orders",
                column: "CancelledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedByUserId",
                table: "Orders",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveredByUserId",
                table: "Orders",
                column: "DeliveredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentRecordedByUserId",
                table: "Orders",
                column: "PaymentRecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberInvitations_InvitedByUserId",
                table: "MemberInvitations",
                column: "InvitedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementRecords_DeletedByUserId",
                table: "MeasurementRecords",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementRecords_RecordedByUserId",
                table: "MeasurementRecords",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementDefinitions_DeletedByUserId",
                table: "MeasurementDefinitions",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultMonthlyFeeChanges_DeletedByUserId",
                table: "DefaultMonthlyFeeChanges",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomRoles_DeletedByUserId",
                table: "CustomRoles",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_RegisteredByUserId",
                table: "Clients",
                column: "RegisteredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvitations_ContactAdultConfirmedByUserId",
                table: "ClientInvitations",
                column: "ContactAdultConfirmedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInvitations_InvitedByUserId",
                table: "ClientInvitations",
                column: "InvitedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientBillingPlanChanges_DeletedByUserId",
                table: "ClientBillingPlanChanges",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_DeletedByUserId",
                table: "ClassPackPurchases",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassPackPurchases_RecordedByUserId",
                table: "ClassPackPurchases",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassFeedbacks_DeletedByUserId",
                table: "ClassFeedbacks",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_DeletedByUserId",
                table: "BusinessMembers",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_DeletedByUserId",
                table: "Announcements",
                column: "DeletedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Users_DeletedByUserId",
                table: "Announcements",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessMembers_Users_DeletedByUserId",
                table: "BusinessMembers",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassFeedbacks_Users_DeletedByUserId",
                table: "ClassFeedbacks",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPackPurchases_Users_DeletedByUserId",
                table: "ClassPackPurchases",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassPackPurchases_Users_RecordedByUserId",
                table: "ClassPackPurchases",
                column: "RecordedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientBillingPlanChanges_Users_DeletedByUserId",
                table: "ClientBillingPlanChanges",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientInvitations_Users_ContactAdultConfirmedByUserId",
                table: "ClientInvitations",
                column: "ContactAdultConfirmedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientInvitations_Users_InvitedByUserId",
                table: "ClientInvitations",
                column: "InvitedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Users_RegisteredByUserId",
                table: "Clients",
                column: "RegisteredByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomRoles_Users_DeletedByUserId",
                table: "CustomRoles",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultMonthlyFeeChanges_Users_DeletedByUserId",
                table: "DefaultMonthlyFeeChanges",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementDefinitions_Users_DeletedByUserId",
                table: "MeasurementDefinitions",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementRecords_Users_DeletedByUserId",
                table: "MeasurementRecords",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasurementRecords_Users_RecordedByUserId",
                table: "MeasurementRecords",
                column: "RecordedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberInvitations_Users_InvitedByUserId",
                table: "MemberInvitations",
                column: "InvitedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_CancelledByUserId",
                table: "Orders",
                column: "CancelledByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_CreatedByUserId",
                table: "Orders",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_DeliveredByUserId",
                table: "Orders",
                column: "DeliveredByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_PaymentRecordedByUserId",
                table: "Orders",
                column: "PaymentRecordedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMembers_Users_DeletedByUserId",
                table: "OrganizationMembers",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_DeletedByUserId",
                table: "Payments",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_RecordedByUserId",
                table: "Payments",
                column: "RecordedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Users_RecordedByUserId",
                table: "StockMovements",
                column: "RecordedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionFeatures_Users_DeletedByUserId",
                schema: "billing",
                table: "SubscriptionFeatures",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_DeletedByUserId",
                schema: "billing",
                table: "Subscriptions",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_DeletedByUserId",
                table: "Users",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Users_DeletedByUserId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessMembers_Users_DeletedByUserId",
                table: "BusinessMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassFeedbacks_Users_DeletedByUserId",
                table: "ClassFeedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassPackPurchases_Users_DeletedByUserId",
                table: "ClassPackPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassPackPurchases_Users_RecordedByUserId",
                table: "ClassPackPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientBillingPlanChanges_Users_DeletedByUserId",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientInvitations_Users_ContactAdultConfirmedByUserId",
                table: "ClientInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientInvitations_Users_InvitedByUserId",
                table: "ClientInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Users_RegisteredByUserId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomRoles_Users_DeletedByUserId",
                table: "CustomRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultMonthlyFeeChanges_Users_DeletedByUserId",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementDefinitions_Users_DeletedByUserId",
                table: "MeasurementDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementRecords_Users_DeletedByUserId",
                table: "MeasurementRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasurementRecords_Users_RecordedByUserId",
                table: "MeasurementRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberInvitations_Users_InvitedByUserId",
                table: "MemberInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_CancelledByUserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_CreatedByUserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_DeliveredByUserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_PaymentRecordedByUserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMembers_Users_DeletedByUserId",
                table: "OrganizationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_DeletedByUserId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_RecordedByUserId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Users_RecordedByUserId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionFeatures_Users_DeletedByUserId",
                schema: "billing",
                table: "SubscriptionFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_DeletedByUserId",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_DeletedByUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DeletedByUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_DeletedByUserId",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionFeatures_DeletedByUserId",
                schema: "billing",
                table: "SubscriptionFeatures");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_RecordedByUserId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_Payments_DeletedByUserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_RecordedByUserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMembers_DeletedByUserId",
                table: "OrganizationMembers");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CancelledByUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CreatedByUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveredByUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentRecordedByUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_MemberInvitations_InvitedByUserId",
                table: "MemberInvitations");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementRecords_DeletedByUserId",
                table: "MeasurementRecords");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementRecords_RecordedByUserId",
                table: "MeasurementRecords");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementDefinitions_DeletedByUserId",
                table: "MeasurementDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_DefaultMonthlyFeeChanges_DeletedByUserId",
                table: "DefaultMonthlyFeeChanges");

            migrationBuilder.DropIndex(
                name: "IX_CustomRoles_DeletedByUserId",
                table: "CustomRoles");

            migrationBuilder.DropIndex(
                name: "IX_Clients_RegisteredByUserId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_ClientInvitations_ContactAdultConfirmedByUserId",
                table: "ClientInvitations");

            migrationBuilder.DropIndex(
                name: "IX_ClientInvitations_InvitedByUserId",
                table: "ClientInvitations");

            migrationBuilder.DropIndex(
                name: "IX_ClientBillingPlanChanges_DeletedByUserId",
                table: "ClientBillingPlanChanges");

            migrationBuilder.DropIndex(
                name: "IX_ClassPackPurchases_DeletedByUserId",
                table: "ClassPackPurchases");

            migrationBuilder.DropIndex(
                name: "IX_ClassPackPurchases_RecordedByUserId",
                table: "ClassPackPurchases");

            migrationBuilder.DropIndex(
                name: "IX_ClassFeedbacks_DeletedByUserId",
                table: "ClassFeedbacks");

            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_DeletedByUserId",
                table: "BusinessMembers");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_DeletedByUserId",
                table: "Announcements");

            migrationBuilder.AlterColumn<Guid>(
                name: "InvitedByUserId",
                table: "MemberInvitations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "InvitedByUserId",
                table: "ClientInvitations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
