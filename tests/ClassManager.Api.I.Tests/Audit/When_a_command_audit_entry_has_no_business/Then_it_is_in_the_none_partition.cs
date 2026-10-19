using Azure.Data.Tables;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_a_command_audit_entry_has_no_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_in_the_none_partition(ApiFixture fixture)
{
    private const string TableOfTheMonth = "Commands202609";

    [Fact]
    public async Task Then_it_is_in_the_none_partition_Run()
    {
        var serviceClient = new TableServiceClient(fixture.StorageConnectionString);
        var entry = CommandAuditEntries.Succeeded(tenantId: null, new DateTimeOffset(2026, 9, 30, 23, 59, 0, TimeSpan.Zero));

        await new TableCommandAuditStore(serviceClient).WriteAsync([entry], CancellationToken.None);

        var userIds = await serviceClient.GetTableClient(TableOfTheMonth)
            .QueryAsync<TableEntity>(stored => stored.PartitionKey == TableCommandAuditStore.NoBusinessPartition)
            .Select(row => row.GetGuid(nameof(CommandAuditEntry.UserId)))
            .ToListAsync();
        userIds.ShouldContain(entry.UserId);
    }
}
