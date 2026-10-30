using ClassManager.Core.UseCases.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Billing.When_a_brand_that_never_paid_opens_the_billing_portal;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var brand = await fixture.SignUpBrandAsync();

        using var response = await brand.HttpClient.PostBillingPortalAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(BillingErrorCodes.NoCustomer);
    }
}
