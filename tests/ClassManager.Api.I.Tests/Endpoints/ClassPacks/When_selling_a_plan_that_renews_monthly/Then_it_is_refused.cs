using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_selling_a_plan_that_renews_monthly;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        var plan = await business.HttpClient.CreateMonthlyPlanAsync();

        using var response = await business.HttpClient.PostClassPackSaleAsync(client.Id, plan.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(ClassPackErrorCodes.RenewsMonthly);
    }
}
