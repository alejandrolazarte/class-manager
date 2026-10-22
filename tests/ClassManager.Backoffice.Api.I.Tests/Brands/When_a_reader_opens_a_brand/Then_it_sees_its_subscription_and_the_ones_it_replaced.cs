using System.Net.Http.Json;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Subscriptions.Subscribers;

namespace ClassManager.Backoffice.Api.I.Tests.Brands.When_a_reader_opens_a_brand;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_sees_its_subscription_and_the_ones_it_replaced(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_it_sees_its_subscription_and_the_ones_it_replaced_Run()
    {
        var brand = await fixture.SeedBrandAsync("Estudio con historia", [1], brandId =>
        {
            var trial = Subscription.Start(brandId, "free", 0m, "EUR", null, BackofficeApiFactory.Now.AddDays(-30));
            trial.Delete(BackofficeApiFactory.Now.AddDays(-1));
            return [trial, Subscription.Start(brandId, "pro", 29m, "EUR", "Precio de lanzamiento", BackofficeApiFactory.Now.AddDays(-1))];
        });
        using var client = fixture.ApiFactory.CreateHttpsClient();
        var accessToken = await client.OpenSessionAsync(await fixture.SeedOperatorAsync(BusinessRole.PlatformReader));

        using var response = await client.GetAsync($"{BackofficeRequests.BrandsRoute}/{brand.Id}", accessToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var detail = (await response.Content.ReadFromJsonAsync<BrandDetailResponse>(BackofficeRequests.JsonOptions))!;
        detail.Subscription.ShouldBe(new BrandSubscriptionResponse("pro", 29m, "EUR"));
        detail.PastSubscriptions.ShouldHaveSingleItem().PlanCode.ShouldBe("free");
        detail.Owners.ShouldHaveSingleItem().Email.ShouldBe(brand.OwnerEmail);
        detail.Branches.ShouldHaveSingleItem().StudentCount.ShouldBe(1);
    }
}
