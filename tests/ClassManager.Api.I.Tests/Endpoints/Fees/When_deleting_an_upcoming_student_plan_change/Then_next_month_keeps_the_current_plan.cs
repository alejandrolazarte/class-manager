namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_deleting_an_upcoming_student_plan_change;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_next_month_keeps_the_current_plan(ApiFixture fixture)
{
    [Fact]
    public async Task Then_next_month_keeps_the_current_plan_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.EnrollClientWithMonthlyPlanAsync();
        (await business.HttpClient.PutStudentPlanAsync(family.StudentId, family.PlanId, 9000m, FeeRequests.NextMonth)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.DeleteStudentPlanChangeAsync(family.StudentId, FeeRequests.NextMonth);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await business.HttpClient.GetMonthlyFeesAsync(FeeRequests.NextMonth))!.Clients.Single().Fee.ShouldBe(FeeRequests.DefaultFee);
    }
}
