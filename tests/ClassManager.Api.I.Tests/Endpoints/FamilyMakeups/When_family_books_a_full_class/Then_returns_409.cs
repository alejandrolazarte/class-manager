using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyMakeups.When_family_books_a_full_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var coaches = scenario.Coaches;
        var owner = coaches.Business.HttpClient;
        var fullClassGroup = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(coaches.OtherInstructorId, [CoachScenario.ClassDate.DayOfWeek], "20:00") with { Name = "Natación llena", Capacity = 1 });
        await owner.EnrollAsync(fullClassGroup.Id, coaches.OtherStudentId, CoachScenario.ClassDate);
        (await scenario.Family.PutAbsenceAsync(coaches.CoachStudentId, coaches.CoachClassGroup.Id, CoachScenario.ClassDate)).EnsureSuccessStatusCode();

        using var response = await scenario.Family.PutMakeupAsync(coaches.CoachStudentId, fullClassGroup.Id, CoachScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(SessionErrorCodes.MakeupFull);
    }
}
