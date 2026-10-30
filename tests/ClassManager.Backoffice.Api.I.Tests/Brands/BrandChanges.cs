using System.Net.Http.Json;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Api.I.Tests.Brands;

internal sealed class BrandChanges(BackofficeApiFixture fixture)
{
    public const string Currency = "EUR";

    public SeededBrand Brand { get; private set; } = null!;

    public HttpClient Client { get; } = fixture.ApiFactory.CreateHttpsClient();

    public string AccessToken { get; private set; } = string.Empty;

    public static async Task<BrandChanges> InSpainAsync(BackofficeApiFixture fixture, Func<long, IReadOnlyList<Subscription>> subscriptionsOf)
    {
        var changes = new BrandChanges(fixture);
        changes.Brand = await fixture.SeedBrandAsync($"Marca {TestIds.Next()}", [0], subscriptionsOf, BackofficeApiFixture.SpainCountryCode);
        changes.AccessToken = await changes.Client.OpenSessionAsync(await fixture.SeedOperatorAsync(BusinessRole.PlatformOperator));
        return changes;
    }

    public static async Task<BrandChanges> OnProAsync(BackofficeApiFixture fixture, BusinessRole role = BusinessRole.PlatformOperator)
    {
        var changes = new BrandChanges(fixture);
        changes.Brand = await fixture.SeedBrandAsync(
            $"Marca {TestIds.Next()}",
            [1],
            brandId => [Subscription.Start(brandId, "pro", 29m, Currency, null, BackofficeApiFactory.Now.AddDays(-10))]);
        changes.AccessToken = await changes.Client.OpenSessionAsync(await fixture.SeedOperatorAsync(role));
        return changes;
    }

    public string BrandRoute => $"{BackofficeRequests.BrandsRoute}/{Brand.Id}";

    public Task<HttpResponseMessage> ChangePlanAsync(string planCode, decimal price = 12m) =>
        Client.SendAsync(HttpMethod.Put, $"{BrandRoute}/subscription", AccessToken, new { planCode, price, currency = Currency, note = "Precio acordado" });

    public Task<HttpResponseMessage> OfferPlanAsync(string planCode, decimal price) =>
        Client.SendAsync(HttpMethod.Put, $"{BrandRoute}/offer", AccessToken, new { planCode, price, note = "Precio de lanzamiento" });

    public Task<HttpResponseMessage> WithdrawOfferAsync() =>
        Client.SendAsync(HttpMethod.Delete, $"{BrandRoute}/offer", AccessToken);

    public async Task<BrandAddOnResponse> GiveAddOnAsync(string featureCode, int? limit)
    {
        using var response = await Client.SendAsync(HttpMethod.Post, $"{BrandRoute}/add-ons", AccessToken, new { featureCode, price = 10m, currency = Currency, limit });
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<BrandAddOnResponse>(BackofficeRequests.JsonOptions))!;
    }

    public async Task<BrandDetailResponse> GetBrandAsync()
    {
        using var response = await Client.GetAsync(BrandRoute, AccessToken);
        return (await response.Content.ReadFromJsonAsync<BrandDetailResponse>(BackofficeRequests.JsonOptions))!;
    }
}
