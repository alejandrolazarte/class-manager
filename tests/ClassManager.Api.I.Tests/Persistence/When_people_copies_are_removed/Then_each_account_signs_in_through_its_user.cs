using ClassManager.Infrastructure.Persistence;
using ClassManager.Security.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassManager.Api.I.Tests.Persistence.When_people_copies_are_removed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_account_signs_in_through_its_user(ApiFixture fixture)
{
    private const string MigrationBeforeRemovingCopies = "20261017120142_PointWhoDidWhatToUsers";

    [Fact]
    public async Task Then_each_account_signs_in_through_its_user_Run()
    {
        var connectionString = new SqlConnectionStringBuilder(fixture.ConnectionString)
        {
            InitialCatalog = $"PeopleCopiesMigration{Guid.NewGuid():N}",
        }.ConnectionString;
        await using var context = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options,
            new FixedTenantContext(Guid.Empty));
        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync(MigrationBeforeRemovingCopies);
        await using (var securityContext = new SecurityDbContext(
            new DbContextOptionsBuilder<SecurityDbContext>().UseSqlServer(connectionString, SecurityDbContext.ConfigureSqlServer).Options))
        {
            await securityContext.Database.MigrateAsync();
        }

        var rows = new MigrationRows();
        await context.Database.ExecuteSqlAsync(
            $"""
            INSERT INTO [identity].[AspNetUsers] (Id, FullName, Email, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
            VALUES ({rows.OwnerAccountId}, N'Marta Ruiz', N'marta@example.com', 1, 0, 0, 0, 0);
            INSERT INTO Organizations (Id, CreatedAt, Name) VALUES ({rows.OrganizationId}, SYSUTCDATETIME(), N'Estudio');
            INSERT INTO Businesses (Id, OrganizationId, Name, Slug, CountryCode, TimeZoneId, CurrencyCode, DefaultCountryCallingCode, NoticedAbsencesKeepStreak, CreatedAt)
            VALUES ({rows.BusinessId}, {rows.OrganizationId}, N'Estudio', {rows.BusinessId.ToString("N")}, N'AR', N'America/Argentina/Buenos_Aires', N'ARS', N'54', 1, SYSUTCDATETIME());
            INSERT INTO Users (Id, TenantId, FullName, Email, PhoneNumber, BirthDate, CreatedAt, IsDeleted)
            VALUES ({rows.InstructorUserId}, {rows.BusinessId}, N'Laura Gómez', N'laura@example.com', NULL, NULL, SYSUTCDATETIME(), 0),
                   ({rows.ClientUserId}, {rows.BusinessId}, N'Ana Pérez', N'ana@example.com', N'+5491122334455', NULL, SYSUTCDATETIME(), 0),
                   ({rows.ChildUserId}, {rows.BusinessId}, N'Tomás Pérez', NULL, NULL, '2014-03-14', SYSUTCDATETIME(), 0);
            INSERT INTO Instructors (Id, TenantId, FullName, Email, IsActive, UserId)
            VALUES ({rows.InstructorId}, {rows.BusinessId}, N'Laura Gómez', N'laura@example.com', 1, {rows.InstructorUserId});
            INSERT INTO Clients (Id, TenantId, FullName, PhoneNumber, Email, CreatedAt, UserId)
            VALUES ({rows.ClientId}, {rows.BusinessId}, N'Ana Pérez', N'+5491122334455', N'ana@example.com', SYSUTCDATETIME(), {rows.ClientUserId});
            INSERT INTO Students (Id, TenantId, ClientId, FullName, BirthDate, CreatedAt, UserId)
            VALUES ({rows.ChildId}, {rows.BusinessId}, {rows.ClientId}, N'Tomás Pérez', '2014-03-14', SYSUTCDATETIME(), {rows.ChildUserId});
            INSERT INTO BusinessMembers (Id, TenantId, AccountId, UserId, Role, InstructorId, IsDeleted)
            VALUES ({rows.InstructorMemberId}, {rows.BusinessId}, {rows.InstructorAccountId}, {rows.InstructorUserId}, N'Instructor', {rows.InstructorId}, 0),
                   ({rows.OwnerMemberId}, {rows.BusinessId}, {rows.OwnerAccountId}, NULL, N'BranchOwner', NULL, 0);
            INSERT INTO ClientAccounts (Id, TenantId, ClientId, StudentId, AccountId, UserId, CreatedAt)
            VALUES ({rows.ChildClientAccountId}, {rows.BusinessId}, {rows.ClientId}, {rows.ChildId}, {rows.ChildAccountId}, {rows.ChildUserId}, SYSUTCDATETIME());
            """);

        await migrator.MigrateAsync();

        await using var tenantDbContext = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options,
            new FixedTenantContext(rows.BusinessId));
        var members = await tenantDbContext.BusinessMembers.AsNoTracking().Include(member => member.User).ToDictionaryAsync(member => member.Id);
        var childClientAccount = await tenantDbContext.ClientAccounts.AsNoTracking().Include(account => account.User).SingleAsync();
        var client = await tenantDbContext.Clients.AsNoTracking().Include(row => row.User).SingleAsync();
        await tenantDbContext.Database.EnsureDeletedAsync();
        var signIns = new[]
        {
            ("Laura", members[rows.InstructorMemberId].User.FullName, members[rows.InstructorMemberId].User.AccountId),
            ("Marta", members[rows.OwnerMemberId].User.FullName, members[rows.OwnerMemberId].User.AccountId),
            ("Tomás", childClientAccount.User.FullName, childClientAccount.User.AccountId),
            ("Ana", client.User.FullName, client.User.AccountId),
        };

        signIns.ShouldBe(
        [
            ("Laura", "Laura Gómez", (Guid?)rows.InstructorAccountId),
            ("Marta", "Marta Ruiz", rows.OwnerAccountId),
            ("Tomás", "Tomás Pérez", rows.ChildAccountId),
            ("Ana", "Ana Pérez", null),
        ]);
    }

    private sealed class MigrationRows
    {
        public Guid OrganizationId { get; } = Guid.NewGuid();
        public Guid BusinessId { get; } = Guid.NewGuid();
        public Guid InstructorId { get; } = Guid.NewGuid();
        public Guid ClientId { get; } = Guid.NewGuid();
        public Guid ChildId { get; } = Guid.NewGuid();
        public Guid InstructorUserId { get; } = Guid.NewGuid();
        public Guid ClientUserId { get; } = Guid.NewGuid();
        public Guid ChildUserId { get; } = Guid.NewGuid();
        public Guid InstructorMemberId { get; } = Guid.NewGuid();
        public Guid OwnerMemberId { get; } = Guid.NewGuid();
        public Guid ChildClientAccountId { get; } = Guid.NewGuid();
        public Guid InstructorAccountId { get; } = Guid.NewGuid();
        public Guid OwnerAccountId { get; } = Guid.NewGuid();
        public Guid ChildAccountId { get; } = Guid.NewGuid();
    }
}
