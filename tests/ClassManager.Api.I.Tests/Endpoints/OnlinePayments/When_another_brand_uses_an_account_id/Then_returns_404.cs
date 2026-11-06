namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_another_brand_uses_an_account_id;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var owner = await fixture.SignUpBrandWithOnlinePaymentsAsync();
        await owner.HttpClient.StartOnlinePaymentOnboardingAsync();
        var account = await fixture.OnlinePaymentAccountOfAsync(owner.BusinessId);
        var stranger = await fixture.SignUpBrandWithOnlinePaymentsAsync();

        using var response = await stranger.HttpClient.PutOnlinePaymentAccountAsync(account.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
