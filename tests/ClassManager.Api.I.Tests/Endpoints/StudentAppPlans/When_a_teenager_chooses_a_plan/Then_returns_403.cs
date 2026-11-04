namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPlans.When_a_teenager_chooses_a_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        using var teenager = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Lucía Pérez", StudentAppRequests.ChildBirthDate);
        var plan = await business.HttpClient.CreateMonthlyPlanAsync();

        using var response = await teenager.App.PutStudentAppPlanAsync(teenager.Student.Id, plan.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
