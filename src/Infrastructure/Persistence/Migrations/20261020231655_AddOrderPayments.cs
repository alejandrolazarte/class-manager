using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Method",
                table: "ClassPackPurchases",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16);

            migrationBuilder.CreateTable(
                name: "OrderPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    PaidOn = table.Column<DateOnly>(type: "date", nullable: false),
                    RecordedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPayments", x => x.Id);
                    table.CheckConstraint("CK_OrderPayments_IsDeleted_DeletedAt", "([IsDeleted] = 0 AND [DeletedAt] IS NULL) OR ([IsDeleted] = 1 AND [DeletedAt] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_OrderPayments_Businesses_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPayments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPayments_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPayments_Users_DeletedByUserId",
                        column: x => x.DeletedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPayments_Users_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderPayments_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_CreatedByUserId",
                table: "OrderPayments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_DeletedByUserId",
                table: "OrderPayments",
                column: "DeletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_OrderId",
                table: "OrderPayments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_RecordedByUserId",
                table: "OrderPayments",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_TenantId_OrderId",
                table: "OrderPayments",
                columns: new[] { "TenantId", "OrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_TenantId_PaidOn",
                table: "OrderPayments",
                columns: new[] { "TenantId", "PaidOn" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_UpdatedByUserId",
                table: "OrderPayments",
                column: "UpdatedByUserId");
            migrationBuilder.Sql(
                """
                INSERT INTO [OrderPayments] ([Id], [TenantId], [OrderId], [Amount], [Method], [PaidOn], [RecordedByUserId], [CreatedAt], [CreatedByUserId], [UpdatedAt], [UpdatedByUserId], [IsDeleted])
                SELECT NEWID(), [Orders].[TenantId], [Orders].[Id],
                    (SELECT SUM([OrderLines].[UnitPrice] * [OrderLines].[Quantity]) FROM [OrderLines] WHERE [OrderLines].[OrderId] = [Orders].[Id]),
                    [Orders].[Method], [Orders].[PaidOn], [Orders].[PaymentRecordedByUserId],
                    [Orders].[CreatedAt], [Orders].[PaymentRecordedByUserId], [Orders].[CreatedAt], [Orders].[PaymentRecordedByUserId], 0
                FROM [Orders]
                WHERE [Orders].[Status] IN ('Paid', 'Delivered') AND [Orders].[PaidOn] IS NOT NULL AND [Orders].[Method] IS NOT NULL;
                """);

            migrationBuilder.Sql(
                """
                UPDATE [Orders]
                SET [Status] = 'Delivered',
                    [DeliveredAt] = CAST([PaidOn] AS datetimeoffset),
                    [DeliveredByUserId] = [PaymentRecordedByUserId]
                WHERE [Status] = 'Paid'
                    AND NOT EXISTS (SELECT 1 FROM [OrderLines] WHERE [OrderLines].[OrderId] = [Orders].[Id] AND [OrderLines].[Kind] = 'Product');
                """);

            migrationBuilder.Sql("UPDATE [Orders] SET [Status] = 'Pending' WHERE [Status] IN ('Requested', 'Paid');");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_PaymentRecordedByUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentRecordedByUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaidOn",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentRecordedByUserId",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "Orders",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "PaidOn",
                table: "Orders",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentRecordedByUserId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [Orders]
                SET [Method] = [LastPayment].[Method],
                    [PaidOn] = [LastPayment].[PaidOn],
                    [PaymentRecordedByUserId] = [LastPayment].[RecordedByUserId]
                FROM [Orders]
                CROSS APPLY (
                    SELECT TOP 1 [OrderPayments].[Method], [OrderPayments].[PaidOn], [OrderPayments].[RecordedByUserId]
                    FROM [OrderPayments]
                    WHERE [OrderPayments].[OrderId] = [Orders].[Id] AND [OrderPayments].[IsDeleted] = 0
                    ORDER BY [OrderPayments].[PaidOn] DESC, [OrderPayments].[CreatedAt] DESC
                ) AS [LastPayment];
                """);

            migrationBuilder.Sql("UPDATE [Orders] SET [Status] = CASE WHEN [PaidOn] IS NULL THEN 'Requested' ELSE 'Paid' END WHERE [Status] = 'Pending';");

            migrationBuilder.Sql("UPDATE [ClassPackPurchases] SET [Method] = 'Other' WHERE [Method] IS NULL;");

            migrationBuilder.AlterColumn<string>(
                name: "Method",
                table: "ClassPackPurchases",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentRecordedByUserId",
                table: "Orders",
                column: "PaymentRecordedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_PaymentRecordedByUserId",
                table: "Orders",
                column: "PaymentRecordedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropTable(
                name: "OrderPayments");
        }
    }
}
