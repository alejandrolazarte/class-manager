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

    [Fact]
    public async Task Then_its_audit_entry_hides_the_contact_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        await business.HttpClient.RegisterClientAsync(fullName: FullName, phoneNumber: PhoneNumber);

        var row = await CommandAuditRows.WaitForAsync(fixture, business.Business.Id, RegisterClientCommand);
        var payload = row.GetString(nameof(CommandAuditEntry.Payload));
        payload.ShouldContain(PersonalMarker);
        payload.ShouldNotContain(PhoneDigits);
        payload.ShouldNotContain(FullName);
    }
}
