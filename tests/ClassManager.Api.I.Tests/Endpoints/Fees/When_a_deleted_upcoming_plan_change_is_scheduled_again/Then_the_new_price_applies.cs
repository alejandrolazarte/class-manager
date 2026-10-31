namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_a_deleted_upcoming_plan_change_is_scheduled_again;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_new_price_applies(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_new_price_applies_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.EnrollClientWithMonthlyPlanAsync();
        (await business.HttpClient.PutStudentPlanAsync(family.StudentId, family.PlanId, 9000m, FeeRequests.NextMonth)).EnsureSuccessStatusCode();
        (await business.HttpClient.DeleteStudentPlanChangeAsync(family.StudentId, FeeRequests.NextMonth)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.PutStudentPlanAsync(family.StudentId, family.PlanId, 7000m, FeeRequests.NextMonth);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await business.HttpClient.GetMonthlyFeesAsync(FeeRequests.NextMonth))!.Clients.Single().Fee.ShouldBe(7000m);
    }
}
