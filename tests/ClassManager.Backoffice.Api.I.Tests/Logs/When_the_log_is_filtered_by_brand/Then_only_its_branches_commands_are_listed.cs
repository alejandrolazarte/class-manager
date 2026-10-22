using Azure.Data.Tables;
using ClassManager.Infrastructure.Audit;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Api.I.Tests.Logs.When_the_log_is_filtered_by_brand;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_its_branches_commands_are_listed(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_only_its_branches_commands_are_listed_Run()
    {
        var day = new DateOnly(2025, 4, 2);
        var occurredAt = new DateTimeOffset(day.ToDateTime(new TimeOnly(10, 0)), TimeSpan.Zero);
        var brand = await fixture.SeedBrandAsync(
            $"Marca {TestIds.Next()}",
            [0, 0],
            brandId => [Subscription.Start(brandId, "pro", 29m, "EUR", null, BackofficeApiFactory.Now)]);
        await new TableCommandAuditStore(new TableServiceClient(fixture.StorageConnectionString)).WriteAsync(
            [
                LoggedCommand("FirstBranchCommand", occurredAt, brand.BranchIds[0]),
                LoggedCommand("SecondBranchCommand", occurredAt.AddMinutes(1), brand.BranchIds[1]),
                LoggedCommand("OtherBrandCommand", occurredAt, TestIds.Next()),
            ],
            CancellationToken.None);
        using var client = fixture.ApiFactory.CreateHttpsClient();
        var accessToken = await client.OpenSessionAsync(await fixture.SeedOperatorAsync());

        var log = await client.GetCommandLogAsync(accessToken, day, brand.Id);

        log.Entries.Select(entry => (entry.Command, entry.BrandId))
            .ShouldBe([("SecondBranchCommand", (long?)brand.Id), ("FirstBranchCommand", brand.Id)]);
    }

    private static CommandAuditEntry LoggedCommand(string command, DateTimeOffset occurredAt, long tenantId) =>
        new(occurredAt, tenantId, command, null, null, CommandOutcome.Succeeded, null, null, 5, null, "{}", false);
}
