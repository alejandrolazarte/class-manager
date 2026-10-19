using System.Globalization;
using Azure.Data.Tables;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_a_client_is_registered_through_the_api;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_audit_entry_hides_the_contact(ApiFixture fixture)
{
    private const string RegisterClientCommand = "RegisterClientCommand";
    private const string FullName = "Lucía Fernández Gómez";
    private const string PhoneNumber = "11 4321-8765";
    private const string PhoneDigits = "43218765";
    private const string PersonalMarker = "[personal]";

    private static readonly TimeSpan WaitLimit = TimeSpan.FromSeconds(30);

    [Fact]
    public async Task Then_its_audit_entry_hides_the_contact_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        await business.HttpClient.RegisterClientAsync(fullName: FullName, phoneNumber: PhoneNumber);

        var payload = await WaitForPayloadAsync(business.Business.Id);
        payload.ShouldContain(PersonalMarker);
        payload.ShouldNotContain(PhoneDigits);
        payload.ShouldNotContain(FullName);
    }

    private async Task<string> WaitForPayloadAsync(Guid tenantId)
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
                    if (row.GetString(nameof(CommandAuditEntry.Command)) == RegisterClientCommand)
                    {
                        return row.GetString(nameof(CommandAuditEntry.Payload));
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
