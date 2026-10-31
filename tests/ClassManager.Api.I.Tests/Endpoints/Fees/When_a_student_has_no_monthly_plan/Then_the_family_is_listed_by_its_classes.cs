namespace ClassManager.Api.I.Tests.Endpoints.Fees.When_a_student_has_no_monthly_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_family_is_listed_by_its_classes(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_family_is_listed_by_its_classes_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.EnrollClientWithMonthlyPlanAsync();

        using var response = await business.HttpClient.PutStudentPlanAsync(family.StudentId, null);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var fees = await business.HttpClient.GetMonthlyFeesAsync();
        fees!.Clients.ShouldBeEmpty();
        fees.ClassPackClients.Single().ClientId.ShouldBe(family.ClientId);
    }
}
