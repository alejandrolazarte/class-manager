namespace ClassManager.Api.I.Tests.Endpoints.PlanSales.When_a_pack_is_sold_as_a_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var studentId = await business.HttpClient.RegisterEnrolledStudentAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();

        using var response = await business.HttpClient.PostPlanSaleAsync(studentId, classPack.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
