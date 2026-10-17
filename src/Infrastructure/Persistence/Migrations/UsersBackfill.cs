namespace ClassManager.Infrastructure.Persistence.Migrations;

internal static class UsersBackfill
{
    public const string Clients = """
        INSERT INTO [Users] ([Id], [TenantId], [FullName], [Email], [PhoneNumber], [CreatedAt], [IsDeleted])
        SELECT [Id], [TenantId], [FullName], [Email], [PhoneNumber], [CreatedAt], 0
        FROM [Clients];

        UPDATE [Clients] SET [UserId] = [Id];
        """;

    public const string AttendingContacts = """
        UPDATE [student]
        SET [UserId] = [client].[UserId]
        FROM [Students] AS [student]
        JOIN [Clients] AS [client] ON [client].[Id] = [student].[ClientId]
        WHERE LTRIM(RTRIM([client].[FullName])) = LTRIM(RTRIM([student].[FullName]));

        UPDATE [user]
        SET [BirthDate] = [student].[BirthDate]
        FROM [Users] AS [user]
        JOIN [Students] AS [student] ON [student].[UserId] = [user].[Id]
        WHERE [student].[BirthDate] IS NOT NULL;
        """;

    public const string Students = """
        INSERT INTO [Users] ([Id], [TenantId], [FullName], [Email], [BirthDate], [CreatedAt], [IsDeleted])
        SELECT [Id], [TenantId], [FullName], [Email], [BirthDate], [CreatedAt], 0
        FROM [Students]
        WHERE [UserId] IS NULL;

        UPDATE [Students] SET [UserId] = [Id] WHERE [UserId] IS NULL;
        """;

    public const string Instructors = """
        INSERT INTO [Users] ([Id], [TenantId], [FullName], [Email], [CreatedAt], [IsDeleted])
        SELECT [Id], [TenantId], [FullName], [Email], SYSUTCDATETIME(), 0
        FROM [Instructors];

        UPDATE [Instructors] SET [UserId] = [Id];
        """;

    public const string MembersWhoTeach = """
        UPDATE [member]
        SET [UserId] = [instructor].[UserId]
        FROM [BusinessMembers] AS [member]
        JOIN [Instructors] AS [instructor] ON [instructor].[Id] = [member].[InstructorId];
        """;

    public const string ClientAccounts = """
        UPDATE [clientAccount]
        SET [UserId] = COALESCE([student].[UserId], [client].[UserId])
        FROM [ClientAccounts] AS [clientAccount]
        JOIN [Clients] AS [client] ON [client].[Id] = [clientAccount].[ClientId]
        LEFT JOIN [Students] AS [student] ON [student].[Id] = [clientAccount].[StudentId];

        WITH [firstAccount] AS (
            SELECT [UserId], [AccountId],
                ROW_NUMBER() OVER (PARTITION BY [UserId] ORDER BY [CreatedAt], [Id]) AS [Position]
            FROM [ClientAccounts]
        )
        UPDATE [user]
        SET [AccountId] = [firstAccount].[AccountId]
        FROM [Users] AS [user]
        JOIN [firstAccount] ON [firstAccount].[UserId] = [user].[Id]
        WHERE [firstAccount].[Position] = 1;
        """;

    public const string AccountsOfMembersWhoTeach = """
        WITH [teachingMember] AS (
            SELECT [member].[UserId], [member].[AccountId], [member].[TenantId],
                ROW_NUMBER() OVER (PARTITION BY [member].[TenantId], [member].[AccountId] ORDER BY [member].[IsDeleted], [member].[Id]) AS [Position]
            FROM [BusinessMembers] AS [member]
            WHERE [member].[UserId] IS NOT NULL
        )
        UPDATE [user]
        SET [AccountId] = [teachingMember].[AccountId]
        FROM [Users] AS [user]
        JOIN [teachingMember] ON [teachingMember].[UserId] = [user].[Id]
        WHERE [teachingMember].[Position] = 1
            AND [user].[AccountId] IS NULL
            AND NOT EXISTS (
                SELECT 1 FROM [Users] AS [other]
                WHERE [other].[TenantId] = [teachingMember].[TenantId] AND [other].[AccountId] = [teachingMember].[AccountId]);
        """;

    public const string MembersOfLinkedAccounts = """
        UPDATE [member]
        SET [UserId] = [user].[Id]
        FROM [BusinessMembers] AS [member]
        JOIN [Users] AS [user] ON [user].[TenantId] = [member].[TenantId] AND [user].[AccountId] = [member].[AccountId]
        WHERE [member].[UserId] IS NULL;
        """;

    public const string OtherMembers = """
        IF OBJECT_ID(N'[identity].[AspNetUsers]') IS NOT NULL
        EXEC(N'
            WITH [unlinkedAccount] AS (
                SELECT [member].[Id], [member].[TenantId], [member].[AccountId],
                    ROW_NUMBER() OVER (PARTITION BY [member].[TenantId], [member].[AccountId] ORDER BY [member].[IsDeleted], [member].[Id]) AS [Position]
                FROM [BusinessMembers] AS [member]
                WHERE [member].[UserId] IS NULL
            )
            INSERT INTO [Users] ([Id], [TenantId], [FullName], [Email], [AccountId], [CreatedAt], [IsDeleted])
            SELECT [unlinkedAccount].[Id], [unlinkedAccount].[TenantId], [account].[FullName], [account].[Email], [unlinkedAccount].[AccountId], SYSUTCDATETIME(), 0
            FROM [unlinkedAccount]
            JOIN [identity].[AspNetUsers] AS [account] ON [account].[Id] = [unlinkedAccount].[AccountId]
            WHERE [unlinkedAccount].[Position] = 1;

            UPDATE [member]
            SET [UserId] = [user].[Id]
            FROM [BusinessMembers] AS [member]
            JOIN [Users] AS [user] ON [user].[TenantId] = [member].[TenantId] AND [user].[AccountId] = [member].[AccountId]
            WHERE [member].[UserId] IS NULL;
        ');
        """;

    public const string BrandOwnersInEveryBranch = """
        IF OBJECT_ID(N'[identity].[AspNetUsers]') IS NOT NULL
        EXEC(N'
            INSERT INTO [Users] ([Id], [TenantId], [FullName], [Email], [AccountId], [CreatedAt], [IsDeleted])
            SELECT NEWID(), [business].[Id], [account].[FullName], [account].[Email], [brandOwner].[UserId], SYSUTCDATETIME(), 0
            FROM [OrganizationMembers] AS [brandOwner]
            JOIN [Businesses] AS [business] ON [business].[OrganizationId] = [brandOwner].[OrganizationId]
            JOIN [identity].[AspNetUsers] AS [account] ON [account].[Id] = [brandOwner].[UserId]
            WHERE [brandOwner].[Role] = N''BrandOwner''
                AND [brandOwner].[IsDeleted] = 0
                AND NOT EXISTS (
                    SELECT 1 FROM [Users] AS [user]
                    WHERE [user].[TenantId] = [business].[Id] AND [user].[AccountId] = [brandOwner].[UserId] AND [user].[IsDeleted] = 0);
        ');
        """;
}
