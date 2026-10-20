namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_a_removed_add_on_is_restored;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_counts_again(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_it_counts_again_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture);
        var addOn = await changes.GiveAddOnAsync("branches", limit: 5);
        using var removal = await changes.Client.SendAsync(HttpMethod.Delete, $"{changes.BrandRoute}/add-ons/{addOn.Id}", changes.AccessToken);
        (await changes.GetBrandAsync()).AddOns.ShouldBeEmpty();

        using var restore = await changes.Client.SendAsync(HttpMethod.Post, $"{changes.BrandRoute}/add-ons/{addOn.Id}/restore", changes.AccessToken);

        restore.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await changes.GetBrandAsync()).Features.ShouldContain(new BrandFeatureResponse("branches", 5));
    }
}
