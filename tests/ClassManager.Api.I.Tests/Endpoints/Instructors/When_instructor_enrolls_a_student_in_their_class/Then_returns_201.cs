using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_enrolls_a_student_in_their_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_201(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_201_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var client = await scenario.Instructor.RegisterClientAsync(phoneNumber: "11 4455-6677", students: [new NewStudent("Juana Ruiz", null, null)]);

        using var response = await scenario.Instructor.PostEnrollmentAsync(scenario.InstructorClassGroup.Id, client.Students[0].Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
