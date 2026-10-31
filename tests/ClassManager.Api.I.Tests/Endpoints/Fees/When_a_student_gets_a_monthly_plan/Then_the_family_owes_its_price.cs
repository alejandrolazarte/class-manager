using ClassManager.Core.Domain.Fees;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_a_student_gets_a_monthly_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_family_owes_its_price(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_family_owes_its_price_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        var family = await business.HttpClient.EnrollClientWithMonthlyPlanAsync();

        var client = (await business.HttpClient.GetMonthlyFeesAsync())!.Clients.Single();
        client.ClientId.ShouldBe(family.ClientId);
        client.Status.ShouldBe(FeeStatus.Unpaid);
        client.Balance.ShouldBe(FeeRequests.DefaultFee);
    }
}
