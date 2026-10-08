using ClassManager.Core.UseCases.Enrollments;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_ends_an_enrollment_of_another_instructor_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var roster = await scenario.Business.HttpClient.GetRosterAsync(scenario.OtherClassGroup.Id);

        using var response = await scenario.Instructor.PutAsJsonAsync(
            $"{ApiRoutes.Enrollments}/{roster!.Single().EnrollmentId}{ApiRoutes.End}",
            new EndEnrollmentRequest(null),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
