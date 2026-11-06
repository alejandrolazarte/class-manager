namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_the_brand_has_no_online_payments_add_on;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);

        using var response = await brand.HttpClient.PostOnlinePaymentOnboardingAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        fixture.ApiFactory.OnlinePaymentGateway.AccountsCreatedFor(brand.BusinessId).ShouldBeEmpty();
    }
}
