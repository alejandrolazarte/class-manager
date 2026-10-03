using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BackfillPackSaleOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SELECT
                    [Purchases].[Id] AS [PurchaseId],
                    NEWID() AS [OrderId],
                    [Businesses].[NextOrderNumber] - 1
                        + ROW_NUMBER() OVER (PARTITION BY [Purchases].[TenantId] ORDER BY [Purchases].[PurchasedOn], [Purchases].[CreatedAt], [Purchases].[Id]) AS [Number]
                INTO #PackSalesWithoutOrder
                FROM [ClassPackPurchases] AS [Purchases]
                INNER JOIN [Businesses] ON [Businesses].[Id] = [Purchases].[TenantId]
                WHERE NOT EXISTS (SELECT 1 FROM [OrderLines] WHERE [OrderLines].[ClassPackPurchaseId] = [Purchases].[Id]);

                INSERT INTO [Orders] (
                    [Id], [TenantId], [Number], [ClientId], [Channel], [Status], [Delivery], [Method], [PaidOn], [Notes],
                    [CreatedAt], [CreatedByUserId], [PaymentRecordedByUserId])
                SELECT
                    [Sales].[OrderId], [Purchases].[TenantId], [Sales].[Number], [Purchases].[ClientId], 'Counter', 'Paid', 'Pickup',
                    [Purchases].[Method], [Purchases].[PurchasedOn], [Purchases].[Notes],
                    [Purchases].[CreatedAt], [Purchases].[RecordedByUserId], [Purchases].[RecordedByUserId]
                FROM #PackSalesWithoutOrder AS [Sales]
                INNER JOIN [ClassPackPurchases] AS [Purchases] ON [Purchases].[Id] = [Sales].[PurchaseId];

                INSERT INTO [OrderLines] (
                    [Id], [TenantId], [OrderId], [Kind], [ClassPackId], [ClassPackPurchaseId], [Name], [Quantity], [UnitPrice],
                    [RefundedQuantity], [RefundedAmount])
                SELECT
                    NEWID(), [Purchases].[TenantId], [Sales].[OrderId], 'ClassPack', [Purchases].[ClassPackId], [Purchases].[Id],
                    [Purchases].[Name], 1, [Purchases].[Price], 0, 0
                FROM #PackSalesWithoutOrder AS [Sales]
                INNER JOIN [ClassPackPurchases] AS [Purchases] ON [Purchases].[Id] = [Sales].[PurchaseId];

                UPDATE [Businesses]
                SET [NextOrderNumber] = ISNULL((SELECT MAX([Number]) FROM [Orders] WHERE [Orders].[TenantId] = [Businesses].[Id]), 0) + 1;

                DROP TABLE #PackSalesWithoutOrder;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
