namespace ClassManager.Infrastructure.Persistence.Migrations;

internal static class WhoDidWhatBackfill
{
    private static readonly (string Table, string Column)[] BranchColumns =
    [
        ("Announcements", "DeletedByUserId"),
        ("BusinessMembers", "DeletedByUserId"),
        ("ClassFeedbacks", "DeletedByUserId"),
        ("ClassPackPurchases", "DeletedByUserId"),
        ("ClassPackPurchases", "RecordedByUserId"),
        ("ClientBillingPlanChanges", "DeletedByUserId"),
        ("ClientInvitations", "ContactAdultConfirmedByUserId"),
        ("ClientInvitations", "InvitedByUserId"),
        ("Clients", "RegisteredByUserId"),
        ("CustomRoles", "DeletedByUserId"),
        ("DefaultMonthlyFeeChanges", "DeletedByUserId"),
        ("MeasurementDefinitions", "DeletedByUserId"),
        ("MeasurementRecords", "DeletedByUserId"),
        ("MeasurementRecords", "RecordedByUserId"),
        ("MemberInvitations", "InvitedByUserId"),
        ("Orders", "CancelledByUserId"),
        ("Orders", "CreatedByUserId"),
        ("Orders", "DeliveredByUserId"),
        ("Orders", "PaymentRecordedByUserId"),
        ("Payments", "DeletedByUserId"),
        ("Payments", "RecordedByUserId"),
        ("StockMovements", "RecordedByUserId"),
        ("Users", "DeletedByUserId"),
    ];

    private static readonly (string Table, string Column)[] ColumnsOutsideBranches =
    [
        ("[OrganizationMembers]", "DeletedByUserId"),
        ("[billing].[Subscriptions]", "DeletedByUserId"),
        ("[billing].[SubscriptionFeatures]", "DeletedByUserId"),
    ];

    public static string AccountsToUsersOfTheBranch { get; } = string.Join(
        Environment.NewLine,
        BranchColumns.Select(column => $"""
            UPDATE [row]
            SET [{column.Column}] = (
                SELECT TOP (1) [user].[Id]
                FROM [Users] AS [user]
                WHERE [user].[TenantId] = [row].[TenantId] AND [user].[AccountId] = [row].[{column.Column}]
                ORDER BY [user].[IsDeleted], [user].[Id])
            FROM [{column.Table}] AS [row]
            WHERE [row].[{column.Column}] IS NOT NULL;
            """));

    public static string AccountsOutsideBranches { get; } = string.Join(
        Environment.NewLine,
        ColumnsOutsideBranches.Select(column => $"""
            UPDATE {column.Table}
            SET [{column.Column}] = NULL
            WHERE [{column.Column}] IS NOT NULL
                AND NOT EXISTS (SELECT 1 FROM [Users] AS [user] WHERE [user].[Id] = {column.Table}.[{column.Column}]);
            """));
}
