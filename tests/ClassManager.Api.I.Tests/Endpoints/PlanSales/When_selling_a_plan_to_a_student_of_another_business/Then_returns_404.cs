namespace ClassManager.Api.I.Tests.Endpoints.PlanSales.When_selling_a_plan_to_a_student_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherStudentId = await otherBusiness.HttpClient.RegisterEnrolledStudentAsync();
        var plan = await business.HttpClient.CreateMonthlyPlanAsync();

        using var response = await business.HttpClient.PostPlanSaleAsync(otherStudentId, plan.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
