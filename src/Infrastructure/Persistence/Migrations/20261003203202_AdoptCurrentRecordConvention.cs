using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdoptCurrentRecordConvention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_SubscriberId",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_SubscriberId_StartsOn",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionFeatures_SubscriptionId",
                schema: "billing",
                table: "SubscriptionFeatures");

            migrationBuilder.DropColumn(
                name: "StartsOn",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.RenameColumn(
                name: "EndsOn",
                schema: "billing",
                table: "Subscriptions",
                newName: "ExpiredOn");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "billing",
                table: "Subscriptions",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "EndsOn",
                schema: "billing",
                table: "SubscriptionFeatures",
                newName: "ExpiredOn");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                schema: "billing",
                table: "Subscriptions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                schema: "billing",
                table: "SubscriptionFeatures",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.Sql("""
                UPDATE billing.SubscriptionFeatures
                SET CreatedOn = CAST(StartsOn AS datetimeoffset);
                """);

            migrationBuilder.DropColumn(
                name: "StartsOn",
                schema: "billing",
                table: "SubscriptionFeatures");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedOn",
                schema: "billing",
                table: "SubscriptionFeatures",
                type: "datetimeoffset",
                nullable: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_SubscriberId",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionFeatures_SubscriptionId_FeatureCode",
                schema: "billing",
                table: "SubscriptionFeatures");

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartsOn",
                schema: "billing",
                table: "Subscriptions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartsOn",
                schema: "billing",
                table: "SubscriptionFeatures",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.Sql("""
                UPDATE billing.Subscriptions SET StartsOn = CAST(CreatedOn AS date);
                UPDATE billing.SubscriptionFeatures SET StartsOn = CAST(CreatedOn AS date);
                """);

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "billing",
                table: "SubscriptionFeatures");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                schema: "billing",
                table: "SubscriptionFeatures");

            migrationBuilder.RenameColumn(
                name: "ExpiredOn",
                schema: "billing",
                table: "Subscriptions",
                newName: "EndsOn");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                schema: "billing",
                table: "Subscriptions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ExpiredOn",
                schema: "billing",
                table: "SubscriptionFeatures",
                newName: "EndsOn");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriberId",
                schema: "billing",
                table: "Subscriptions",
                column: "SubscriberId",
                unique: true,
                filter: "[EndsOn] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriberId_StartsOn",
                schema: "billing",
                table: "Subscriptions",
                columns: new[] { "SubscriberId", "StartsOn" });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionFeatures_SubscriptionId",
                schema: "billing",
                table: "SubscriptionFeatures",
                column: "SubscriptionId");
        }
    }
}
