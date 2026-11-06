namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_a_branch_outside_europe_connects;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var brand = await fixture.SignUpBrandAsync(ApiFixture.ArgentinaCountryCode, withSelfServiceBilling: false);

        using var response = await brand.HttpClient.PostOnlinePaymentOnboardingAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        fixture.ApiFactory.OnlinePaymentGateway.AccountsCreatedFor(brand.BusinessId).ShouldBeEmpty();
    }
}
