using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructors_are_listed_and_one_is_also_a_student;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_names_the_client(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_names_the_client_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var (instructorId, _) = await fixture.SeedInstructorWhoIsAStudentAsync(scenario);

        var instructors = await scenario.Instructors.Business.HttpClient.GetFromJsonAsync<List<InstructorResponse>>(
            new Uri(ApiRoutes.Instructors, UriKind.Relative), ApiRequests.JsonOptions);

        instructors!.Single(instructor => instructor.Id == instructorId).SamePersonClientId.ShouldBe(scenario.ClientId);
        instructors!.Where(instructor => instructor.Id != instructorId).ShouldAllBe(instructor => instructor.SamePersonClientId == null);
    }
}
