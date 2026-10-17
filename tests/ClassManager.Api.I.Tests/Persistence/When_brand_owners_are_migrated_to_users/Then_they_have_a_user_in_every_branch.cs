using ClassManager.Infrastructure.Persistence;
using ClassManager.Security.Persistence;
using ClassManager.Tenancy.AspNetCore.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassManager.Api.I.Tests.Persistence.When_brand_owners_are_migrated_to_users;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_have_a_user_in_every_branch(ApiFixture fixture)
{
    private const string MigrationBeforeUsers = "20261017025643_UseDeletedAtForSoftDelete";

    [Fact]
    public async Task Then_they_have_a_user_in_every_branch_Run()
    {
        var connectionString = new SqlConnectionStringBuilder(fixture.ConnectionString)
        {
            InitialCatalog = $"BrandOwnerUsersMigration{Guid.NewGuid():N}",
        }.ConnectionString;
        await using var context = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options,
            new FixedTenantContext(Guid.Empty));
        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync(MigrationBeforeUsers);
        await using (var securityContext = new SecurityDbContext(
            new DbContextOptionsBuilder<SecurityDbContext>().UseSqlServer(connectionString, SecurityDbContext.ConfigureSqlServer).Options))
        {
            await securityContext.Database.MigrateAsync();
        }

        var organizationId = Guid.NewGuid();
        var businessWithMemberId = Guid.NewGuid();
        var businessWithoutMemberId = Guid.NewGuid();
        var brandOwnerAccountId = Guid.NewGuid();
        await context.Database.ExecuteSqlAsync(
            $"""
            INSERT INTO [identity].[AspNetUsers] (Id, FullName, Email, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
            VALUES ({brandOwnerAccountId}, N'Marta Ruiz', N'marta@example.com', 1, 0, 0, 0, 0);
            INSERT INTO Organizations (Id, CreatedAt, Name) VALUES ({organizationId}, SYSUTCDATETIME(), N'Estudio');
            INSERT INTO Businesses (Id, OrganizationId, Name, Slug, CountryCode, TimeZoneId, CurrencyCode, DefaultCountryCallingCode, NoticedAbsencesKeepStreak, CreatedAt)
            VALUES ({businessWithMemberId}, {organizationId}, N'Estudio Centro', {businessWithMemberId.ToString("N")}, N'AR', N'America/Argentina/Buenos_Aires', N'ARS', N'54', 1, SYSUTCDATETIME()),
                   ({businessWithoutMemberId}, {organizationId}, N'Estudio Norte', {businessWithoutMemberId.ToString("N")}, N'AR', N'America/Argentina/Buenos_Aires', N'ARS', N'54', 1, SYSUTCDATETIME());
            INSERT INTO OrganizationMembers (Id, OrganizationId, UserId, Role, IsDeleted)
            VALUES ({Guid.NewGuid()}, {organizationId}, {brandOwnerAccountId}, N'BrandOwner', 0);
            INSERT INTO BusinessMembers (Id, TenantId, UserId, Role, InstructorId, IsDeleted)
            VALUES ({Guid.NewGuid()}, {businessWithMemberId}, {brandOwnerAccountId}, N'BranchOwner', NULL, 0);
            """);

        await migrator.MigrateAsync();

        var users = await context.Users.IgnoreTenantFilter().AsNoTracking()
            .Where(user => user.AccountId == brandOwnerAccountId)
            .Select(user => new { user.TenantId, user.FullName })
            .ToListAsync();
        await context.Database.EnsureDeletedAsync();
        users.Select(user => (user.TenantId, user.FullName)).ShouldBe(
            [(businessWithMemberId, "Marta Ruiz"), (businessWithoutMemberId, "Marta Ruiz")],
            ignoreOrder: true);
    }
}
