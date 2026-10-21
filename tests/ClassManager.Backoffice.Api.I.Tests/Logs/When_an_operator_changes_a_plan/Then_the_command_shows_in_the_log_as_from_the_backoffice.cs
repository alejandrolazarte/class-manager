using ClassManager.Backoffice.Api.I.Tests.Brands;

namespace ClassManager.Backoffice.Api.I.Tests.Logs.When_an_operator_changes_a_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_command_shows_in_the_log_as_from_the_backoffice(BackofficeApiFixture fixture)
{
    private const string ChangePlanCommand = "ChangeBrandPlanCommand";

    [Fact]
    public async Task Then_the_command_shows_in_the_log_as_from_the_backoffice_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture);
        var day = DateOnly.FromDateTime(fixture.ApiFactory.TimeProvider.GetUtcNow().UtcDateTime);
        using (var change = await changes.ChangePlanAsync("enterprise"))
        {
            change.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        bool IsTheChange(CommandLogEntryResponse entry) =>
            entry.Command == ChangePlanCommand && entry.Payload!.Contains(changes.Brand.Id.ToString(), StringComparison.Ordinal);
        var log = await changes.Client.WaitForCommandLogAsync(changes.AccessToken, day, IsTheChange);

        var entry = log.Entries.Single(IsTheChange);
        (entry.Outcome, entry.Source).ShouldBe(("Succeeded", "Backoffice"));
    }
}
