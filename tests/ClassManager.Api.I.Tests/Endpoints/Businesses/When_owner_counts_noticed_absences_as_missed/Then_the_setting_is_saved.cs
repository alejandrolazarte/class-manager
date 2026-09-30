namespace ClassManager.Api.I.Tests.Endpoints.Businesses.When_owner_counts_noticed_absences_as_missed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_setting_is_saved(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_setting_is_saved_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        (await business.HttpClient.GetCurrentBusinessAsync())!.NoticedAbsencesKeepStreak.ShouldBeTrue();

        using var response = await business.HttpClient.PutBusinessAsync(SettingsRequests.SettingsOf(business, noticedAbsencesKeepStreak: false));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await business.HttpClient.GetCurrentBusinessAsync())!.NoticedAbsencesKeepStreak.ShouldBeFalse();
    }
}
