using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalSubscriptionId",
                schema: "billing",
                table: "Subscriptions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                schema: "billing",
                table: "Subscriptions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProcessedBillingEvents",
                schema: "billing",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedBillingEvents", x => x.EventId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ExternalSubscriptionId",
                schema: "billing",
                table: "Subscriptions",
                column: "ExternalSubscriptionId",
                unique: true,
                filter: "[IsDeleted] = 0 AND [ExternalSubscriptionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessedBillingEvents",
                schema: "billing");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_ExternalSubscriptionId",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "ExternalSubscriptionId",
                schema: "billing",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                schema: "billing",
                table: "Subscriptions");
        }
    }
}
