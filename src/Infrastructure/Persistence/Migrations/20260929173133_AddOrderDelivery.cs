using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Delivery",
                table: "Orders",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "Pickup");

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryClassGroupId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReadyAt",
                table: "Orders",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryClassGroupId",
                table: "Orders",
                column: "DeliveryClassGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ClassGroups_DeliveryClassGroupId",
                table: "Orders",
                column: "DeliveryClassGroupId",
                principalTable: "ClassGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ClassGroups_DeliveryClassGroupId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryClassGroupId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Delivery",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryClassGroupId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReadyAt",
                table: "Orders");
        }
    }
}
