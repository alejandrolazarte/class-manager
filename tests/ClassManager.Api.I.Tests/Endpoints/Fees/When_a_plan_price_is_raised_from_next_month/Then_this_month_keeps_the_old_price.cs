using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_a_plan_price_is_raised_from_next_month;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_this_month_keeps_the_old_price(ApiFixture fixture)
{
    [Fact]
    public async Task Then_this_month_keeps_the_old_price_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.EnrollClientWithMonthlyPlanAsync();
        var plan = (await business.HttpClient.GetFromJsonAsync<IReadOnlyList<ClassPackResponse>>(ApiRoutes.ClassPacks, ApiRequests.JsonOptions))!
            .Single(classPack => classPack.Id == family.PlanId);

        using var response = await business.HttpClient.PutPlanPriceAsync(plan, 15000m, FeeRequests.NextMonth);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await business.HttpClient.GetMonthlyFeesAsync())!.Clients.Single().Fee.ShouldBe(FeeRequests.DefaultFee);
        (await business.HttpClient.GetMonthlyFeesAsync(FeeRequests.NextMonth))!.Clients.Single().Fee.ShouldBe(15000m);
    }
}
