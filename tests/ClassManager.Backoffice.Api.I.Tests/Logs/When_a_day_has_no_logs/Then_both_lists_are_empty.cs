using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Api.I.Tests.Logs.When_a_day_has_no_logs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_both_lists_are_empty(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_both_lists_are_empty_Run()
    {
        var day = new DateOnly(2024, 1, 9);
        using var client = fixture.ApiFactory.CreateHttpsClient();
        var accessToken = await client.OpenSessionAsync(await fixture.SeedOperatorAsync(BusinessRole.PlatformReader));

        var commands = await client.GetCommandLogAsync(accessToken, day);
        var events = await client.GetSecurityEventsAsync(accessToken, day);

        (commands.Entries.Count, events.Events.Count).ShouldBe((0, 0));
    }
}
