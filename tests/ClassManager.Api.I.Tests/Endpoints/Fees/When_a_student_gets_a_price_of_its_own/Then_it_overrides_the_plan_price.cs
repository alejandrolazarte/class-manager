namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_a_student_gets_a_price_of_its_own;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_overrides_the_plan_price(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_overrides_the_plan_price_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.EnrollClientWithMonthlyPlanAsync();

        using var response = await business.HttpClient.PutStudentPlanAsync(family.StudentId, family.PlanId, 9000m);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await business.HttpClient.GetMonthlyFeesAsync())!.Clients.Single().Fee.ShouldBe(9000m);
    }
}
