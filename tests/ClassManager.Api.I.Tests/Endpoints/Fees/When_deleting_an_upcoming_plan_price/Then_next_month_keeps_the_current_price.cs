using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_deleting_an_upcoming_plan_price;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_next_month_keeps_the_current_price(ApiFixture fixture)
{
    [Fact]
    public async Task Then_next_month_keeps_the_current_price_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.EnrollClientWithMonthlyPlanAsync();
        var plan = (await business.HttpClient.GetFromJsonAsync<IReadOnlyList<ClassPackResponse>>(ApiRoutes.ClassPacks, ApiRequests.JsonOptions))!
            .Single(classPack => classPack.Id == family.PlanId);
        (await business.HttpClient.PutPlanPriceAsync(plan, 15000m, FeeRequests.NextMonth)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.DeletePlanPriceChangeAsync(plan.Id, FeeRequests.NextMonth);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await business.HttpClient.GetMonthlyFeesAsync(FeeRequests.NextMonth))!.Clients.Single().Fee.ShouldBe(FeeRequests.DefaultFee);
    }
}
