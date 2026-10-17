using ClassManager.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassManager.Api.I.Tests.Persistence.When_people_copies_are_removed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_repeated_emails_stop_the_migration(ApiFixture fixture)
{
    private const string MigrationBeforeRemovingCopies = "20261017120142_PointWhoDidWhatToUsers";
    private const string RepeatedEmailsScript = "list-repeated-emails.sql";

    [Fact]
    public async Task Then_repeated_emails_stop_the_migration_Run()
    {
        var connectionString = new SqlConnectionStringBuilder(fixture.ConnectionString)
        {
            InitialCatalog = $"RepeatedEmailsMigration{Guid.NewGuid():N}",
        }.ConnectionString;
        await using var context = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options,
            new FixedTenantContext(Guid.Empty));
        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync(MigrationBeforeRemovingCopies);
        var organizationId = Guid.NewGuid();
        var businessId = Guid.NewGuid();
        await context.Database.ExecuteSqlAsync(
            $"""
            INSERT INTO Organizations (Id, CreatedAt, Name) VALUES ({organizationId}, SYSUTCDATETIME(), N'Estudio');
            INSERT INTO Businesses (Id, OrganizationId, Name, Slug, CountryCode, TimeZoneId, CurrencyCode, DefaultCountryCallingCode, NoticedAbsencesKeepStreak, CreatedAt)
            VALUES ({businessId}, {organizationId}, N'Estudio', {businessId.ToString("N")}, N'AR', N'America/Argentina/Buenos_Aires', N'ARS', N'54', 1, SYSUTCDATETIME());
            INSERT INTO Users (Id, TenantId, FullName, Email, CreatedAt, IsDeleted)
            VALUES ({Guid.NewGuid()}, {businessId}, N'Ana Pérez', N'ana@example.com', SYSUTCDATETIME(), 0),
                   ({Guid.NewGuid()}, {businessId}, N'Ana María Pérez', N'ANA@example.com', SYSUTCDATETIME(), 0);
            """);

        var migration = await Should.ThrowAsync<SqlException>(() => migrator.MigrateAsync());

        await context.Database.EnsureDeletedAsync();
        migration.Message.ShouldContain(RepeatedEmailsScript);
    }
}
