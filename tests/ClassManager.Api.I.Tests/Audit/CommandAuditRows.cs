using System.Globalization;
using Azure.Data.Tables;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit;

internal static class CommandAuditRows
{
    private static readonly TimeSpan WaitLimit = TimeSpan.FromSeconds(30);

    public static async Task<TableEntity> WaitForAsync(ApiFixture fixture, Guid tenantId, string command)
    {
        var table = new TableServiceClient(fixture.StorageConnectionString)
            .GetTableClient("Commands" + BusinessApiFactory.Now.UtcDateTime.ToString("yyyyMM", CultureInfo.InvariantCulture));
        var waitUntil = DateTime.UtcNow + WaitLimit;
        while (DateTime.UtcNow < waitUntil)
        {
            try
            {
                await foreach (var row in table.QueryAsync<TableEntity>(stored => stored.PartitionKey == tenantId.ToString()))
                {
                    if (row.GetString(nameof(CommandAuditEntry.Command)) == command)
                    {
                        return row;
                    }
                }
            }
            catch (Azure.RequestFailedException)
            {
            }

            await Task.Delay(TimeSpan.FromMilliseconds(200));
        }

        throw new TimeoutException("The command audit entry was not written.");
    }
}
