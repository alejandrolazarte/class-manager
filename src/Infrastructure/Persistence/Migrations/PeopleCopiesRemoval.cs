namespace ClassManager.Infrastructure.Persistence.Migrations;

internal static class PeopleCopiesRemoval
{
    public const string UsersOfPeopleWithoutOne = """
        INSERT INTO [Users] ([Id], [TenantId], [FullName], [Email], [PhoneNumber], [CreatedAt], [IsDeleted])
        SELECT [Id], [TenantId], [FullName], [Email], [PhoneNumber], [CreatedAt], 0
        FROM [Clients]
        WHERE [UserId] IS NULL;

        UPDATE [Clients] SET [UserId] = [Id] WHERE [UserId] IS NULL;

        INSERT INTO [Users] ([Id], [TenantId], [FullName], [Email], [BirthDate], [CreatedAt], [IsDeleted])
        SELECT [Id], [TenantId], [FullName], [Email], [BirthDate], [CreatedAt], 0
        FROM [Students]
        WHERE [UserId] IS NULL;

        UPDATE [Students] SET [UserId] = [Id] WHERE [UserId] IS NULL;

        INSERT INTO [Users] ([Id], [TenantId], [FullName], [Email], [CreatedAt], [IsDeleted])
        SELECT [Id], [TenantId], [FullName], [Email], SYSUTCDATETIME(), 0
        FROM [Instructors]
        WHERE [UserId] IS NULL;

        UPDATE [Instructors] SET [UserId] = [Id] WHERE [UserId] IS NULL;
        """;

    public const string RepeatedEmailsGuard = """
        IF EXISTS (
            SELECT 1
            FROM [Users]
            WHERE [Email] IS NOT NULL AND [IsDeleted] = 0
            GROUP BY [TenantId], [Email]
            HAVING COUNT(*) > 1)
            THROW 51000, 'A business has more than one person with the same email. List them with scripts/list-repeated-emails.sql and fix them in the app before migrating.', 1;
        """;

    private static readonly string[] TablesThatSignIn = ["BusinessMembers", "ClientAccounts"];

    public static string AccountsOnTheirUsers { get; } = string.Join(
        Environment.NewLine,
        TablesThatSignIn.Select(table => LinkFreeUsers(table) + PointToTheUserOfTheAccount(table)));

    public static string UsersOfAccountsWithoutOne { get; } = string.Join(
        Environment.NewLine,
        TablesThatSignIn.Select(table => AddUsersNamedAfterTheAccount(table) + PointToTheUserOfTheAccount(table)));

    private static string LinkFreeUsers(string table) => $"""
        WITH [candidate] AS (
            SELECT [row].[UserId], [row].[AccountId], [row].[TenantId],
                ROW_NUMBER() OVER (PARTITION BY [row].[TenantId], [row].[AccountId] ORDER BY [row].[Id]) AS [AccountPosition],
                ROW_NUMBER() OVER (PARTITION BY [row].[UserId] ORDER BY [row].[Id]) AS [UserPosition]
            FROM [{table}] AS [row]
            JOIN [Users] AS [user] ON [user].[Id] = [row].[UserId]
            WHERE [user].[AccountId] IS NULL
        )
        UPDATE [user]
        SET [AccountId] = [candidate].[AccountId]
        FROM [Users] AS [user]
        JOIN [candidate] ON [candidate].[UserId] = [user].[Id]
        WHERE [candidate].[AccountPosition] = 1
            AND [candidate].[UserPosition] = 1
            AND NOT EXISTS (
                SELECT 1 FROM [Users] AS [other]
                WHERE [other].[TenantId] = [candidate].[TenantId] AND [other].[AccountId] = [candidate].[AccountId] AND [other].[IsDeleted] = 0);

        """;

    private static string PointToTheUserOfTheAccount(string table) => $"""
        UPDATE [row]
        SET [UserId] = [user].[Id]
        FROM [{table}] AS [row]
        JOIN [Users] AS [user] ON [user].[TenantId] = [row].[TenantId] AND [user].[AccountId] = [row].[AccountId] AND [user].[IsDeleted] = 0
        WHERE [row].[UserId] IS NULL OR [row].[UserId] <> [user].[Id];

        """;

    private static string AddUsersNamedAfterTheAccount(string table) => $"""
        IF OBJECT_ID(N'[identity].[AspNetUsers]') IS NOT NULL
        EXEC(N'
            WITH [unlinkedAccount] AS (
                SELECT [row].[TenantId], [row].[AccountId],
                    ROW_NUMBER() OVER (PARTITION BY [row].[TenantId], [row].[AccountId] ORDER BY [row].[Id]) AS [Position]
                FROM [{table}] AS [row]
                WHERE NOT EXISTS (
                    SELECT 1 FROM [Users] AS [user]
                    WHERE [user].[TenantId] = [row].[TenantId] AND [user].[AccountId] = [row].[AccountId] AND [user].[IsDeleted] = 0)
            )
            INSERT INTO [Users] ([Id], [TenantId], [FullName], [Email], [AccountId], [CreatedAt], [IsDeleted])
            SELECT NEWID(), [unlinkedAccount].[TenantId], [account].[FullName],
                CASE WHEN EXISTS (
                    SELECT 1 FROM [Users] AS [other]
                    WHERE [other].[TenantId] = [unlinkedAccount].[TenantId] AND [other].[Email] = [account].[Email] AND [other].[IsDeleted] = 0)
                    THEN NULL ELSE [account].[Email] END,
                [unlinkedAccount].[AccountId], SYSUTCDATETIME(), 0
            FROM [unlinkedAccount]
            JOIN [identity].[AspNetUsers] AS [account] ON [account].[Id] = [unlinkedAccount].[AccountId]
            WHERE [unlinkedAccount].[Position] = 1;
        ');

        """;
}
