using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RecordBillingPlanOfRegisteredClients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO [ClientBillingPlanChanges] ([TenantId], [ClientId], [EffectiveFrom], [Kind], [CustomFee], [CreatedAt], [UpdatedAt], [IsDeleted])
                SELECT [client].[TenantId], [client].[Id], [timeline].[EffectiveFrom], [timeline].[Kind], NULL, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), 0
                FROM [Clients] AS [client]
                INNER JOIN (
                    SELECT [TenantId], [EffectiveFrom], [Kind], LAG([Kind]) OVER (PARTITION BY [TenantId] ORDER BY [EffectiveFrom]) AS [PreviousKind]
                    FROM (
                        SELECT [TenantId], [EffectiveFrom], CASE WHEN [BillingMode] = 'ClassPacks' THEN 'ClassPacks' ELSE 'BusinessFee' END AS [Kind]
                        FROM [DefaultMonthlyFeeChanges]
                        WHERE [IsDeleted] = 0
                    ) AS [businessChange]
                ) AS [timeline] ON [timeline].[TenantId] = [client].[TenantId]
                WHERE ([timeline].[PreviousKind] IS NULL OR [timeline].[PreviousKind] <> [timeline].[Kind])
                    AND NOT EXISTS (
                        SELECT 1
                        FROM [ClientBillingPlanChanges] AS [planChange]
                        WHERE [planChange].[ClientId] = [client].[Id] AND [planChange].[IsDeleted] = 0)
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
