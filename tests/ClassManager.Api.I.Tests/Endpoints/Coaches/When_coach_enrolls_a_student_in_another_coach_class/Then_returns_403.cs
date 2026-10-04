using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_enrolls_a_student_in_another_coach_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();
        var client = await scenario.Coach.RegisterClientAsync(phoneNumber: "11 4455-6677", students: [new NewStudent("Juana Ruiz", null, null)]);

        using var response = await scenario.Coach.PostEnrollmentAsync(scenario.OtherClassGroup.Id, client.Students[0].Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
