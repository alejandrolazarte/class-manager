using ClassManager.Core.UseCases.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_brand_without_self_service_billing_reads_the_subscription;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_self_service_is_not_available(ApiFixture fixture)
{
    [Fact]
    public async Task Then_self_service_is_not_available_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);

        using var response = await brand.HttpClient.GetOrganizationSubscriptionAsync();

        var subscription = await response.Content.ReadFromJsonAsync<OrganizationSubscriptionResponse>(ApiRequests.JsonOptions);
        subscription!.SelfService.ShouldBeFalse();
    }
}
