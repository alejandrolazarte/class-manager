using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_brand_owner_reads_the_subscription;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_real_price_is_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_real_price_is_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Enterprise);

        using var response = await business.HttpClient.GetOrganizationSubscriptionAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var subscription = (await response.Content.ReadFromJsonAsync<OrganizationSubscriptionResponse>(ApiRequests.JsonOptions))!;
        subscription.PlanCode.ShouldBe(PlanCodes.Enterprise);
        subscription.Price.ShouldBe(0m);
        subscription.Currency.ShouldBe(ApiFixture.CurrencyCode);
    }
}
