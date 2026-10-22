using System.Net.Http.Json;

namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_an_add_on_ends_this_week;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_listed_with_its_brand(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_listed_with_its_brand_Run()
    {
        var changes = await BrandChanges.OnProAsync(fixture);
        var endsThisWeek = DateOnly.FromDateTime(BackofficeApiFactory.Now.UtcDateTime).AddDays(3);
        using var given = await changes.Client.SendAsync(
            HttpMethod.Post,
            $"{changes.BrandRoute}/add-ons",
            changes.AccessToken,
            new { featureCode = "shop", price = 0m, currency = BrandChanges.Currency, limit = (int?)null, expiredOn = endsThisWeek });
        given.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var response = await changes.Client.GetAsync("/api/expiring-add-ons", changes.AccessToken);

        var expiring = (await response.Content.ReadFromJsonAsync<ExpiringAddOnList>(BackofficeRequests.JsonOptions))!.AddOns;
        expiring.ShouldContain(new ExpiringAddOn(changes.Brand.Id, "shop", endsThisWeek));
    }

    private sealed record ExpiringAddOnList(IReadOnlyList<ExpiringAddOn> AddOns);

    private sealed record ExpiringAddOn(long BrandId, string FeatureCode, DateOnly ExpiredOn);
}
