namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_a_branch_outside_europe_reads_online_payments;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_are_not_available(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_are_not_available_Run()
    {
        var brand = await fixture.SignUpBrandWithOnlinePaymentsAsync(ApiFixture.ArgentinaCountryCode);

        var onlinePayments = await brand.HttpClient.ReadOnlinePaymentsAsync();

        onlinePayments.IsAvailable.ShouldBeFalse();
    }
}
