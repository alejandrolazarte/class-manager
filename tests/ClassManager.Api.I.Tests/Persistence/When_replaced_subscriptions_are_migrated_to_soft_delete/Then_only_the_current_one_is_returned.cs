using ClassManager.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassManager.Api.I.Tests.Persistence.When_replaced_subscriptions_are_migrated_to_soft_delete;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_the_current_one_is_returned(ApiFixture fixture)
{
    private const string MigrationBeforeSoftDelete = "20261017011633_AddContactAdultConfirmation";
    private const string FreePlanCode = "free";
    private const string Currency = "EUR";
    private const string OrganizationName = "Migration check";

    [Fact]
    public async Task Then_only_the_current_one_is_returned_Run()
    {
        var connectionString = new SqlConnectionStringBuilder(fixture.ConnectionString)
        {
            InitialCatalog = $"MigrationCheck{Guid.NewGuid():N}",
        }.ConnectionString;
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options;
        await using var context = new AppDbContext(options, new FixedTenantContext(Guid.Empty));
        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync(MigrationBeforeSoftDelete);
        var subscriberId = Guid.NewGuid();
        var replacedSubscriptionId = Guid.NewGuid();
        var currentSubscriptionId = Guid.NewGuid();
        var replacedOn = BusinessApiFactory.Now.AddDays(-10);
        await context.Database.ExecuteSqlAsync(
            $"""
            INSERT INTO Organizations (Id, CreatedAt, Name)
            VALUES ({subscriberId}, {replacedOn.AddDays(-30)}, {OrganizationName});
            INSERT INTO billing.Subscriptions (Id, SubscriberId, PlanCode, Price, Currency, Note, CreatedOn, DeletedOn, ExpiredOn)
            VALUES ({replacedSubscriptionId}, {subscriberId}, {FreePlanCode}, 0, {Currency}, NULL, {replacedOn.AddDays(-30)}, {replacedOn}, NULL),
                   ({currentSubscriptionId}, {subscriberId}, {FreePlanCode}, 0, {Currency}, NULL, {replacedOn}, NULL, NULL)
            """);

        await migrator.MigrateAsync();

        var subscriptionIds = await context.Subscriptions
            .Where(subscription => subscription.SubscriberId == subscriberId)
            .Select(subscription => subscription.Id)
            .ToListAsync();
        var storedRows = await context.Database
            .SqlQuery<int>($"SELECT COUNT(*) AS [Value] FROM billing.Subscriptions WHERE SubscriberId = {subscriberId}")
            .SingleAsync();
        await context.Database.EnsureDeletedAsync();
        subscriptionIds.ShouldBe([currentSubscriptionId]);
        storedRows.ShouldBe(2);
    }
}
