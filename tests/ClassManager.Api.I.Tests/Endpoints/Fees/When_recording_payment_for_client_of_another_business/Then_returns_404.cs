namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_recording_payment_for_client_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherClient = await otherBusiness.HttpClient.RegisterClientAsync();

        using var response = await business.HttpClient.PostPaymentAsync(otherClient.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
